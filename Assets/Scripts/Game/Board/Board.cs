using System.Collections.Generic;
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

        public IEnumerable<StoneType> CastedStoneTypes => stoneTypes.Cast<StoneType>();

        public int PlayerStoneCount => CastedStoneTypes.Count(type => type == StoneType.Player);
        public int EnemyStoneCount => CastedStoneTypes.Count(type => type == StoneType.Enemy);

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

        public void SetStone(StoneType stoneType, Vector2Int pos)
        {
            stoneTypes[pos.x, pos.y] = stoneType;
        }

        public Board Clone()
        {
            var clone = new Board(ColCount, RowCount);
            for (var row = 0; row < RowCount; row++)
            {
                for (var col = 0; col < ColCount; col++)
                {
                    var pos = new Vector2Int(col, row);
                    var type = GetStoneType(pos) ?? StoneType.Empty;
                    clone.SetStone(type, pos);
                }
            }

            return clone;
        }
    }

    public enum StoneType
    {
        Empty,
        Player,
        Enemy
    }
}
