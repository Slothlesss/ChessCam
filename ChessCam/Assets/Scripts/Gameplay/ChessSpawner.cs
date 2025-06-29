using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class ChessSpawner : Singleton<ChessSpawner>
{
    [Header("Prefabs & Parents")]
    public GameObject piecePrefab; // Should have RectTransform + Image
    public Transform pieceParent;      // UI Panel (RectTransform)

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
    public Dictionary<Vector2Int, ChessPiece> copiedBoardMap = new Dictionary<Vector2Int, ChessPiece>();

    [Header("Board State")]
    public bool isBoardFlip = false;

    // Constants
    private const int gridSize = 8;
    private const float cellSize = 100;

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

    public void SpawnPieceFromDetection(Prediction prediction, float cellSize, int width, int height)
    {
        // Convert pixel position to grid coordinate
        Vector2Int cell = DetectionToGridCell(prediction.x, prediction.y, width, height);

        // Convert to UI anchored position (local coordinates relative to parent)
        Vector2 anchoredPos = GridToAnchoredPosition(cell.x, cell.y, cellSize);

        if (pieceSpriteMap.TryGetValue(prediction.name, out Sprite sprite))
        {
            GameObject piece = Instantiate(piecePrefab, pieceParent);

            var chessPiece = piece.GetComponent<ChessPiece>();
            chessPiece.pieceType = prediction.name;
            chessPiece.gridPos = cell;

            RectTransform rect = piece.GetComponent<RectTransform>();
            rect.anchoredPosition = anchoredPos;
            rect.sizeDelta = new Vector2(cellSize, cellSize) * 0.8f;

            piece.GetComponent<Image>().sprite = sprite;

            boardMap[cell] = chessPiece;
        }
        else
        {
            Debug.LogWarning($"No sprite assigned for class: {prediction.name}");
        }
        copiedBoardMap = new Dictionary<Vector2Int, ChessPiece>(boardMap);
    }

    public void SpawnPiecesForThumbnail(Prediction prediction, Transform parent, float cellSize, int width, int height)
    {
        // Convert pixel position to grid coordinate
        Vector2Int cell = DetectionToGridCell(prediction.x, prediction.y, width, height);

        // Convert to UI anchored position (local coordinates relative to parent)
        Vector2 anchoredPos = GridToAnchoredPosition(cell.x, cell.y, cellSize);

        if (pieceSpriteMap.TryGetValue(prediction.name, out Sprite sprite))
        {
            GameObject piece = Instantiate(piecePrefab, parent);

            var chessPiece = piece.GetComponent<ChessPiece>();
            chessPiece.gridPos = cell;

            RectTransform rect = piece.GetComponent<RectTransform>();
            rect.anchoredPosition = anchoredPos;
            rect.sizeDelta = new Vector2(cellSize, cellSize) * 0.8f;

            piece.GetComponent<Image>().sprite = sprite;
        }
        else
        {
            Debug.LogWarning($"No sprite assigned for class: {prediction.name}");
        }
    }

    public void SpawnAll()
    {
        imageWidth = InferenceService.Instance.GetImageWidth();
        imageHeight = InferenceService.Instance.GetImageHeight();

        foreach (var pred in InferenceService.Instance.inferenceResult.predictions)
        {
            SpawnPieceFromDetection(pred, cellSize, imageWidth, imageHeight);
        }
    }

    public void ClearBoard()
    {
        foreach (Transform child in pieceParent)
        {
            Destroy(child.gameObject);
        }
        boardMap.Clear();
    }

    public Dictionary<string, Sprite> GetPieceSpriteMap()
    {
        return pieceSpriteMap;
    }


    /// <summary>
    /// Converts prediction (x,y) to 8x8 grid coordinates (0,0) top-left.
    /// </summary>
    private Vector2Int DetectionToGridCell(float x, float y, int width, int height)
    {
        int col = Mathf.FloorToInt(x / (width / gridSize));
        int row = Mathf.FloorToInt(y / (height / gridSize)); // Flip Y axis
        return new Vector2Int(col, row);
    }


    /// <summary>
    /// Converts 8x8 grid coordinate to UI anchored position relative to parent RectTransform.
    /// </summary>
    private Vector2 GridToAnchoredPosition(int col, int row, float cellSize)
    {
        float posX = -cellSize * 3.5f + col * cellSize;
        float posY = cellSize * 3.5f - row * cellSize;
        return new Vector2(posX, posY);
    }


    /// <summary>
    /// Converts 8x8 grid coordinate to UI anchored position relative to parent RectTransform.
    /// </summary>
    public Vector2 GridToAnchoredPosition(Vector2Int pos, float cellSize = 100f)
    {
        float posX = -cellSize * 3.5f + pos.x * cellSize;
        float posY = cellSize * 3.5f - pos.y * cellSize;
        return new Vector2(posX, posY);
    }

    public void SetBoardFlip(bool isFlip)
    {
        isBoardFlip = isFlip;
    }
}
