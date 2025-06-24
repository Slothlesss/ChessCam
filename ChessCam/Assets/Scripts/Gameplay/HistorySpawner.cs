using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HistorySpawner : MonoBehaviour
{
    [SerializeField] private GameObject boardPrefab;
    [SerializeField] private Transform container;
    [SerializeField] private GameObject thumbnailImagePrefab; // UI Image prefab with Image component

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
            TileSpawner.Instance.SpawnTiles(tempBoard, cellSize);
            List<Prediction> preds = item.GetParsedPredictions();
            foreach (var pred in preds)
            {
                ChessSpawner.Instance.SpawnPiecesForThumbnail(pred, tempBoard.transform, cellSize, item.image_width, item.image_height);
            }

            yield return new WaitForEndOfFrame();

            yield return CaptureThumbnail(tempBoard);

            Destroy(tempBoard);
        }
        container.parent.gameObject.SetActive(true);
    }

    private IEnumerator CaptureThumbnail(GameObject board)
    {
        yield return new WaitForEndOfFrame();

        Texture2D tex = new Texture2D(320, 320, TextureFormat.RGB24, false);

        int x = Screen.width / 2 - 160;
        int y = Screen.height / 2 - 160;

        tex.ReadPixels(new Rect(x, y, 320, 320), 0, 0);
        tex.Apply();

        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));

        GameObject imgGO = Instantiate(thumbnailImagePrefab, container);
        imgGO.GetComponent<Image>().sprite = sprite;
    }
}
