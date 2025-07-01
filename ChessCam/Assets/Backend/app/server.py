from fastapi import FastAPI, UploadFile, File
from fastapi.middleware.cors import CORSMiddleware
from ultralytics import YOLO
from PIL import Image

from app.modules.users.routes import users_router
from app.modules.inferences.routes import inferences_router
from app.modules.users.models import User
from app.modules.inferences.models import Inference
from app.database import engine
from app.database import Base

app = FastAPI()

app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

@app.on_event("startup")
async def on_startup():
    async with engine.begin() as conn:
        await conn.run_sync(Base.metadata.create_all)

app.include_router(users_router)
app.include_router(inferences_router)

@app.api_route("/health", methods=["GET", "HEAD"])
def root():
    return {"message": "Server is healthy"}
