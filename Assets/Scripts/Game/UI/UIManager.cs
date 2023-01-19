using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Board;
using Game.Board.Enemy;
using Game.Cycle;
using UnityEngine;

namespace Game.UI
{
    public class UIManager : MonoBehaviour, IGamePhaseEvent, ITurnPhaseEvent
    {
        [SerializeField] private ScoreView scoreView;
        [SerializeField] private EnemyLevelView enemyLevelView;
        [SerializeField] private PassView passView;
        [SerializeField] private GameObject retryTextObj;

        private int currentPlayerScore;
        private int currentEnemyScore;
        private Board.Board cachedBoard;

        private void Awake()
        {
            scoreView.Initialize();
            enemyLevelView.OnLevelClicked = ChangeEnemyLevel;
            enemyLevelView.SetLevel(EnemyLevelSetting.CurrentEnemyLevel);
        }
        
        public UniTask OnGamePhaseChanged(GameCycle.GamePhase phase, CancellationToken token)
        {
            switch (phase)
            {
                case GameCycle.GamePhase.Initialize:
                    scoreView.ResetView();
                    retryTextObj.SetActive(false);
                    passView.gameObject.SetActive(false);
                    break;
                case GameCycle.GamePhase.Finish:
                    var isPlayerWin = currentPlayerScore >= currentEnemyScore;
                    scoreView.ShowResultLabel(isPlayerWin);
                    retryTextObj.SetActive(true);
                    break;
            }

            return UniTask.CompletedTask;
        }
        
        public UniTask OnTurnPhaseChanged(GameCycle.TurnPhase phase, StoneType stoneType, CancellationToken token)
        {
            if (phase == GameCycle.TurnPhase.SelectCell && !cachedBoard.GetCanPutPoses(stoneType).Any())
            {
                return ShowPass(token);
            }

            return UniTask.CompletedTask;
        }

        private void ReloadScoreView(int playerStoneCount, int enemyStoneCount)
        {
            scoreView.SetScore(currentPlayerScore, currentEnemyScore,playerStoneCount, enemyStoneCount);
            currentPlayerScore = playerStoneCount;
            currentEnemyScore = enemyStoneCount;
        }

        private void ChangeEnemyLevel(EnemyLevel level)
        {
            EnemyLevelSetting.CurrentEnemyLevel = level;
            enemyLevelView.SetLevel(level);
        }

        private async UniTask ShowPass(CancellationToken token)
        {
            await passView.Play(token);
        }

        public void OnBoardChanged(Board.Board board)
        {
            cachedBoard = board;
            ReloadScoreView(board.PlayerStoneCount, board.EnemyStoneCount);
        }
    }
}