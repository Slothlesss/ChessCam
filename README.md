![release](https://img.shields.io/badge/release-v0.1.0-orange?style=flat-square)
# ♟️ ChessCam 📸

**ChessCam** is an chessboard recognition app that detects the positions of chess pieces from an image and recreates the board, allowing players to continue a gameplay.

![image](https://github.com/user-attachments/assets/39db63a6-d988-4326-8938-20e56a8a41dc)


## 1. Playable version
You can try the playable web build hosted on Itch.io:
<p align="center">
  <a href="https://slothless.itch.io/chesscam">
    <img src="https://img.shields.io/badge/Try%20it%20here-4CAF50?style=for-the-badge" alt="Try it here">
  </a>
</p>


## 2. Instruction
### 2.1 Login and Registration
![image](https://github.com/user-attachments/assets/d80a239e-3c9f-44a4-a4da-1bd231b3ce7a)
Your username and password will be stored, and the password encrypted with Werkzerg.

### 2.2 Inference
- Step 1: Click "Choose Image" to upload a chessboard image.
- Step 2: Click "Inference" and wait until you see "Sucessfully inference...".
- Step 3: Click "Spawn Pieces".
- Step 4: (Optional) Change the settings (turn and board flip).
=> You can continue the gameplay now.

### 2.3 Inference History
![image](https://github.com/user-attachments/assets/bf278c46-6c35-4c47-8a71-24c8b3292eec)
- Each user will have 3 inference attempts per day (reset at 00:00 UTC time).
- You can click to history image to spawn the board.

## 3. Deploy with Docker
### 3.1 Install Docker
Ensure you have [Docker](https://docs.docker.com/get-started/get-docker/) installed.

### 3.2 Build Docker Image
```
cd Backend
docker build -t chesscam-backend .
```

### 3.3 Run the container
```
docker run -d -p 8000:8000 chesscam-backend
```

### 3.4 Change BaseUrl
- Open the file: `Assets/Script/Network/APIConfig.cs`
- Update BaseUrl:
```
public static string BaseUrl = "http://localhost:8000";
```

## 4. Asset Attribution

### Chess Piece Assets
- The chess piece images used in this project are sourced from [**Lichess**](https://lichess.org).
- Lichess piece sets are released under the **Creative Commons Attribution 4.0 International (CC BY 4.0)** license.
- You can find the original asset sets at: https://github.com/lichess-org/lila/tree/master/public/piece

✅ Attribution (as required by CC BY 4.0):
> Chess piece graphics © [lichess.org](https://lichess.org), used under CC BY 4.0.
