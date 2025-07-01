# app/modules/user/models.py
from sqlalchemy import Column, Integer, String
from sqlalchemy.orm import relationship
from app.database import Base

class User(Base):
    __tablename__ = "users"

    id = Column(Integer, primary_key=True, index=True)
    username = Column(String, unique=True, index=True, nullable=False)
    hashed_password = Column(String, nullable=False)

    inferences = relationship("Inference", back_populates="user", cascade="all, delete-orphan")

    def to_dict(self):
        data = {
            "id": self.id,
            "username": self.username,
            "inferences": [inf.to_dict() for inf in self.inferences]
        }

        return data