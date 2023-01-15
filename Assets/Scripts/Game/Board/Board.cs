using System.Linq;
using UnityEngine;

namespace Game.Board
{
    public class Board
    {
        private readonly SquareType[,] squareTypes;

        public Board(int colCount, int rowCount)
        {
            squareTypes = new SquareType[colCount, rowCount];
        }

        public int ColCount => squareTypes.GetLength(0);
        public int RowCount => squareTypes.GetLength(1);
        
        public SquareType PlayerStoneType = SquareType.Black;
        public SquareType EnemyStoneType = SquareType.White;

        public int PlayerStoneCount => squareTypes.Cast<SquareType>().Count(type => type == PlayerStoneType);
        public int EnemyStoneCount => squareTypes.Cast<SquareType>().Count(type => type == EnemyStoneType);

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

        public bool IsEmpty(Vector2Int pos)
        {
            return GetSquareType(pos) == SquareType.Empty;
        }

        public void SetStone(SquareType stoneType, Vector2Int pos)
        {
            squareTypes[pos.x, pos.y] = stoneType;
        }
    }

    public enum SquareType
    {
        Empty,
        Black,
        White
    }
}
