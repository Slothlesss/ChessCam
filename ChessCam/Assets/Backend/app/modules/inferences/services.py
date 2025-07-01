# app/modules/user/services.py
from sqlalchemy.ext.asyncio import AsyncSession
from sqlalchemy.future import select
from app.modules.inferences.models import Inference

async def get_inferences_by_user_id(db: AsyncSession, user_id: int):
    result = await db.execute(select(Inference).where(Inference.user_id == user_id).order_by(Inference.created_at.desc()))
    return result.scalars().all()
