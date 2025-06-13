using System.Collections.Generic;
using UnityEngine;

public static class ChessRules
{
    public static List<Vector2Int> GetKnightMoves(Vector2Int pos)
    {
        Vector2Int[] offsets = {
            new Vector2Int(-2, -1), new Vector2Int(-2, 1),
            new Vector2Int(-1, -2), new Vector2Int(-1, 2),
            new Vector2Int(1, -2),  new Vector2Int(1, 2),
            new Vector2Int(2, -1),  new Vector2Int(2, 1),
        };

        return FilterInsideBoard(pos, offsets);
    }

    public static List<Vector2Int> GetKingMoves(Vector2Int pos)
    {
        Vector2Int[] offsets = {
            new Vector2Int(-1, -1) , new Vector2Int(-1, 0), new Vector2Int(-1, 1),
            new Vector2Int(0, -1)  ,                         new Vector2Int(0, 1),
            new Vector2Int(1, -1)  ,  new Vector2Int(1, 0),  new Vector2Int(1, 1),
        };

        return FilterInsideBoard(pos, offsets);
    }
    public static List<Vector2Int> GetRookMoves(Vector2Int pos)
    {
        return GetLinearMoves(pos, new Vector2Int[] {
            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
        });
    }

    public static List<Vector2Int> GetBishopMoves(Vector2Int pos)
    {
        return GetLinearMoves(pos, new Vector2Int[] {
            new Vector2Int(1, 1), new Vector2Int(1, -1),
            new Vector2Int(-1, 1), new Vector2Int(-1, -1)
        });
    }

    public static List<Vector2Int> GetQueenMoves(Vector2Int pos)
    {
        var allMoves = new List<Vector2Int>();
        allMoves.AddRange(GetRookMoves(pos));
        allMoves.AddRange(GetBishopMoves(pos));
        return allMoves;
    }

    public static List<Vector2Int> GetPawnMoves(Vector2Int pos, bool isWhite, System.Func<Vector2Int, bool> isOccupied, System.Func<Vector2Int, bool> isEnemy)
    {
        List<Vector2Int> moves = new List<Vector2Int>();
        int direction = isWhite ? 1 : -1;

        Vector2Int forward = new Vector2Int(pos.x, pos.y + direction);
        if (IsInsideBoard(forward) && !isOccupied(forward))
        {
            moves.Add(forward);

            // First double step
            Vector2Int doubleForward = new Vector2Int(pos.x, pos.y + 2 * direction);
            bool isAtInitialRow = isWhite ? pos.y == 1 : pos.y == 6;
            if (isAtInitialRow && !isOccupied(doubleForward))
            {
                moves.Add(doubleForward);
            }
        }

        // Diagonal captures
        Vector2Int[] diagonals = {
            new Vector2Int(pos.x - 1, pos.y + direction),
            new Vector2Int(pos.x + 1, pos.y + direction)
        };

        foreach (var diag in diagonals)
        {
            if (IsInsideBoard(diag) && isEnemy(diag))
            {
                moves.Add(diag);
            }
        }

        return moves;
    }


    private static bool IsInsideBoard(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < 8 && pos.y >= 0 && pos.y < 8;
    }

    // Helper to filter fixed-offset moves
    private static List<Vector2Int> FilterInsideBoard(Vector2Int pos, Vector2Int[] offsets)
    {
        List<Vector2Int> result = new List<Vector2Int>();
        foreach (var offset in offsets)
        {
            Vector2Int newPos = pos + offset;
            if (IsInsideBoard(newPos))
                result.Add(newPos);
        }
        return result;
    }

    private static List<Vector2Int> GetLinearMoves(Vector2Int pos, Vector2Int[] directions)
    {
        List<Vector2Int> result = new List<Vector2Int>();
        foreach (var dir in directions)
        {
            Vector2Int current = pos + dir;
            while (IsInsideBoard(current))
            {
                result.Add(current);
                current += dir;
            }
        }
        return result;
    }
}
