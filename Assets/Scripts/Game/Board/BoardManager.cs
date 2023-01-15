using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Cycle;
using UniRx;
using UnityEngine;

namespace Game.Board
{
    public class BoardManager : MonoBehaviour, ISubscribeGamePhase, ISubscribeTurnPhase, ICheckFinishGame
    {
        [SerializeField] private BoardView view;
        
        private Board board;

        private readonly Subject<Board> onBoardChangedSubject = new();
        public IObservable<Board> OnBoardChangedAsObservable => onBoardChangedSubject;

        private const int ROW_COUNT = 8;
        private const int COL_COUNT = 8;

        public UniTask OnGamePhaseChanged(GameCycle.GamePhase phase, CancellationToken token)
        {
            return phase switch
            {
                GameCycle.GamePhase.Initialize => Initialize(token),
                _ => UniTask.CompletedTask
            };
        }

        public UniTask OnTurnPhaseChanged(GameCycle.TurnPhase phase, SquareType stoneType, CancellationToken token)
        {
            return phase switch
            {
                GameCycle.TurnPhase.SelectSquare => WaitForSelectSquare(stoneType, token),
                GameCycle.TurnPhase.PutStone => PutStone(stoneType, token),
                GameCycle.TurnPhase.ReverseStones => ReverseStones(stoneType, token),
                _ => UniTask.CompletedTask
            };
        }

        private async UniTask Initialize(CancellationToken token)
        {
            board = new Board(COL_COUNT, ROW_COUNT);
            await view.CreateBoard(board, token);
            
            // 初期配置
            var firstStones = new Dictionary<Vector2Int, SquareType>
            {
                { new Vector2Int(COL_COUNT / 2 - 1, ROW_COUNT / 2 - 1), SquareType.White },
                { new Vector2Int(COL_COUNT / 2, ROW_COUNT / 2 - 1), SquareType.Black },
                { new Vector2Int(COL_COUNT / 2 - 1, ROW_COUNT / 2), SquareType.Black },
                { new Vector2Int(COL_COUNT / 2, ROW_COUNT / 2), SquareType.White }
            };
            foreach (var (pos, type) in firstStones)
            {
                board.SetStone(type, pos);
                await view.PutStone(type, pos, token);
                onBoardChangedSubject.OnNext(board);
            }
        }

        private async UniTask WaitForSelectSquare(SquareType stoneType, CancellationToken token)
        {
            var canPutPoses = board.GetCanPutPoses(stoneType);
            view.SetSquareHighlights(canPutPoses);

            view.ResetSelectedPos();
            await UniTask.WaitUntil(() => view.SelectedPos.HasValue, cancellationToken: token);
            view.SetSquareHighlights(new List<Vector2Int>());
        }

        private async UniTask PutStone(SquareType stoneType, CancellationToken token)
        {
            if (!view.SelectedPos.HasValue)
            {
                return;
            }
            
            var selectedPos = view.SelectedPos.Value;
            board.SetStone(stoneType, selectedPos);
            await view.PutStone(stoneType, selectedPos, token);
            onBoardChangedSubject.OnNext(board);
        }

        private async UniTask ReverseStones(SquareType stoneType, CancellationToken token)
        {
            if (!view.SelectedPos.HasValue)
            {
                return;
            }

            var selectedPos = view.SelectedPos.Value;
            var reversePoses = board.GetReversePoses(stoneType, selectedPos);
            
            var tasks = new List<UniTask>();
            var reverseAnimPosGroups = Enumerable.Range(0, reversePoses.Max(c => c.Count))
                .Select(i => reversePoses.Select(c => i < c.Count ? c[i] : (Vector2Int?) null));
            foreach (var animPosGroup in reverseAnimPosGroups)
            {
                foreach (var animPos in animPosGroup)
                {
                    if (!animPos.HasValue)
                    {
                        continue;
                    }

                    tasks.Add(ReverseStone(stoneType, animPos.Value, token));
                }

                await UniTask.Delay(100, cancellationToken: token);
            }

            await UniTask.WhenAll(tasks);
        }

        private async UniTask ReverseStone(SquareType stoneType, Vector2Int pos, CancellationToken token)
        {
            board.SetStone(stoneType, pos);
            await view.ReverseStone(pos, token);
            onBoardChangedSubject.OnNext(board);
        }

        public bool IsFinishedGame()
        {
            return board?.IsFinishedGame() ?? false;
        }
    }
}