using System;
using System.Threading;
using Common;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Board.Square
{
    public class SquareView : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Transform stone;
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
            
            stoneSpriteRenderer.sortingOrder = 0;
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

            stone.localPosition = Vector3.up;
            var color = stoneSpriteRenderer.color;
            color.a = 0;
            stoneSpriteRenderer.color = color;
            stoneSpriteRenderer.sortingOrder = 1;
            
            SEPlayer.I.Play(SEPlayer.SEName.PutStone);
            await DOTween.Sequence()
                .Append(stoneSpriteRenderer.DOFade(1, 0.2f))
                .Join(stone.DOLocalMoveY(0, 0.2f))
                .SetLink(stone.gameObject)
                .ToUniTask(TweenCancelBehaviour.Complete, token);
            stoneSpriteRenderer.sortingOrder = 0;
        }

        public async UniTask ReverseStone(CancellationToken token)
        {
            if (currentStoneType == StoneType.Empty)
            {
                return;
            }

            var reversedStoneType = currentStoneType == StoneType.Player ? StoneType.Enemy : StoneType.Player;
            stoneSpriteRenderer.sortingOrder = 1;
            
            SEPlayer.I.Play(SEPlayer.SEName.ReverseStone);
            await DOTween.Sequence()
                .Append(stone.DOLocalMoveY(0.4f, 0.08f).SetEase(Ease.InQuad))
                .Append(stoneSpriteRenderer.transform.DORotate(Vector3.up * 90, 0.08f).SetEase(Ease.InQuart))
                .AppendCallback(() => SetStoneColor(reversedStoneType))
                .Append(stoneSpriteRenderer.transform.DORotate(Vector3.up * 0, 0.08f))
                .Append(stone.DOLocalMoveY(0, 0.08f))
                .SetLink(stone.gameObject)
                .ToUniTask(TweenCancelBehaviour.Complete, token);
            
            stoneSpriteRenderer.sortingOrder = 0;
            currentStoneType = reversedStoneType;
        }
    }
}