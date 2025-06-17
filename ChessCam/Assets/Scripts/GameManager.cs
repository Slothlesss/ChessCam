using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerTurn
{
    White,
    Black
}

public class GameManager : Singleton<GameManager>
{
    public PlayerTurn currentTurn = PlayerTurn.White;

    public void HandleMove(string pieceType)
    {
        // Optional: Validate legality again
        currentTurn = currentTurn == PlayerTurn.White ? PlayerTurn.Black : PlayerTurn.White;
        Debug.Log("Next turn: " + currentTurn);
    }

    public bool IsTurnFor(string pieceType)
    {
        return (currentTurn == PlayerTurn.White && pieceType.StartsWith("white")) ||
               (currentTurn == PlayerTurn.Black && pieceType.StartsWith("black"));
    }
}

