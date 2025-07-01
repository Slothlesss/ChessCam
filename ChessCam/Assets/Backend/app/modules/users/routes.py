from fastapi import APIRouter, Depends
from sqlalchemy.ext.asyncio import AsyncSession
from app.database import get_db
from app.modules.users.services import create_user, get_user_by_username
from fastapi import HTTPException, status
from werkzeug.security import check_password_hash

users_router = APIRouter(prefix="/users", tags=["Users"])

@users_router.post("/register")
async def register(username: str, password: str, db: AsyncSession = Depends(get_db)):
    existing = await get_user_by_username(db, username)
    if existing:
        raise HTTPException(status.HTTP_409_CONFLICT, "Username already exists")
    
    if not username:
        raise HTTPException(status.HTTP_406_NOT_ACCEPTABLE, "Require Username")
    
    if not password:
        raise HTTPException(status.HTTP_406_NOT_ACCEPTABLE, "Require Password")
    
    if len(password) < 6:
        raise HTTPException(status.HTTP_411_LENGTH_REQUIRED, "Password can not be less than 6 characters")

    user = await create_user(db, username, password)
    return {"id": user.id, "username": user.username}

@users_router.post("/login")
async def login(username: str, password: str, db: AsyncSession = Depends(get_db)):
    existing = await get_user_by_username(db, username)
    if not existing:
        raise HTTPException(status.HTTP_404_NOT_FOUND, "Username does not exist")
    
    if not check_password_hash(existing.hashed_password, password):
        raise HTTPException(status.HTTP_401_UNAUTHORIZED, "Wrong password")

    return {"id": existing.id, "username": existing.username}
