using TMPro;
using UnityEngine;

namespace Game.UI
{
    public class ScoreView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI playerScoreText;
        [SerializeField] private TextMeshProUGUI enemyScoreText;

        public void SetScore(int playerScore, int enemyScore)
        {
            playerScoreText.text = playerScore.ToString("D2");
            enemyScoreText.text = enemyScore.ToString("D2");
        }
    }
}