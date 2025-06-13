using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class ChessPiece : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;
    public static ChessPiece selectedPiece;

    public Vector2Int gridPos;
    public string pieceType;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = rectTransform.anchoredPosition; 
        foreach (var kvp in ChessSpawner.Instance.boardMap)
            kvp.Value.GetComponent<CanvasGroup>().blocksRaycasts = false;
        selectedPiece = this;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
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
            Debug.Log("CancelMove: Invalid move");
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

        // Move the piece
        board.Remove(gridPos);
        board[targetGrid] = this;
        rectTransform.anchoredPosition = tileObject.GetComponent<RectTransform>().anchoredPosition;
        gridPos = targetGrid;

        if (pieceType.Contains("pawn") && (gridPos.y == 0 || gridPos.y == 7))
        {
            PromotionUIManager.Instance.DisplayPromotionOptions(this);
        }


        selectedPiece = null;
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
            "king" => ChessRules.GetKingMoves(from),
            "rook" => ChessRules.GetRookMoves(from),
            "bishop" => ChessRules.GetBishopMoves(from),
            "queen" => ChessRules.GetQueenMoves(from),
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
