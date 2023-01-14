using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Board
{
    public static class BoardExtensions
    {
        private static readonly List<Vector2Int> directions = new()
        {
            new Vector2Int(1, 0),
            new Vector2Int(1, 1),
            new Vector2Int(0, 1),
            new Vector2Int(-1, 1),
            new Vector2Int(-1, 0),
            new Vector2Int(-1, -1),
            new Vector2Int(0, -1),
            new Vector2Int(1, -1)
        };

        public static bool IsFinishedGame(this Board board)
        {
            var hasEmpty = false;
            for (var row = 0; row < board.RowCount; row++)
            {
                for (var col = 0; col < board.ColCount; col++)
                {
                    var type = board.GetSquareType(new Vector2Int(col, row));
                    if (type == SquareType.Empty)
                    {
                        hasEmpty = true;
                        break;
                    }
                }
            }

            // TODO: 両者石を置けなくなったときもfinish判定する
            return !hasEmpty;
        }

        public static List<Vector2Int> GetCanPutPoses(this Board board, SquareType stoneType)
        {
            var canPutPoses = new List<Vector2Int>();
            for (var row = 0; row < board.RowCount; row++)
            {
                for (var col = 0; col < board.ColCount; col++)
                {
                    var pos = new Vector2Int(col, row);
                    var canPut = board.IsEmpty(pos) && board.GetReversePoses(stoneType, pos).Any();
                    if (canPut)
                    {
                        canPutPoses.Add(pos);
                    }
                }
            }

            return canPutPoses;
        }

        public static List<List<Vector2Int>> GetReversePoses(this Board board, SquareType stoneType, Vector2Int putPos)
        {
            return directions
                .Select(direction => board.GetReversePoses(stoneType, putPos, direction))
                .Where(poses => poses.Any())
                .ToList();
        }
        
        private static List<Vector2Int> GetReversePoses(this Board board, SquareType stoneType, Vector2Int putPos, Vector2Int direction)
        {
            if (stoneType == SquareType.Empty)
            {
                Debug.LogError("putType is Empty");
                return new List<Vector2Int>();
            }

            var reversePoses = new List<Vector2Int>();
            var targetPos = putPos;
            while (true)
            {
                targetPos += direction;
                var targetType = board.GetSquareType(targetPos);
                if (targetType is null or SquareType.Empty)
                {
                    reversePoses.Clear();
                    break;
                }

                if (targetType == stoneType)
                {
                    break;
                }

                reversePoses.Add(targetPos);
            }

            return reversePoses;
        }
    }
}