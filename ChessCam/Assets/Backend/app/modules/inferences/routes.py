from fastapi import APIRouter, Depends, Form, HTTPException, status, FastAPI, UploadFile, File
from sqlalchemy.ext.asyncio import AsyncSession
from app.database import get_db
from app.modules.inferences.models import Inference
from app.modules.inferences.services import get_inferences_by_user_id
from app.modules.users.services import get_user_by_id
from ultralytics import YOLO
from PIL import Image
from datetime import datetime
import io
from datetime import datetime, timedelta, time
from sqlalchemy import select, func
import json
inferences_router = APIRouter(prefix="/inferences", tags=["Inferences"])

# Load YOLO model once at startup
model = YOLO("app/best.pt")

@inferences_router.post("/detect")
async def detect(
    file: UploadFile = File(...), 
    image_width: int = Form(...),
    image_height: int = Form(...),
    user_id: int = Form(...),
    db: AsyncSession = Depends(get_db)):
    
    if user_id:
        user = await get_user_by_id(db, user_id)
        if not user:
            raise HTTPException(status_code=404, detail="User not found")

    num_inf = await num_inference_today(db, user_id)
    if num_inf >= 3:
        return {
            "num_inf": num_inf,
            "predictions": []
        }

    image_data = await file.read()
    image = Image.open(io.BytesIO(image_data)).convert("RGB")

    # Run inference
    results = model(image)
    detections = results[0]

    predictions = []
    names = model.model.names  # class ID -> name mapping

    for box, conf, cls_id in zip(detections.boxes.xyxy, detections.boxes.conf, detections.boxes.cls):
        x1, y1, x2, y2 = box.tolist()
        x_center = round((x1 + x2) / 2, 2)
        y_center = round((y1 + y2) / 2, 2)

        predictions.append({
            "x": x_center,
            "y": y_center,
            "confidence": round(float(conf), 2),
            "name": names[int(cls_id)]
        })
    

    inference = Inference(
        predictions=json.dumps(predictions),
        image_width=image_width,
        image_height=image_height,
        user_id=user_id
    )

    db.add(inference)
    await db.commit()
    await db.refresh(inference)

    return {
        "num_inf": num_inf,
        "predictions": predictions
    }


@inferences_router.get("/history")
async def get_user_inferences(user_id: int, db: AsyncSession = Depends(get_db)):
    result = await get_inferences_by_user_id(db, user_id)
    return {"history": [inf.to_dict() for inf in result]}

async def num_inference_today(db: AsyncSession, user_id: int) -> int:
    now = datetime.utcnow()
    today_start = datetime.combine(now.date(), time.min)
    today_end = datetime.combine(now.date(), time.max)

    stmt = select(func.count()).select_from(Inference).where(
        Inference.user_id == user_id,
        Inference.created_at >= today_start,
        Inference.created_at <= today_end
    )
    result = await db.execute(stmt)
    return result.scalar()