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

        public static bool HasEmpty(this Board board)
        {
            return board.CastedStoneTypes.Any(type => type == StoneType.Empty);
        }

        private static bool IsEmpty(this Board board, Vector2Int pos)
        {
            return board.GetStoneType(pos) == StoneType.Empty;
        }

        public static List<Vector2Int> GetCanPutPoses(this Board board, StoneType stoneType)
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

        public static List<List<Vector2Int>> GetReversePoses(this Board board, StoneType stoneType, Vector2Int putPos)
        {
            return directions
                .Select(direction => board.GetReversePoses(stoneType, putPos, direction))
                .Where(poses => poses.Any())
                .ToList();
        }
        
        private static List<Vector2Int> GetReversePoses(this Board board, StoneType stoneType, Vector2Int putPos, Vector2Int direction)
        {
            if (stoneType == StoneType.Empty)
            {
                Debug.LogError("putType is Empty");
                return new List<Vector2Int>();
            }

            var reversePoses = new List<Vector2Int>();
            var targetPos = putPos;
            while (true)
            {
                targetPos += direction;
                var targetType = board.GetStoneType(targetPos);
                if (targetType is null or StoneType.Empty)
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