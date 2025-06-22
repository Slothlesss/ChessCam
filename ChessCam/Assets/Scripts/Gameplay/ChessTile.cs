using UnityEngine;
using UnityEngine.EventSystems;

public class ChessTile : MonoBehaviour, IPointerClickHandler
{
    public Vector2Int gridPos;

    public void OnPointerClick(PointerEventData eventData)
    {
        ChessPiece selected = ChessPiece.selectedPiece;
        if (selected == null) return;

        if (!GameManager.Instance.IsTurnFor(selected.pieceType))
        {
            PlayerTurn notTurn = GameManager.Instance.currentTurn == PlayerTurn.White? PlayerTurn.Black : PlayerTurn.White;
            NotificationUI.Instance.ShowMessage($"Not {notTurn} turn", true);
            return;
        }

        if (!selected.IsValidMove(selected.gridPos, gridPos))
        {
            ChessPiece.selectedPiece = null;
            return;
        }

        var board = ChessSpawner.Instance.boardMap;

        // Capture
        if (board.TryGetValue(gridPos, out ChessPiece targetPiece))
        {
            if (selected.IsEnemyAt(gridPos, selected.IsWhite()))
            {
                Destroy(targetPiece.gameObject);
                board.Remove(gridPos);
            }
            else
            {
                NotificationUI.Instance.ShowMessage("Tile occupied by ally", true);
                ChessPiece.selectedPiece = null;
                return;
            }
        }

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
