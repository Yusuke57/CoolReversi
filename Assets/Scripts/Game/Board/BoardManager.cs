using System;
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
            return phase switch
            {
                _ => UniTask.Delay(1000)
            };
        }

        private UniTask Initialize(CancellationToken token)
        {
            return view.CreateBoard(board, token);
        }

        public bool IsFinishedGame()
        {
            return board?.IsFinishedGame() ?? false;
        }
    }
}