using System;
using System.Threading;
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

        [SerializeField] private Sprite blackStoneSprite;
        [SerializeField] private Sprite whiteStoneSprite;

        private Action onClickAction;
        private SquareType currentSquareType;

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

        private Sprite GetStoneSprite(SquareType stoneType)
        {
            return stoneType == SquareType.Black ? blackStoneSprite : whiteStoneSprite;
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
            currentSquareType = SquareType.Empty;
        }

        public async UniTask PutStone(SquareType type, CancellationToken token)
        {
            if (type == SquareType.Empty)
            {
                SetEmpty();
                return;
            }

            currentSquareType = type;
            stoneSpriteRenderer.sprite = GetStoneSprite(type);
            stoneSpriteRenderer.gameObject.SetActive(true);

            // TODO: 仮
            stoneSpriteRenderer.color = new Color(1, 1, 1, 0);
            await DOTween.Sequence()
                .Append(stoneSpriteRenderer.DOFade(1, 0.12f))
                .ToUniTask(cancellationToken: token);
        }

        public async UniTask ReverseStone(CancellationToken token)
        {
            if (currentSquareType == SquareType.Empty)
            {
                return;
            }

            var reversedStoneType = currentSquareType == SquareType.Black ? SquareType.White : SquareType.Black;
            var reversedSprite = GetStoneSprite(reversedStoneType);

            // TODO: 仮
            await DOTween.Sequence()
                .Append(stoneSpriteRenderer.transform.DORotate(Vector3.up * 90, 0.08f))
                .AppendCallback(() => stoneSpriteRenderer.sprite = reversedSprite)
                .Append(stoneSpriteRenderer.transform.DORotate(Vector3.up * 0, 0.08f))
                .ToUniTask(cancellationToken: token);

            currentSquareType = reversedStoneType;
        }
    }
}