using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Square;
using UnityEngine;

namespace Game.Board
{
    public class BoardView : MonoBehaviour
    {
        [SerializeField] private Transform squareParent;
        [SerializeField] private SquareView squarePrefab;

        private SquareView[,] squares;
        
        public Vector2Int? SelectedPos { get; private set; } = null;

        public async UniTask CreateBoard(Board board, CancellationToken token)
        {
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
                    square.Initialize(worldPos, () => SelectedPos = pos);
                    square.PutStone(board.GetSquareType(pos) ?? SquareType.Empty, token).Forget();
                    squares[col, row] = square;
                }

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

        public void ResetSelectedPos()
        {
            SelectedPos = null;
        }

        public async UniTask PutStone(SquareType stoneType, Vector2Int pos, CancellationToken token)
        {
            await squares[pos.x, pos.y].PutStone(stoneType, token);
        }

        public async UniTask ReverseStone(Vector2Int pos, CancellationToken token)
        {
            await squares[pos.x, pos.y].ReverseStone(token);
        }
    }
}