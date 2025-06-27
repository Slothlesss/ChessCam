using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HistorySpawner : MonoBehaviour
{
    [SerializeField] private GameObject boardPrefab;
    [SerializeField] private Transform container;
    [SerializeField] private HistoryBoardUI historyBoard; // UI Image prefab with Image component

    [SerializeField] private Transform renderHolder;
    private float cellSize = 40f;

    public void SpawnHistoryThumbnails()
    {
        StartCoroutine(GenerateThumbnails());
    }

    private IEnumerator GenerateThumbnails()
    {
        yield return InferenceService.Instance.GetInferenceHistory();

        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }

        List<InferenceHistoryItem> historyItems = InferenceService.Instance.historyResult.history;

        foreach (var item in historyItems)
        {
            GameObject tempBoard = Instantiate(boardPrefab, renderHolder);
            TileSpawner.Instance.SpawnTilesForThumbnails(tempBoard, cellSize);
            List<Prediction> preds = item.GetParsedPredictions();
            foreach (var pred in preds)
            {
                ChessSpawner.Instance.SpawnPiecesForThumbnail(pred, tempBoard.transform, cellSize, item.image_width, item.image_height);
            }

            yield return new WaitForEndOfFrame();

            yield return CaptureThumbnail(preds, tempBoard, item.id, item.created_at, item.image_width, item.image_height);

            Destroy(tempBoard);
        }
        container.parent.gameObject.SetActive(true);
    }

    private IEnumerator CaptureThumbnail(List<Prediction> predictions, GameObject board, int id, string createAt, int width, int height)
    {
        yield return new WaitForEndOfFrame();

        // Capture Thumbnail
        Vector3[] corners = new Vector3[4];
        board.GetComponent<RectTransform>().GetWorldCorners(corners);

        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, corners[0]);// Bottom left corner

        int x = Mathf.RoundToInt(screenPos.x);
        int y = Mathf.RoundToInt(screenPos.y);

        int boardWidth = Mathf.RoundToInt(corners[2].x - corners[0].x);
        int boardHeight = Mathf.RoundToInt(corners[2].y - corners[0].y);

        Texture2D tex = new Texture2D(boardWidth, boardHeight, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(x, y, boardWidth, boardHeight), 0, 0);
        tex.Apply();

        // Spawn History Board
        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        HistoryBoardUI imgGO = Instantiate(historyBoard.gameObject, container).GetComponent<HistoryBoardUI>();
        imgGO.Initialize(predictions, sprite, id, createAt, width, height);
    }
}
