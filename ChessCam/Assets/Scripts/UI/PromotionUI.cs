using System.Drawing;
using UnityEngine;
using UnityEngine.UI;

public class PromotionUI : Singleton<PromotionUI>
{
    public GameObject panel;
    private ChessPiece pendingPawn;
    private Button[] promotionOptions;

    void Awake()
    {
        promotionOptions = panel.GetComponentsInChildren<Button>();
        promotionOptions[0].onClick.AddListener(() => PromoteTo("queen"));
        promotionOptions[1].onClick.AddListener(() => PromoteTo("rook"));
        promotionOptions[2].onClick.AddListener(() => PromoteTo("knight"));
        promotionOptions[3].onClick.AddListener(() => PromoteTo("bishop"));

        panel.SetActive(false);
    }

    public void DisplayPromotionOptions(ChessPiece pawn)
    {
        pendingPawn = pawn;
        string color = pawn.pieceType.Split('-')[0];
        Vector2 pivot = pawn.gridPos.y == 0? new Vector2(0, -100) : new Vector2(0, 100);
        panel.GetComponent<RectTransform>().anchoredPosition = pawn.GetRectTransform().anchoredPosition + pivot;
        promotionOptions[0].GetComponent<Image>().sprite = ChessSpawner.Instance.GetPieceSpriteMap()[color + "-queen"];
        promotionOptions[1].GetComponent<Image>().sprite = ChessSpawner.Instance.GetPieceSpriteMap()[color + "-rook"];
        promotionOptions[2].GetComponent<Image>().sprite = ChessSpawner.Instance.GetPieceSpriteMap()[color + "-knight"];
        promotionOptions[3].GetComponent<Image>().sprite = ChessSpawner.Instance.GetPieceSpriteMap()[color + "-bishop"];

        panel.SetActive(true);
    }

    public void PromoteTo(string type)
    {
        panel.SetActive(false);
        if (pendingPawn != null)
        {
            string color = pendingPawn.pieceType.Split('-')[0];
            pendingPawn.GetComponent<Image>().sprite = ChessSpawner.Instance.GetPieceSpriteMap()[color + $"-{type}"];
            pendingPawn.pieceType = color + "-" + type;
            pendingPawn = null;
        }
    }
}
