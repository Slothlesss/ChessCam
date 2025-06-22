using System.Collections.Generic;
using UnityEngine;

public static class ChessRules
{
    public static List<Vector2Int> GetKnightMoves(Vector2Int pos, System.Func<Vector2Int, bool> isOccupied, System.Func<Vector2Int, bool> isEnemy)
    {
        Vector2Int[] offsets = {
            new Vector2Int(-2, -1), new Vector2Int(-2, 1),
            new Vector2Int(-1, -2), new Vector2Int(-1, 2),
            new Vector2Int(1, -2),  new Vector2Int(1, 2),
            new Vector2Int(2, -1),  new Vector2Int(2, 1),
        };

        List<Vector2Int> validMoves = FilterInsideBoard(pos, offsets);

        List<Vector2Int> results = new List<Vector2Int>();
        foreach (var move in validMoves)
        {
            if (isOccupied(move))
            {
                if (isEnemy(move))
                {
                    results.Add(move);
                }
                continue;
            }
            results.Add(move);
        }

        return results;
    }

    public static List<Vector2Int> GetKingMoves(Vector2Int pos, System.Func<Vector2Int, bool> isOccupied, System.Func<Vector2Int, bool> isEnemy)
    {
        Vector2Int[] offsets = {
            new Vector2Int(-1, -1) , new Vector2Int(-1, 0), new Vector2Int(-1, 1),
            new Vector2Int(0, -1)  ,                         new Vector2Int(0, 1),
            new Vector2Int(1, -1)  ,  new Vector2Int(1, 0),  new Vector2Int(1, 1),
        };

        List<Vector2Int> validMoves = FilterInsideBoard(pos, offsets);

        List<Vector2Int> results = new List<Vector2Int>();

        foreach (var move in validMoves)
        {
            if (isOccupied(move))
            {
                if (isEnemy(move))
                {
                    results.Add(move);
                }
                continue;
            }
            results.Add(move);
        }

        return results;
    }
    public static List<Vector2Int> GetKingMovesWithCastling(Vector2Int pos, bool isWhite, bool hasMoved, System.Func<Vector2Int, bool> isOccupied, System.Func<Vector2Int, bool> isEnemy)
    {
        List<Vector2Int> moves = GetKingMoves(pos, isOccupied, isEnemy);

        if (hasMoved) return moves;

        var board = ChessSpawner.Instance.boardMap;
        bool flip = ChessSpawner.Instance.isBoardFlip;

        int y = pos.y; // king's current row

        // Loop over both potential rooks (0,y) and (7,y)
        foreach (int rookX in new int[] { 0, 7 })
        {
            if (!board.TryGetValue(new Vector2Int(rookX, y), out ChessPiece rook)) continue;
            if (!rook.pieceType.Contains("rook") || rook.hasMoved) continue;

            // Determine direction and range to check
            int dir = (rookX < pos.x) ? -1 : 1;

            bool pathClear = true;
            for (int x = pos.x + dir; x != rookX; x += dir)
            {
                if (isOccupied(new Vector2Int(x, y)))
                {
                    pathClear = false;
                    break;
                }
            }

            if (pathClear)
            {
                // Move king 2 steps toward rook
                moves.Add(new Vector2Int(pos.x + 2 * dir, y));
            }
        }

        return moves;
    }


    public static List<Vector2Int> GetRookMoves(Vector2Int pos, System.Func<Vector2Int, bool> isOccupied, System.Func<Vector2Int, bool> isEnemy)
    {
        return GetLinearMoves(
            pos, 
            new Vector2Int[] {Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right}, 
            isOccupied,
            isEnemy
        );
    }

    public static List<Vector2Int> GetBishopMoves(Vector2Int pos, System.Func<Vector2Int, bool> isOccupied, System.Func<Vector2Int, bool> isEnemy)
    {
        return GetLinearMoves(pos, new Vector2Int[] {
            new Vector2Int(1, 1), new Vector2Int(1, -1),
            new Vector2Int(-1, 1), new Vector2Int(-1, -1)
        }, isOccupied, isEnemy);
    }

    public static List<Vector2Int> GetQueenMoves(Vector2Int pos, System.Func<Vector2Int, bool> isOccupied, System.Func<Vector2Int, bool> isEnemy)
    {
        var allMoves = new List<Vector2Int>();
        allMoves.AddRange(GetRookMoves(pos, isOccupied, isEnemy));
        allMoves.AddRange(GetBishopMoves(pos, isOccupied, isEnemy));
        return allMoves;
    }

    public static List<Vector2Int> GetPawnMoves(Vector2Int pos, bool isWhite, System.Func<Vector2Int, bool> isOccupied, System.Func<Vector2Int, bool> isEnemy)
    {
        List<Vector2Int> moves = new List<Vector2Int>();
        int direction = isWhite ? (ChessSpawner.Instance.isBoardFlip ? 1 : -1)
                                : (ChessSpawner.Instance.isBoardFlip ? -1 : 1);

        Vector2Int forward = new Vector2Int(pos.x, pos.y + direction);
        if (IsInsideBoard(forward) && !isOccupied(forward))
        {
            moves.Add(forward);

            // First double step
            Vector2Int doubleForward = new Vector2Int(pos.x, pos.y + 2 * direction);
            bool isAtInitialRow = isWhite ? (ChessSpawner.Instance.isBoardFlip ? pos.y == 1 : pos.y == 6)
                                          : (ChessSpawner.Instance.isBoardFlip ? pos.y == 6 : pos.y == 1);
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

    private static List<Vector2Int> GetLinearMoves(Vector2Int pos, Vector2Int[] directions, System.Func<Vector2Int, bool> isOccupied, System.Func<Vector2Int, bool> isEnemy)
    {
        List<Vector2Int> result = new List<Vector2Int>();
        foreach (var dir in directions)
        {
            Vector2Int current = pos + dir;
            while (IsInsideBoard(current))
            {
                if (isOccupied(current))
                {
                    if (isEnemy(current))
                    {
                        result.Add(current);
                    }
                    break;
                }
                result.Add(current);
                current += dir;
            }
        }
        return result;
    }

}
