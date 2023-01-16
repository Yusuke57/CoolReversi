using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Game.UI
{
    public class PassView : MonoBehaviour
    {
        [SerializeField] private RectTransform background;
        [SerializeField] private GameObject content;

        public async UniTask Play(CancellationToken token)
        {
            background.localScale = new Vector3(1, 0, 1);
            content.SetActive(false);
            gameObject.SetActive(true);

            await DOTween.Sequence()
                .Append(background.DOScaleY(1, 0.2f))
                .AppendCallback(() => content.SetActive(true))
                .AppendInterval(0.8f)
                .AppendCallback(() => content.SetActive(false))
                .Append(background.DOScaleY(0, 0.12f))
                .ToUniTask(cancellationToken: token);

            gameObject.SetActive(false);
        }
    }
}