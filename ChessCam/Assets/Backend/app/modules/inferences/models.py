# app/modules/user/models.py
from sqlalchemy import Column, Integer, String, ForeignKey, DateTime, LargeBinary, Text
from sqlalchemy.orm import relationship
from datetime import datetime
from app.database import Base
import base64

class Inference(Base):
    __tablename__ = "inferences"

    id = Column(Integer, primary_key=True, index=True)
    predictions = Column(Text, nullable=False)
    image_width = Column(Integer, nullable=False)
    image_height = Column(Integer, nullable=False)
    created_at = Column(DateTime, default=datetime.utcnow)
    # Foreign key to user
    user_id = Column(Integer, ForeignKey("users.id", ondelete="CASCADE"), nullable=False)

    user = relationship("User", back_populates="inferences")

    def to_dict(self):
        data = {
            "id": self.id,
            "predictions": self.predictions,
            "image_width": self.image_width,
            "image_height": self.image_height,
            "created_at": self.created_at.date().isoformat(),
            "user_id": self.user_id
        }

        return data
