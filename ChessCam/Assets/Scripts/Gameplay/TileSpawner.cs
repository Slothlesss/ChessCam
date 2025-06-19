using UnityEngine;

public class TileSpawner : MonoBehaviour
{
    public GameObject tileWhitePrefab;   // Assign the Tile prefab in inspector
    public GameObject tileBlackPrefab;   // Assign the Tile prefab in inspector
    public GameObject parent;       // The parent UI object with a RectTransform (e.g., an empty GameObject under Canvas)

    private const int GridSize = 8;
    private const float CellSize = 100f;
    private const float StartX = -350f;
    private const float StartY = 350f;

    void Start()
    {
        SpawnTiles();
    }

    void SpawnTiles()
    {
        for (int row = 0; row < GridSize; row++)
        {
            for (int col = 0; col < GridSize; col++)
            {
                GameObject tile = (col + row) % 2 == 0
                    ? Instantiate(tileWhitePrefab, parent.transform)
                    : Instantiate(tileBlackPrefab, parent.transform);

                tile.GetComponent<ChessTile>().gridPos = new Vector2Int(col, row); // FIXED: use (col, row)

                RectTransform rect = tile.GetComponent<RectTransform>();
                float x = StartX + col * CellSize;
                float y = StartY - row * CellSize;
                rect.anchoredPosition = new Vector2(x, y);
            }
        }
    }

}
