using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Cycle;
using UniRx;
using UnityEngine;

namespace Game.UI
{
    public class UIManager : MonoBehaviour, ISubscribeGamePhase
    {
        [SerializeField] private ScoreView scoreView;

        private IDisposable disposable;

        public void Initialize(IObservable<Board.Board> onBoardChangedAsObservable)
        {
            disposable?.Dispose();
            disposable = onBoardChangedAsObservable
                .Subscribe(ReloadScoreView)
                .AddTo(this);
            
            scoreView.Initialize();
        }
        
        public UniTask OnGamePhaseChanged(GameCycle.GamePhase phase, CancellationToken token)
        {
            if (phase == GameCycle.GamePhase.Initialize)
            {
                scoreView.SetScore(0, 0);
            }

            return UniTask.CompletedTask;
        }

        private void ReloadScoreView(Board.Board board)
        {
            scoreView.SetScore(board.PlayerStoneCount, board.EnemyStoneCount);
        }
    }
}