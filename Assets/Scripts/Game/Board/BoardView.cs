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

        public async UniTask CreateBoard(Board board, CancellationToken token)
        {
            squares = new SquareView[board.ColCount, board.RowCount];

            for (var col = 0; col < board.ColCount; col++)
            {
                for (var row = 0; row < board.RowCount; row++)
                {
                    var pos = new Vector2Int(col, row);
                    var square = Instantiate(squarePrefab, squareParent);
                    square.Initialize(pos);
                    squares[col, row] = square;
                }

                await UniTask.Delay(40, cancellationToken: token);
            }
        }
    }
}