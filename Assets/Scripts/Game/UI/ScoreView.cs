using Common;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Game.UI
{
    public class ScoreView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI playerScoreText;
        [SerializeField] private TextMeshProUGUI enemyScoreText;
        [SerializeField] private Transform winLabel;
        [SerializeField] private Transform loseLabel;
        [SerializeField] private ColorPalette colorPalette;

        private Tweener playerScoreTweener;
        private Tweener enemyScoreTweener;

        public void Initialize()
        {
            playerScoreText.color = colorPalette.playerStoneColor;
            enemyScoreText.color = colorPalette.enemyStoneColor;
        }

        public void ResetView()
        {
            SetScore(0, 0, 0, 0);
            winLabel.gameObject.SetActive(false);
            loseLabel.gameObject.SetActive(false);
        }

        public void SetScore(int prePlayerScore, int preEnemyScore, int playerScore, int enemyScore)
        {
            playerScoreText.text = playerScore.ToString("D2");
            enemyScoreText.text = enemyScore.ToString("D2");

            const float punchScale = 1.1f;
            const float punchDuration = 0.1f;
            if (prePlayerScore != playerScore)
            {
                playerScoreTweener?.Complete();
                playerScoreText.transform.localScale = Vector3.one * punchScale;
                playerScoreTweener = playerScoreText.transform.DOScale(1, punchDuration);
            }
            if (preEnemyScore != enemyScore)
            {
                enemyScoreTweener?.Complete();
                enemyScoreText.transform.localScale = Vector3.one * punchScale;
                enemyScoreTweener = enemyScoreText.transform.DOScale(1, punchDuration);
            }
        }

        public void ShowResultLabel(bool isPlayerWin)
        {
            winLabel.transform.position = (isPlayerWin ? playerScoreText : enemyScoreText).transform.position;
            loseLabel.transform.position = (isPlayerWin ? enemyScoreText : playerScoreText).transform.position;
            winLabel.gameObject.SetActive(true);
            loseLabel.gameObject.SetActive(true);

            var seName = isPlayerWin ? SEPlayer.SEName.Win : SEPlayer.SEName.Lose;
            SEPlayer.I.Play(seName);
        }
    }
}