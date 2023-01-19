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

        private CellView[,] cellViews;
        
        public Action<Vector2Int> OnSelected { private get; set; }

        public async UniTask CreateBoard(Board board, CancellationToken token)
        {
            var colCount = board.ColCount;
            var rowCount = board.RowCount;
            ResetView(colCount, rowCount);

            await UniTask.Delay(500, cancellationToken: token);

            var offset = new Vector2(-colCount / 2f + 0.5f, -rowCount / 2f + 0.5f);
            var defaultQuaternion = Quaternion.Euler(Vector3.zero);
            for (var col = 0; col < colCount; col++)
            {
                for (var row = 0; row < rowCount; row++)
                {
                    var pos = new Vector2Int(col, row);
                    var worldPos = pos + offset;
                    var cellView = cellViews[col, row];
                    if (cellView == null)
                    {
                        cellView = Instantiate(cellPrefab, worldPos, defaultQuaternion, cellParent);
                        cellView.OnClickAction = () => OnSelected?.Invoke(pos);
                        cellViews[col, row] = cellView;
                    }
                    cellView.ResetView();
                    cellView.gameObject.SetActive(true);
                }

                SEPlayer.I.Play(SEPlayer.SEName.CreateBoard);
                await UniTask.Delay(40, cancellationToken: token);
            }
        }

        private void ResetView(int colCount, int rowCount)
        {
            // col/rowが変わるなら要対応。面倒なので対応してない
            if (cellViews == null)
            {
                cellViews = new CellView[colCount, rowCount];
            }
            else
            {
                foreach (var cellView in cellViews)
                {
                    cellView.gameObject.SetActive(false);
                }
            }
        }

        public void SetCellHighlights(List<Vector2Int> highlightPoses)
        {
            for (var col = 0; col < cellViews.GetLength(0); col++)
            {
                for (var row = 0; row < cellViews.GetLength(1); row++)
                {
                    var pos = new Vector2Int(col, row);
                    cellViews[col, row].SetHighlight(highlightPoses.Contains(pos));
                }
            }
        }

        public async UniTask PutStone(StoneType stoneType, Vector2Int pos, CancellationToken token)
        {
            await cellViews[pos.x, pos.y].PutStone(stoneType, token);
        }

        public async UniTask ReverseStone(Vector2Int pos, CancellationToken token)
        {
            await cellViews[pos.x, pos.y].ReverseStone(token);
        }
    }
}