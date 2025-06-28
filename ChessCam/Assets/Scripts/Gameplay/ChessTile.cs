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
        GameManager.Instance.OnTileClicked(this);
    }

}
