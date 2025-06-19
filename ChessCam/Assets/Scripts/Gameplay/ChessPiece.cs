using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChessPiece : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;
    public static ChessPiece selectedPiece;

    public Vector2Int gridPos;
    public string pieceType; 
    public bool hasMoved = false;


    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!GameManager.Instance.IsTurnFor(pieceType))
        {
            NotificationUI.Instance.ShowMessage($"Not {GameManager.Instance.currentTurn} turn", true);
            Debug.Log("Not your turn: " + pieceType);
            return;
        }

        originalPosition = rectTransform.anchoredPosition; 
        foreach (var kvp in ChessSpawner.Instance.boardMap)
            kvp.Value.GetComponent<CanvasGroup>().blocksRaycasts = false;
        selectedPiece = this;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (selectedPiece != null)
        {
            rectTransform.anchoredPosition += eventData.delta;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {

        if (selectedPiece == null) return;
        
        foreach (var kvp in ChessSpawner.Instance.boardMap)
            kvp.Value.GetComponent<CanvasGroup>().blocksRaycasts = true;

        GameObject tileObject = null;

        foreach (GameObject obj in eventData.hovered)
        {
            if (obj.CompareTag("Tile"))
            {
                tileObject = obj;
                break;
            }
        }

        if (tileObject == null)
        {
            Debug.Log("CancelMove: No tile found under pointer");
            CancelMove();
            return;
        }

        Vector2Int targetGrid = tileObject.GetComponent<ChessTile>().gridPos;

        if (!IsValidMove(gridPos, targetGrid))
        {
            NotificationUI.Instance.ShowMessage("CancelMove: Invalid move", true);
            CancelMove();
            return;
        }

        var board = ChessSpawner.Instance.boardMap;

        // Capture enemy if present
        if (board.TryGetValue(targetGrid, out ChessPiece targetPiece))
        {
            if (IsEnemyAt(targetGrid, IsWhite()))
            {
                Destroy(targetPiece.gameObject);
                board.Remove(targetGrid);
            }
            else
            {
                Debug.Log("CancelMove: Occupied by ally");
                CancelMove();
                return;
            }
        }

        if (pieceType.Contains("king") && targetGrid.y == gridPos.y && Mathf.Abs(targetGrid.x - gridPos.x) == 2)
        {
            int dir = targetGrid.x - gridPos.x;

            Vector2Int rookPos = dir < 0 ? new Vector2Int(0, gridPos.y) : new Vector2Int(7, gridPos.y); 
            if (board.TryGetValue(rookPos, out ChessPiece rookPiece))
            {
                board.Remove(rookPos);
                Vector2Int newRookPos = gridPos + new Vector2Int(dir / 2, 0);
                board[newRookPos] = rookPiece;
                rookPiece.rectTransform.anchoredPosition = ChessSpawner.Instance.GridToAnchoredPosition(newRookPos);
                rookPiece.gridPos = newRookPos;
                rookPiece.hasMoved = true;
            }
        }

        // Move the piece
        board.Remove(gridPos);
        board[targetGrid] = this;
        rectTransform.anchoredPosition = tileObject.GetComponent<RectTransform>().anchoredPosition;
        gridPos = targetGrid;

        if (pieceType.Contains("pawn") && (gridPos.y == 0 || gridPos.y == 7))
        {
            PromotionUI.Instance.DisplayPromotionOptions(this);
        }

        hasMoved = true;
        selectedPiece = null;

        GameManager.Instance.HandleMove(pieceType);
    }

    private void CancelMove()
    {
        rectTransform.anchoredPosition = originalPosition;
        canvasGroup.blocksRaycasts = true;
        selectedPiece = null;
    }

    public bool IsValidMove(Vector2Int from, Vector2Int to)
    {
        string typeOnly = pieceType.Contains('-') ? pieceType.Split('-')[1] : pieceType;

        List<Vector2Int> validMoves = typeOnly switch
        {
            "knight" => ChessRules.GetKnightMoves(from),
            "king" => ChessRules.GetKingMovesWithCastling(
                from,
                IsWhite(),
                hasMoved,
                IsSquareOccupied
            ),
            "rook" => ChessRules.GetRookMoves(
                from, 
                IsSquareOccupied, 
                pos => IsEnemyAt(pos, IsWhite())
            ),
            "bishop" => ChessRules.GetBishopMoves(
                from,
                IsSquareOccupied,
                pos => IsEnemyAt(pos, IsWhite())
            ),
            "queen" => ChessRules.GetQueenMoves(
                from,
                IsSquareOccupied,
                pos => IsEnemyAt(pos, IsWhite())
            ),
            "pawn" => ChessRules.GetPawnMoves(
                from,
                IsWhite(),
                IsSquareOccupied,
                pos => IsEnemyAt(pos, IsWhite())
            ),
            _ => null
        };

        return validMoves != null && validMoves.Contains(to);
    }

    private bool IsSquareOccupied(Vector2Int pos)
    {
        return ChessSpawner.Instance.boardMap.ContainsKey(pos);
    }

    private bool IsEnemyAt(Vector2Int pos, bool isWhite)
    {
        if (ChessSpawner.Instance.boardMap.TryGetValue(pos, out ChessPiece piece))
        {
            return piece.IsWhite() != isWhite;
        }
        return false;
    }


    private bool IsWhite() => pieceType.StartsWith("white");

    public RectTransform GetRectTransform()
    {
        return rectTransform;
    }

}
