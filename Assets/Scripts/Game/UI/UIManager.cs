using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Board.Enemy;
using Game.Cycle;
using UniRx;
using UnityEngine;

namespace Game.UI
{
    public class UIManager : MonoBehaviour, ISubscribeGamePhase
    {
        [SerializeField] private ScoreView scoreView;
        [SerializeField] private EnemyLevelView enemyLevelView;
        [SerializeField] private GameObject retryTextObj;

        private int currentPlayerScore;
        private int currentEnemyScore;
        private IDisposable disposable;

        private void Awake()
        {
            scoreView.Initialize();
            enemyLevelView.OnLevelClicked = ChangeEnemyLevel;
            enemyLevelView.SetLevel(EnemyLevelSetting.CurrentEnemyLevel);
        }

        public void Initialize(IObservable<Board.Board> onBoardChangedAsObservable)
        {
            disposable?.Dispose();
            disposable = onBoardChangedAsObservable
                .Subscribe(ReloadScoreView)
                .AddTo(this);

            retryTextObj.SetActive(false);
        }
        
        public UniTask OnGamePhaseChanged(GameCycle.GamePhase phase, CancellationToken token)
        {
            switch (phase)
            {
                case GameCycle.GamePhase.Initialize:
                    scoreView.ResetView();
                    retryTextObj.SetActive(false);
                    break;
                case GameCycle.GamePhase.Finish:
                    var isPlayerWin = currentPlayerScore >= currentEnemyScore;
                    scoreView.ShowResultLabel(isPlayerWin);
                    retryTextObj.SetActive(true);
                    break;
            }

            return UniTask.CompletedTask;
        }

        private void ReloadScoreView(Board.Board board)
        {
            scoreView.SetScore(currentPlayerScore, currentEnemyScore, board.PlayerStoneCount, board.EnemyStoneCount);
            currentPlayerScore = board.PlayerStoneCount;
            currentEnemyScore = board.EnemyStoneCount;
        }

        private void ChangeEnemyLevel(EnemyLevel level)
        {
            EnemyLevelSetting.CurrentEnemyLevel = level;
            enemyLevelView.SetLevel(level);
        }
    }
}