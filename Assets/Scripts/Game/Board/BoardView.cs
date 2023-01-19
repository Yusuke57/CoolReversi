using System;
using System.Collections.Generic;
using System.Threading;
using Common;
using Cysharp.Threading.Tasks;
using Game.Board.Cell;
using UnityEngine;

namespace Game.Board
{
    public class BoardView : MonoBehaviour
    {
        [SerializeField] private Transform cellParent;
        [SerializeField] private CellView cellPrefab;

        private CellView[,] cells;
        
        public Action<Vector2Int> OnSelected { private get; set; }

        public async UniTask CreateBoard(Board board, CancellationToken token)
        {
            for (var i = cellParent.childCount - 1; i >= 0; i--)
            {
                Destroy(cellParent.GetChild(i).gameObject);
            }
            
            await UniTask.Delay(500, cancellationToken: token);
            
            var colCount = board.ColCount;
            var rowCount = board.RowCount;
            cells = new CellView[colCount, rowCount];
            var offset = new Vector2(-colCount / 2f + 0.5f, -rowCount / 2f + 0.5f);

            for (var col = 0; col < colCount; col++)
            {
                for (var row = 0; row < rowCount; row++)
                {
                    var pos = new Vector2Int(col, row);
                    var worldPos = pos + offset;
                    var cell = Instantiate(cellPrefab, cellParent);
                    cell.Initialize(worldPos, () => OnSelected?.Invoke(pos));
                    cells[col, row] = cell;
                }

                SEPlayer.I.Play(SEPlayer.SEName.CreateBoard);
                await UniTask.Delay(40, cancellationToken: token);
            }
        }

        public void SetCellHighlights(List<Vector2Int> highlightPoses)
        {
            for (var col = 0; col < cells.GetLength(0); col++)
            {
                for (var row = 0; row < cells.GetLength(1); row++)
                {
                    var pos = new Vector2Int(col, row);
                    cells[col, row].SetHighlight(highlightPoses.Contains(pos));
                }
            }
        }

        public async UniTask PutStone(StoneType stoneType, Vector2Int pos, CancellationToken token)
        {
            await cells[pos.x, pos.y].PutStone(stoneType, token);
        }

        public async UniTask ReverseStone(Vector2Int pos, CancellationToken token)
        {
            await cells[pos.x, pos.y].ReverseStone(token);
        }
    }
}