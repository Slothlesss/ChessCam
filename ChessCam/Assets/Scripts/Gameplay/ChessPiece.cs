using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChessPiece : MonoBehaviour, IPointerClickHandler
{
    public static ChessPiece selectedPiece;

    private RectTransform rectTransform;
    public Vector2Int gridPos;
    public string pieceType;
    public bool hasMoved = false;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GameManager.Instance.OnPieceClicked(this);
    }

    public bool IsValidMove(Vector2Int from, Vector2Int to)
    {
        List<Vector2Int> validMoves = GetValidMoves();
        return validMoves != null && validMoves.Contains(to);
    }

    public List<Vector2Int> GetValidMoves()
    {
        string typeOnly = pieceType.Contains('-') ? pieceType.Split('-')[1] : pieceType;

        List<Vector2Int> validMoves = typeOnly switch
        {
            "knight" => ChessRules.GetKnightMoves(gridPos, IsSquareOccupied, pos => IsEnemyAt(pos)),
            "king" => ChessRules.GetKingMovesWithCastling(gridPos, IsWhite(), hasMoved, IsSquareOccupied, pos => IsEnemyAt(pos)),
            "rook" => ChessRules.GetRookMoves(gridPos, IsSquareOccupied, pos => IsEnemyAt(pos)),
            "bishop" => ChessRules.GetBishopMoves(gridPos, IsSquareOccupied, pos => IsEnemyAt(pos)),
            "queen" => ChessRules.GetQueenMoves(gridPos, IsSquareOccupied, pos => IsEnemyAt(pos)),
            "pawn" => ChessRules.GetPawnMoves(gridPos, IsWhite(), IsSquareOccupied, pos => IsEnemyAt(pos)),
            _ => null
        };
        return validMoves;
    }

    public bool IsWhite() => pieceType.StartsWith("white");

    private bool IsSquareOccupied(Vector2Int pos) => ChessSpawner.Instance.boardMap.ContainsKey(pos);

    public bool IsEnemyAt(Vector2Int pos)
    {
        if (ChessSpawner.Instance.boardMap.TryGetValue(pos, out ChessPiece piece))
            return piece.IsWhite() != IsWhite();
        return false;
    }

    public RectTransform GetRectTransform() => rectTransform;
}
