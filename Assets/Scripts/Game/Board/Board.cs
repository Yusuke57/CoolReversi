using System.Linq;
using UnityEngine;

namespace Game.Board
{
    public class Board
    {
        private readonly StoneType[,] stoneTypes;

        public Board(int colCount, int rowCount)
        {
            stoneTypes = new StoneType[colCount, rowCount];
        }

        public int ColCount => stoneTypes.GetLength(0);
        public int RowCount => stoneTypes.GetLength(1);

        public int PlayerStoneCount => stoneTypes.Cast<StoneType>().Count(type => type == StoneType.Player);
        public int EnemyStoneCount => stoneTypes.Cast<StoneType>().Count(type => type == StoneType.Enemy);

        public StoneType? GetStoneType(Vector2Int pos)
        {
            if (pos.x < 0 || pos.x >= stoneTypes.GetLength(0))
            {
                return null;
            }
            if (pos.y < 0 || pos.y >= stoneTypes.GetLength(1))
            {
                return null;
            }

            return stoneTypes[pos.x, pos.y];
        }

        public bool IsEmpty(Vector2Int pos)
        {
            return GetStoneType(pos) == StoneType.Empty;
        }

        public void SetStone(StoneType stoneType, Vector2Int pos)
        {
            stoneTypes[pos.x, pos.y] = stoneType;
        }
    }

    public enum StoneType
    {
        Empty,
        Player,
        Enemy
    }
}
