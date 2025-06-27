using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChessTile : MonoBehaviour, IPointerClickHandler
{
    public Vector2Int gridPos;
    public GameObject highlight;
    public static ChessTile selectedTile;

    public void HighlightTile(bool isHighlighted)
    {
        if (selectedTile != null)
        {
            selectedTile.highlight.SetActive(false);
        }
        selectedTile = this;
        if (highlight != null)
        {
            highlight.SetActive(isHighlighted);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ChessPiece selected = ChessPiece.selectedPiece;
        if (selected == null) return;

        if (!selected.IsValidMove(selected.gridPos, gridPos))
        {
            ChessPiece.selectedPiece = null;
            GameManager.Instance.ClearMoveDots();
            return;
        }

        var board = ChessSpawner.Instance.boardMap;

        // Castling
        if (selected.pieceType.Contains("king") && gridPos.y == selected.gridPos.y && Mathf.Abs(gridPos.x - selected.gridPos.x) == 2)
        {
            int dir = gridPos.x - selected.gridPos.x;
            Vector2Int rookPos = dir < 0 ? new Vector2Int(0, gridPos.y) : new Vector2Int(7, gridPos.y);

            if (board.TryGetValue(rookPos, out ChessPiece rook))
            {
                board.Remove(rookPos);
                Vector2Int newRookPos = selected.gridPos + new Vector2Int(dir / 2, 0);
                board[newRookPos] = rook;
                rook.GetComponent<RectTransform>().anchoredPosition = ChessSpawner.Instance.GridToAnchoredPosition(newRookPos);
                rook.gridPos = newRookPos;
                rook.hasMoved = true;
            }
        }

        // Move selected piece
        board.Remove(selected.gridPos);
        board[gridPos] = selected;

        selected.GetRectTransform().anchoredPosition = ChessSpawner.Instance.GridToAnchoredPosition(gridPos);
        selected.gridPos = gridPos;
        selected.hasMoved = true;

        // Promotion
        if (selected.pieceType.Contains("pawn") && (gridPos.y == 0 || gridPos.y == 7))
        {
            PromotionUI.Instance.DisplayPromotionOptions(selected);
        }

        GameManager.Instance.HandleMove(selected.pieceType);
        ChessPiece.selectedPiece = null;
        GameManager.Instance.ClearMoveDots();
    }

}
