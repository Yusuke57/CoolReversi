using System.Collections.Generic;
using Game.Cycle;
using UnityEngine;

namespace Game.Board
{
    public class Board : ICheckFinishGame
    {
        private readonly SquareType[,] squareTypes;

        public Board(int colCount, int rowCount)
        {
            squareTypes = new SquareType[colCount, rowCount];

            var firstStones = new Dictionary<Vector2Int, SquareType>
            {
                { new Vector2Int(colCount / 2 - 1, rowCount / 2 - 1), SquareType.White },
                { new Vector2Int(colCount / 2, rowCount / 2 - 1), SquareType.Black },
                { new Vector2Int(colCount / 2 - 1, rowCount / 2), SquareType.Black },
                { new Vector2Int(colCount / 2, rowCount / 2), SquareType.White }
            };
            foreach (var (pos, type) in firstStones)
            {
                squareTypes[pos.x, pos.y] = type;
            }
        }

        public int ColCount => squareTypes.GetLength(0);
        public int RowCount => squareTypes.GetLength(1);

        public SquareType? GetSquareType(Vector2Int pos)
        {
            if (pos.x < 0 || pos.x >= squareTypes.GetLength(0))
            {
                return null;
            }
            if (pos.y < 0 || pos.y >= squareTypes.GetLength(1))
            {
                return null;
            }

            return squareTypes[pos.x, pos.y];
        }

        public bool IsFinishedGame()
        {
            var hasEmpty = false;
            foreach (var squareType in squareTypes)
            {
                hasEmpty |= squareType == SquareType.Empty;
            }

            // TODO: 両者石を置けなくなったときもfinish判定する
            return !hasEmpty;
        }
    }

    public enum SquareType
    {
        Empty,
        Black,
        White
    }
}
