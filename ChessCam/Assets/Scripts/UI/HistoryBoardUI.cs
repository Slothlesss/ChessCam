using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HistoryBoardUI : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI idUI;
    [SerializeField] private TextMeshProUGUI createAtUI;

    public void Initialize(List<Prediction> predictions, Sprite sprite, int id, string createAt, int width, int height)
    {
        image.sprite = sprite;
        button.onClick.AddListener(() => SpawnHistoryBoard(predictions, 100f, width, height));
        idUI.text = "ID: " + id.ToString();
        createAtUI.text = createAt;
    }

    private void SpawnHistoryBoard(List<Prediction> predictions, float cellSize, int width, int height)
    {
        ChessSpawner.Instance.ClearBoard();
        foreach (var pred in predictions)
        {
            ChessSpawner.Instance.SpawnPieceFromDetection(pred, cellSize, width, height);
        }
        GetComponentInParent<HistoryContainer>().gameObject.SetActive(false);
    }
}
