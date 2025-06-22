using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PlayerTurn
{
    White,
    Black
}

public class GameManager : Singleton<GameManager>
{
    public PlayerTurn currentTurn;
    public Toggle[] turnToggles;


    public GameObject moveDotPrefab;
    private List<GameObject> moveDots = new List<GameObject>();
    private void Start()
    {
        currentTurn = PlayerTurn.White;
        turnToggles[0].onValueChanged.AddListener((isOn) => { if (isOn) SetWhiteTurn(); });
        turnToggles[1].onValueChanged.AddListener((isOn) => { if (isOn) SetBlackTurn(); });

        UpdateToggleUI();
    }
    private void UpdateToggleUI()
    {
        if (currentTurn == PlayerTurn.White)
        {
            turnToggles[0].isOn = true;
        }
        else
        {
            turnToggles[1].isOn = true;
        }
    }

    public void HandleMove(string pieceType)
    {
        currentTurn = currentTurn == PlayerTurn.White ? PlayerTurn.Black : PlayerTurn.White;
        UpdateToggleUI();
    }

    public bool IsTurnFor(string pieceType)
    {
        return (currentTurn == PlayerTurn.White && pieceType.StartsWith("white")) ||
               (currentTurn == PlayerTurn.Black && pieceType.StartsWith("black"));
    }

    public void SetWhiteTurn()
    {
        currentTurn = PlayerTurn.White;
        UpdateToggleUI();
    }

    public void SetBlackTurn()
    {
        currentTurn = PlayerTurn.Black;
        UpdateToggleUI();
    }

    public void ShowValidMoves(ChessPiece piece)
    {
        var validMoves = piece.GetValidMoves();
        foreach (var move in validMoves)
        {
            Vector2 anchoredPos = ChessSpawner.Instance.GridToAnchoredPosition(move);
            GameObject dot = Instantiate(moveDotPrefab, ChessSpawner.Instance.parent.transform);
            dot.GetComponent<RectTransform>().anchoredPosition = anchoredPos;
            moveDots.Add(dot);
        }
    }

    public void ClearMoveDots()
    {
        foreach (var dot in moveDots)
        {
            Destroy(dot);
        }
        moveDots.Clear();
    }
}


