FROM python:3.10-slim

ENV YOLO_CONFIG_DIR=/tmp/ultralytics_config

# Install system dependencies
RUN apt-get update && apt-get install -y \
    libglib2.0-0 libsm6 libxext6 libxrender1 libgl1 \
    && rm -rf /var/lib/apt/lists/*

# Copy the whole root folder
COPY . . 

# Install CPU-only PyTorch first, then requirements
RUN pip install --no-cache-dir \
    torch torchvision torchaudio --index-url https://download.pytorch.org/whl/cpu \
 && pip install --no-cache-dir -r requirements.txt \
 && pip install --no-cache-dir gdown

RUN mkdir -p app
RUN gdown 1e4VFO8N-wa17gOjylvdl6-_Z9PS45x-2 -O app/best.pt

# Launch FastAPI via Uvicorn
CMD ["uvicorn", "app.server:app", "--host", "0.0.0.0", "--port", "8080"]
