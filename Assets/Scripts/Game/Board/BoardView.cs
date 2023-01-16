using System;
using System.Collections.Generic;
using System.Threading;
using Common;
using Cysharp.Threading.Tasks;
using Game.Board.Square;
using UnityEngine;

namespace Game.Board
{
    public class BoardView : MonoBehaviour
    {
        [SerializeField] private Transform squareParent;
        [SerializeField] private SquareView squarePrefab;

        private SquareView[,] squares;
        
        public Action<Vector2Int> OnSelected { private get; set; }

        public async UniTask CreateBoard(Board board, CancellationToken token)
        {
            for (var i = squareParent.childCount - 1; i >= 0; i--)
            {
                Destroy(squareParent.GetChild(i).gameObject);
            }
            
            await UniTask.Delay(500, cancellationToken: token);
            
            var colCount = board.ColCount;
            var rowCount = board.RowCount;
            squares = new SquareView[colCount, rowCount];
            var offset = new Vector2(-colCount / 2f + 0.5f, -rowCount / 2f + 0.5f);

            for (var col = 0; col < colCount; col++)
            {
                for (var row = 0; row < rowCount; row++)
                {
                    var pos = new Vector2Int(col, row);
                    var worldPos = pos + offset;
                    var square = Instantiate(squarePrefab, squareParent);
                    square.Initialize(worldPos, () => OnSelected?.Invoke(pos));
                    squares[col, row] = square;
                }

                SEPlayer.I.Play(SEPlayer.SEName.CreateBoard);
                await UniTask.Delay(40, cancellationToken: token);
            }
        }

        public void SetSquareHighlights(List<Vector2Int> highlightPoses)
        {
            for (var col = 0; col < squares.GetLength(0); col++)
            {
                for (var row = 0; row < squares.GetLength(1); row++)
                {
                    var pos = new Vector2Int(col, row);
                    squares[col, row].SetHighlight(highlightPoses.Contains(pos));
                }
            }
        }

        public async UniTask PutStone(StoneType stoneType, Vector2Int pos, CancellationToken token)
        {
            await squares[pos.x, pos.y].PutStone(stoneType, token);
        }

        public async UniTask ReverseStone(Vector2Int pos, CancellationToken token)
        {
            await squares[pos.x, pos.y].ReverseStone(token);
        }
    }
}