using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChessSpawner : Singleton<ChessSpawner>
{
    [Header("Prefabs & Parents")]
    public GameObject piecePrefab; // Should have RectTransform + Image
    public GameObject parent;      // UI Panel (RectTransform)

    [Header("Piece Sprites")]
    public Sprite whiteKingPrefab;
    public Sprite whiteQueenPrefab;
    public Sprite whiteBishopPrefab;
    public Sprite whiteKnightPrefab;
    public Sprite whiteRookPrefab;
    public Sprite whitePawnPrefab;
    public Sprite blackKingPrefab;
    public Sprite blackQueenPrefab;
    public Sprite blackBishopPrefab;
    public Sprite blackKnightPrefab;
    public Sprite blackRookPrefab;
    public Sprite blackPawnPrefab;

    [Header("Image Dimensions")]
    public int imageWidth = 640;
    public int imageHeight = 640;


    [Header("Current Pieces")] 
    public Dictionary<Vector2Int, ChessPiece> boardMap = new Dictionary<Vector2Int, ChessPiece>();


    // Constants
    private const int GridSize = 8;
    private const float CellSize = 100f;
    private const float BoardUIOriginX = -350f; // Top-left corner X in anchored space
    private const float BoardUIOriginY = 350f;  // Top-left corner Y in anchored space

    private Dictionary<string, Sprite> pieceSpriteMap;

    void Awake()
    {
        pieceSpriteMap = new Dictionary<string, Sprite>()
        {
            { "white-king", whiteKingPrefab },
            { "white-queen", whiteQueenPrefab },
            { "white-bishop", whiteBishopPrefab },
            { "white-knight", whiteKnightPrefab },
            { "white-rook", whiteRookPrefab },
            { "white-pawn", whitePawnPrefab },
            { "black-king", blackKingPrefab },
            { "black-queen", blackQueenPrefab },
            { "black-bishop", blackBishopPrefab },
            { "black-knight", blackKnightPrefab },
            { "black-rook", blackRookPrefab },
            { "black-pawn", blackPawnPrefab },
        };
    }

    public void SpawnPieceFromDetection(Prediction prediction)
    {
        // Convert pixel position to grid coordinate
        Vector2Int cell = DetectionToGridCell(prediction.x, prediction.y);

        // Convert to UI anchored position (local coordinates relative to parent)
        Vector2 anchoredPos = GridToAnchoredPosition(cell.x, cell.y);

        Debug.Log($"Spawn: {prediction.@class} at cell {cell}, anchoredPos {anchoredPos}");

        if (pieceSpriteMap.TryGetValue(prediction.@class, out Sprite sprite))
        {
            GameObject piece = Instantiate(piecePrefab, parent.transform);

            var chessPiece = piece.GetComponent<ChessPiece>();
            chessPiece.pieceType = prediction.@class;
            chessPiece.gridPos = cell;

            var rt = piece.GetComponent<RectTransform>();
            rt.anchoredPosition = anchoredPos;
            piece.GetComponent<Image>().sprite = sprite;

            boardMap[cell] = chessPiece;
        }
        else
        {
            Debug.LogWarning($"No sprite assigned for class: {prediction.@class}");
        }
    }

    public void SpawnAll()
    {
        foreach (var pred in RoboflowUploader.Instance.result.predictions)
        {
            SpawnPieceFromDetection(pred);
        }
    }

    public void ClearBoard()
    {
        foreach (Transform child in parent.transform)
        {
            Destroy(child.gameObject);
        }
        boardMap.Clear();
    }


    /// <summary>
    /// Converts Roboflow's prediction (x,y in pixel space) to 8x8 grid coordinates (0,0) top-left.
    /// </summary>
    private Vector2Int DetectionToGridCell(float x, float y)
    {
        int col = Mathf.FloorToInt(x / (imageWidth / GridSize));
        int row = Mathf.FloorToInt(y / (imageHeight / GridSize)); // Flip Y axis
        return new Vector2Int(col, row);
    }


    /// <summary>
    /// Converts 8x8 grid coordinate to UI anchored position relative to parent RectTransform.
    /// </summary>
    private Vector2 GridToAnchoredPosition(int col, int row)
    {
        float posX = BoardUIOriginX + col * CellSize;
        float posY = BoardUIOriginY - row * CellSize;
        return new Vector2(posX, posY);
    }


    public Dictionary<string, Sprite> GetPieceSpriteMap()
    {
        return pieceSpriteMap;
    }
}
