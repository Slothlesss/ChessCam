![release](https://img.shields.io/badge/release-v0.1.0-orange?style=flat-square)
# ♟️ ChessCam 📸

**ChessCam** is an chessboard recognition app that detects the positions of chess pieces from an image and recreates the board, allowing players to continue a gameplay.
![image](https://github.com/user-attachments/assets/2a57ad38-418d-49c6-95a2-6866376a8912)

## 1. Playable version
You can try the playable web build hosted on Itch.io:
<p align="center">
  <a href="https://slothless.itch.io/chesscam">
    <img src="https://img.shields.io/badge/Try%20it%20here-4CAF50?style=for-the-badge" alt="Try it here">
  </a>
</p>

## 2. Instruction Video

https://github.com/user-attachments/assets/26e27d2b-8dc6-4725-a0f6-f99c3ad662c7

## 3. Features
### 3.1 Login and Registration
Your username and password will be stored online, and the password is encrypted with Werkzerg.

### 3.2 Inference
- Step 1: Click "Choose Image" to upload a chessboard image.
- Step 2: Click "Inference" and wait until you see "Sucessfully inference...".
- Step 3: Click "Spawn Pieces".
- Step 4: (Optional) Change the settings (turn and board flip).
=> You can continue the gameplay now.

### 3.3 Inference History
- Each user will have 3 inference attempts per day (reset at 00:00 UTC time).
- You can click to history image to spawn the board.

## 4. Deploy with Docker
### 4.1 Install Docker
Ensure you have [Docker](https://docs.docker.com/get-started/get-docker/) and [DockerCompose](https://docs.docker.com/compose/install/) installed.

### 4.2 Run the backend
```
cd Backend
docker-compose up --build
```

### 4.3 Update BaseUrl
- Open the file: `Assets/Script/Network/APIConfig.cs`
- Update BaseUrl:
```
public static string BaseUrl = "http://localhost:8000";
```

## 5. Asset Attribution

### Chess Piece Assets
- The chess piece images used in this project are sourced from [**Lichess**](https://lichess.org).
- Lichess piece sets are released under the **Creative Commons Attribution 4.0 International (CC BY 4.0)** license.
- You can find the original asset sets at: https://github.com/lichess-org/lila/tree/master/public/piece/staunty

✅ Attribution (as required by CC BY 4.0):
> Chess piece graphics © [lichess.org](https://lichess.org), used under CC BY 4.0.
