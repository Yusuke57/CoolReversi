using System;
using System.Threading;
using Common;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Board;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Square
{
    public class SquareView : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private SpriteRenderer stoneSpriteRenderer;
        [SerializeField] private SpriteRenderer highlightSpriteRenderer;
        [SerializeField] private ColorPalette colorPalette;

        private Action onClickAction;
        private StoneType currentStoneType;

        public void OnPointerClick(PointerEventData eventData)
        {
            onClickAction?.Invoke();
        }

        public void Initialize(Vector2 pos, Action onClick)
        {
            transform.position = pos;
            onClickAction = onClick;
            
            SetHighlight(false);
            SetEmpty();
        }

        private void SetStoneColor(StoneType stoneType)
        {
            var stoneColor = colorPalette.GetStoneColor(stoneType);
            stoneColor += new Color(0.1f, 0.1f, 0.1f); // 元画像が若干グレーなので少し明るくする
            stoneSpriteRenderer.color = stoneColor;
        }

        public void SetHighlight(bool isActive)
        {
            if (isActive)
            {
                highlightSpriteRenderer.DOFade(0.3f, 0);
            }
            highlightSpriteRenderer.gameObject.SetActive(isActive);
        }

        private void SetEmpty()
        {
            stoneSpriteRenderer.gameObject.SetActive(false);
            currentStoneType = StoneType.Empty;
        }

        public async UniTask PutStone(StoneType type, CancellationToken token)
        {
            if (type == StoneType.Empty)
            {
                SetEmpty();
                return;
            }

            currentStoneType = type;
            SetStoneColor(type);
            stoneSpriteRenderer.gameObject.SetActive(true);

            // TODO: 仮
            var color = stoneSpriteRenderer.color;
            color.a = 0;
            stoneSpriteRenderer.color = color;
            await DOTween.Sequence()
                .Append(stoneSpriteRenderer.DOFade(1, 0.12f))
                .ToUniTask(cancellationToken: token);
        }

        public async UniTask ReverseStone(CancellationToken token)
        {
            if (currentStoneType == StoneType.Empty)
            {
                return;
            }

            var reversedStoneType = currentStoneType == StoneType.Player ? StoneType.Enemy : StoneType.Player;

            // TODO: 仮
            await DOTween.Sequence()
                .Append(stoneSpriteRenderer.transform.DORotate(Vector3.up * 90, 0.08f))
                .AppendCallback(() => SetStoneColor(reversedStoneType))
                .Append(stoneSpriteRenderer.transform.DORotate(Vector3.up * 0, 0.08f))
                .ToUniTask(cancellationToken: token);

            currentStoneType = reversedStoneType;
        }
    }
}