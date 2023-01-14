using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Cycle;
using UnityEngine;

namespace Game.Board
{
    public class BoardManager : MonoBehaviour, ISubscribeGamePhase, ISubscribeTurnPhase, ICheckFinishGame
    {
        [SerializeField] private BoardView view;
        
        private Board board;
        
        private const int ROW_COUNT = 8;
        private const int COL_COUNT = 8;

        private void Awake()
        {
            board = new Board(COL_COUNT, ROW_COUNT);
        }

        public UniTask OnGamePhaseChanged(GameCycle.GamePhase phase, CancellationToken token)
        {
            return phase switch
            {
                GameCycle.GamePhase.Initialize => Initialize(token),
                _ => UniTask.CompletedTask
            };
        }

        public UniTask OnTurnPhaseChanged(GameCycle.TurnPhase phase, bool isPlayerTurn, CancellationToken token)
        {
            var stoneType = isPlayerTurn ? board.PlayerStoneType : board.EnemyStoneType;
            return phase switch
            {
                GameCycle.TurnPhase.SelectSquare => WaitForSelectSquare(stoneType, token),
                _ => UniTask.Delay(1000)
            };
        }

        private UniTask Initialize(CancellationToken token)
        {
            return view.CreateBoard(board, token);
        }

        private async UniTask WaitForSelectSquare(SquareType stoneType, CancellationToken token)
        {
            var canPutPoses = board.GetCanPutPoses(stoneType);
            view.SetSquareHighlights(canPutPoses);

            await UniTask.Delay(3000, cancellationToken: token);
            
            view.SetSquareHighlights(new List<Vector2Int>());
        }

        public bool IsFinishedGame()
        {
            return board?.IsFinishedGame() ?? false;
        }
    }
}