using System;
using Common;
using Game.Board.Enemy;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class EnemyLevelView : MonoBehaviour
    {
        [SerializeField] private EnemyLevelButton[] buttonGroups;
        
        public Action<EnemyLevel> OnLevelClicked { private get; set; }

        private void Awake()
        {
            foreach (var buttonGroup in buttonGroups)
            {
                buttonGroup.button.OnClickAsObservable()
                    .Subscribe(_ =>
                    {
                        SEPlayer.I.Play(SEPlayer.SEName.Button);
                        OnLevelClicked?.Invoke(buttonGroup.level);
                    })
                    .AddTo(buttonGroup.button);
            }
        }

        public void SetLevel(EnemyLevel level)
        {
            foreach (var buttonGroup in buttonGroups)
            {
                buttonGroup.activeObject.SetActive(buttonGroup.level == level);
            }
        }
        
        [Serializable]
        private struct EnemyLevelButton
        {
            public EnemyLevel level;
            public Button button;
            public GameObject activeObject;
        }
    }
}