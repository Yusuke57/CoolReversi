using Common;
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

        public void Initialize()
        {
            playerScoreText.color = colorPalette.playerStoneColor;
            enemyScoreText.color = colorPalette.enemyStoneColor;
        }

        public void ResetView()
        {
            SetScore(0, 0);
            winLabel.gameObject.SetActive(false);
            loseLabel.gameObject.SetActive(false);
        }

        public void SetScore(int playerScore, int enemyScore)
        {
            playerScoreText.text = playerScore.ToString("D2");
            enemyScoreText.text = enemyScore.ToString("D2");
        }

        public void ShowResultLabel(bool isPlayerWin)
        {
            winLabel.transform.position = (isPlayerWin ? playerScoreText : enemyScoreText).transform.position;
            loseLabel.transform.position = (isPlayerWin ? enemyScoreText : playerScoreText).transform.position;
            winLabel.gameObject.SetActive(true);
            loseLabel.gameObject.SetActive(true);
        }
    }
}