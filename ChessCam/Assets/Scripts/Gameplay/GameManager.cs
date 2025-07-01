using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PlayerTurn
{
    White,
    Black
}

public class GameManager : Singleton<GameManager>
{
    public PlayerTurn currentTurn;
    public Toggle[] turnToggles;

    [Header("Tile Highlights")]
    public GameObject moveDotPrefab;
    public GameObject capturablePrefab;

    private List<GameObject> moveDots = new List<GameObject>();
    private List<GameObject> capturables = new List<GameObject>();

    private ChessPiece selectedPiece;
    private void Start()
    {
        currentTurn = PlayerTurn.White;
        turnToggles[0].onValueChanged.AddListener((isOn) => { if (isOn) SetWhiteTurn(); });
        turnToggles[1].onValueChanged.AddListener((isOn) => { if (isOn) SetBlackTurn(); });

        UpdateToggleUI();
    }
    private void UpdateToggleUI()
    {
        if (currentTurn == PlayerTurn.White)
        {
            turnToggles[0].isOn = true;
        }
        else
        {
            turnToggles[1].isOn = true;
        }
    }

    public void OnPieceClicked(ChessPiece piece)
    {
        var tileMap = TileSpawner.Instance.tileMap;

        //Highlight tile
        if (tileMap.TryGetValue(piece.gridPos, out ChessTile targetTile))
        {
            targetTile.HighlightTile(true);
        }

        //Check turn and capture
        if (!IsTurnFor(piece.pieceType))
        {
            var board = ChessSpawner.Instance.boardMap;
            // Capture
            if (selectedPiece != null && selectedPiece.IsEnemyAt(piece.gridPos) && selectedPiece.IsValidMove(selectedPiece.gridPos, piece.gridPos))
            {
                Debug.Log("3");
                Destroy(piece.gameObject);
                board.Remove(piece.gridPos);

                board.Remove(selectedPiece.gridPos);
                board[piece.gridPos] = selectedPiece;

                selectedPiece.GetRectTransform().anchoredPosition = ChessSpawner.Instance.GridToAnchoredPosition(piece.gridPos);
                selectedPiece.gridPos = piece.gridPos;
                selectedPiece.hasMoved = true;
                HandleMove();
            }

            selectedPiece = null;
            ClearMoveDots();
            return;
        }

        //Select piece
        if (selectedPiece == null)
        {
            Debug.Log("1");
            selectedPiece = piece;
            ShowValidMoves(piece);
        }
        else
        {
            //Deselect same piece
            if (selectedPiece == piece)
            {
                Debug.Log("2");
                selectedPiece = null;
                targetTile.HighlightTile(false);
                ClearMoveDots();
                return;
            }

            Debug.Log("4");
            // Select new piece
            selectedPiece = piece;
            ClearMoveDots();
            ShowValidMoves(piece);
        }

    }

    public void OnTileClicked(ChessTile tile)
    {
        if (selectedPiece == null) return;

        if (!selectedPiece.IsValidMove(selectedPiece.gridPos, tile.gridPos))
        {
            selectedPiece = null;
            ClearMoveDots();
            return;
        }

        var board = ChessSpawner.Instance.boardMap;

        // Castling
        if (selectedPiece.pieceType.Contains("king") && tile.gridPos.y == selectedPiece.gridPos.y && Mathf.Abs(tile.gridPos.x - selectedPiece.gridPos.x) == 2)
        {
            int dir = tile.gridPos.x - selectedPiece.gridPos.x;
            Vector2Int rookPos = dir < 0 ? new Vector2Int(0, tile.gridPos.y) : new Vector2Int(7, tile.gridPos.y);

            if (board.TryGetValue(rookPos, out ChessPiece rook))
            {
                board.Remove(rookPos);
                Vector2Int newRookPos = selectedPiece.gridPos + new Vector2Int(dir / 2, 0);
                board[newRookPos] = rook;
                rook.GetComponent<RectTransform>().anchoredPosition = ChessSpawner.Instance.GridToAnchoredPosition(newRookPos);
                rook.gridPos = newRookPos;
                rook.hasMoved = true;
            }
        }

        // Move selected piece
        board.Remove(selectedPiece.gridPos);
        board[tile.gridPos] = selectedPiece;

        selectedPiece.GetRectTransform().anchoredPosition = ChessSpawner.Instance.GridToAnchoredPosition(tile.gridPos);
        selectedPiece.gridPos = tile.gridPos;
        selectedPiece.hasMoved = true;

        // Promotion
        if (selectedPiece.pieceType.Contains("pawn") && (tile.gridPos.y == 0 || tile.gridPos.y == 7))
        {
            PromotionUI.Instance.DisplayPromotionOptions(selectedPiece);
        }

        HandleMove();
        ClearMoveDots();
    }

    public void HandleMove()
    {
        currentTurn = currentTurn == PlayerTurn.White ? PlayerTurn.Black : PlayerTurn.White;
        selectedPiece = null;
        UpdateToggleUI();

        bool isWhite = currentTurn == PlayerTurn.White;
        ChessSpawner.Instance.copiedBoardMap = new Dictionary<Vector2Int, ChessPiece>(ChessSpawner.Instance.boardMap);
        if (ChessRules.IsKingInCheck(isWhite))
        {
            Debug.Log($"{(isWhite ? "White" : "Black")} is in check!");
            if (ChessRules.IsCheckmate(isWhite))
            {
                Debug.Log($"{(isWhite ? "White" : "Black")} is in checkmate!");
                NotificationUI.Instance.ShowEndGameBanner(!isWhite);
            }
        }
    }


    public bool IsTurnFor(string pieceType)
    {
        return (currentTurn == PlayerTurn.White && pieceType.StartsWith("white")) ||
               (currentTurn == PlayerTurn.Black && pieceType.StartsWith("black"));
    }

    public void SetWhiteTurn()
    {
        currentTurn = PlayerTurn.White;
        UpdateToggleUI();
    }

    public void SetBlackTurn()
    {
        currentTurn = PlayerTurn.Black;
        UpdateToggleUI();
    }

    public void ShowValidMoves(ChessPiece piece)
    {
        var validMoves = piece.GetValidMoves();
        foreach (var move in validMoves)
        {
            if (!piece.IsEnemyAt(move))
            {
                Vector2 anchoredPos = ChessSpawner.Instance.GridToAnchoredPosition(move, 100f);
                GameObject dot = Instantiate(moveDotPrefab, ChessSpawner.Instance.pieceParent);
                dot.GetComponent<RectTransform>().anchoredPosition = anchoredPos;
                moveDots.Add(dot);
            }
        }
        ShowCapturableEnemies(piece);
    }


    public void ShowCapturableEnemies(ChessPiece piece)
    {
        var validMoves = piece.GetValidMoves();
        foreach (var move in validMoves)
        {
            if (piece.IsEnemyAt(move))
            {
                Vector2 anchoredPos = ChessSpawner.Instance.GridToAnchoredPosition(move, 100f);
                GameObject cap = Instantiate(capturablePrefab, TileSpawner.Instance.tileParent.transform);
                cap.GetComponent<RectTransform>().anchoredPosition = anchoredPos;
                capturables.Add(cap);
            }
        }
    }

    public void ClearMoveDots()
    {
        foreach (var dot in moveDots)
        {
            Destroy(dot);
        }
        moveDots.Clear(); 
        ClearCapturables();
    }
    public void ClearCapturables()
    {
        foreach (var cap in capturables)
        {
            Destroy(cap);
        }
        capturables.Clear();
    }
}


