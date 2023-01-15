using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Board.Enemy;
using Game.Cycle;
using UniRx;
using UnityEngine;

namespace Game.Board
{
    public class BoardManager : MonoBehaviour, ISubscribeGamePhase, ISubscribeTurnPhase, ICheckFinishGame
    {
        [SerializeField] private BoardView view;
        
        private Board board;
        private Vector2Int? selectedPos;

        private readonly Subject<Board> onBoardChangedSubject = new();
        public IObservable<Board> OnBoardChangedAsObservable => onBoardChangedSubject;

        private const int ROW_COUNT = 8;
        private const int COL_COUNT = 8;

        private void Awake()
        {
            view.OnSelected = pos => selectedPos = pos;
        }

        public UniTask OnGamePhaseChanged(GameCycle.GamePhase phase, CancellationToken token)
        {
            return phase switch
            {
                GameCycle.GamePhase.Initialize => Initialize(token),
                _ => UniTask.CompletedTask
            };
        }

        public UniTask OnTurnPhaseChanged(GameCycle.TurnPhase phase, StoneType stoneType, CancellationToken token)
        {
            return phase switch
            {
                GameCycle.TurnPhase.SelectSquare => SelectSquare(stoneType, token),
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
            var firstStones = new Dictionary<Vector2Int, StoneType>
            {
                { new Vector2Int(COL_COUNT / 2 - 1, ROW_COUNT / 2 - 1), StoneType.Enemy },
                { new Vector2Int(COL_COUNT / 2, ROW_COUNT / 2 - 1), StoneType.Player },
                { new Vector2Int(COL_COUNT / 2 - 1, ROW_COUNT / 2), StoneType.Player },
                { new Vector2Int(COL_COUNT / 2, ROW_COUNT / 2), StoneType.Enemy }
            };
            foreach (var (pos, type) in firstStones)
            {
                board.SetStone(type, pos);
                await view.PutStone(type, pos, token);
                onBoardChangedSubject.OnNext(board);
            }
        }

        private async UniTask SelectSquare(StoneType stoneType, CancellationToken token)
        {
            selectedPos = null;
            var canPutPoses = board.GetCanPutPoses(stoneType);
            
            // 石を置ける箇所がないとき
            if (!canPutPoses.Any())
            {
                return;
            }

            if (stoneType == StoneType.Player)
            {
                view.SetSquareHighlights(canPutPoses);
                await UniTask.WaitUntil(() => selectedPos.HasValue, cancellationToken: token);
                view.SetSquareHighlights(new List<Vector2Int>());
            }
            else
            {
                selectedPos = EnemyLogic.CalculateBestPutStonePos(board, StoneType.Enemy);
                await UniTask.Delay(300, cancellationToken: token);
            }
        }

        private async UniTask PutStone(StoneType stoneType, CancellationToken token)
        {
            if (!selectedPos.HasValue)
            {
                return;
            }
            
            board.SetStone(stoneType, selectedPos.Value);
            await view.PutStone(stoneType, selectedPos.Value, token);
            onBoardChangedSubject.OnNext(board);
        }

        private async UniTask ReverseStones(StoneType stoneType, CancellationToken token)
        {
            if (!selectedPos.HasValue)
            {
                return;
            }

            var reversePoses = board.GetReversePoses(stoneType, selectedPos.Value);
            
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

        private async UniTask ReverseStone(StoneType stoneType, Vector2Int pos, CancellationToken token)
        {
            board.SetStone(stoneType, pos);
            await view.ReverseStone(pos, token);
            onBoardChangedSubject.OnNext(board);
        }

        public bool IsFinishedGame()
        {
            // 空きマスがなくなったら終了
            if (!board.HasEmpty())
            {
                return true;
            }
            
            // 両者とも置けなくなったら終了
            var cantPutStone = !board.GetCanPutPoses(StoneType.Player).Any() && !board.GetCanPutPoses(StoneType.Enemy).Any();
            return cantPutStone;
        }
    }
}