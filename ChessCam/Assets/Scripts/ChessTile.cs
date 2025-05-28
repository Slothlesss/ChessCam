using UnityEngine;
using UnityEngine.EventSystems;

public class ChessTile : MonoBehaviour, IPointerClickHandler
{
    public Vector2Int gridPos; // must be set when spawning tiles

    public void OnPointerClick(PointerEventData eventData)
    {
        if (ChessPiece.selectedPiece != null)
        {
            Vector2Int targetGrid = gridPos;

            if (ChessPiece.selectedPiece.IsValidMove(ChessPiece.selectedPiece.gridPos, targetGrid))
            {
                RectTransform pieceTransform = ChessPiece.selectedPiece.GetComponent<RectTransform>();
                pieceTransform.anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
                ChessPiece.selectedPiece.gridPos = targetGrid;
            }

            ChessPiece.selectedPiece = null;
        }
    }

}

