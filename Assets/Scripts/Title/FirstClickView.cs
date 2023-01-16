using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Title
{
    public class FirstClickView : MonoBehaviour
    {
        [SerializeField] private Button button;

        private void Awake()
        {
            button.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    SceneManager.LoadScene("Game");
                }).AddTo(button);
        }
    }
}