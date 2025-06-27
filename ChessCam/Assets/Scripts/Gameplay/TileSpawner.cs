using System.Collections.Generic;
using UnityEngine;

public class TileSpawner : Singleton<TileSpawner>
{
    public GameObject tileWhitePrefab;   // Assign the Tile prefab in inspector
    public GameObject tileBlackPrefab;   // Assign the Tile prefab in inspector
    public GameObject tileParent;       // The parent UI object with a RectTransform (e.g., an empty GameObject under Canvas)

    private const int GridSize = 8;

    public Dictionary<Vector2Int, ChessTile> tileMap = new Dictionary<Vector2Int, ChessTile>();

    void Start()
    {
        SpawnTiles(tileParent, 100f);
    }

    public void SpawnTiles(GameObject parent, float cellSize)
    {
        for (int row = 0; row < GridSize; row++)
        {
            for (int col = 0; col < GridSize; col++)
            {
                GameObject tile = (col + row) % 2 == 0
                    ? Instantiate(tileWhitePrefab, parent.transform)
                    : Instantiate(tileBlackPrefab, parent.transform);

                ChessTile chessTile = tile.GetComponent<ChessTile>();
                var cell = chessTile.gridPos = new Vector2Int(col, row);

                RectTransform rect = tile.GetComponent<RectTransform>();
                float startX = -cellSize * 3.5f;
                float startY = cellSize * 3.5f;

                float x = startX + col * cellSize;
                float y = startY - row * cellSize;
                rect.anchoredPosition = new Vector2(x, y);
                rect.sizeDelta = new Vector2(cellSize, cellSize);

                tileMap[cell] = chessTile;
            }
        }
    }

    public void SpawnTilesForThumbnails(GameObject parent, float cellSize)
    {
        for (int row = 0; row < GridSize; row++)
        {
            for (int col = 0; col < GridSize; col++)
            {
                GameObject tile = (col + row) % 2 == 0
                    ? Instantiate(tileWhitePrefab, parent.transform)
                    : Instantiate(tileBlackPrefab, parent.transform);

                ChessTile chessTile = tile.GetComponent<ChessTile>();
                var cell = chessTile.gridPos = new Vector2Int(col, row);

                RectTransform rect = tile.GetComponent<RectTransform>();
                float startX = -cellSize * 3.5f;
                float startY = cellSize * 3.5f;

                float x = startX + col * cellSize;
                float y = startY - row * cellSize;
                rect.anchoredPosition = new Vector2(x, y);
                rect.sizeDelta = new Vector2(cellSize, cellSize);
            }
        }
    }

}
