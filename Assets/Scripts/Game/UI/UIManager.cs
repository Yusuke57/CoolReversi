using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Board;
using Game.Cycle;
using UniRx;
using UnityEngine;

namespace Game.UI
{
    public class UIManager : MonoBehaviour, ISubscribeGamePhase, ISubscribeTurnPhase
    {
        [SerializeField] private GameObject playerTurnObj;
        [SerializeField] private GameObject enemyTurnObj;
        [SerializeField] private ScoreView scoreView;

        private IDisposable disposable;

        public void Initialize(IObservable<Board.Board> onBoardChangedAsObservable)
        {
            disposable?.Dispose();
            disposable = onBoardChangedAsObservable
                .Subscribe(ReloadScoreView)
                .AddTo(this);
        }
        
        public UniTask OnGamePhaseChanged(GameCycle.GamePhase phase, CancellationToken token)
        {
            return UniTask.CompletedTask;
        }

        public UniTask OnTurnPhaseChanged(GameCycle.TurnPhase phase, SquareType stoneType, CancellationToken token)
        {
            if (phase == GameCycle.TurnPhase.SelectSquare)
            {
                //playerTurnObj.SetActive(isPlayerTurn);
                //enemyTurnObj.SetActive(!isPlayerTurn);
            }
            
            return UniTask.CompletedTask;
        }

        private void ReloadScoreView(Board.Board board)
        {
            scoreView.SetScore(board.PlayerStoneCount, board.EnemyStoneCount);
        }
    }
}