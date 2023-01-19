using System;
using System.Threading;
using Common;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Board.Cell
{
    public class CellView : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Transform stone;
        [SerializeField] private SpriteRenderer stoneSpriteRenderer;
        [SerializeField] private SpriteRenderer highlightSpriteRenderer;
        [SerializeField] private ColorPalette colorPalette;

        private StoneType? currentStoneType;
        private Tweener highlightTweener;

        public Action OnClickAction { private get; set; }
        
        private const float DEFAULT_HIGHLIGHT_ALPHA = 0.3f;
        private const float HOVER_HIGHLIGHT_ALPHA = 0.7f;

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClickAction?.Invoke();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            SetHighlightAlpha(HOVER_HIGHLIGHT_ALPHA);
            SEPlayer.I.Play(SEPlayer.SEName.HoverCell);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            SetHighlightAlpha(DEFAULT_HIGHLIGHT_ALPHA);
        }

        public void ResetView()
        {
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
                SetHighlightAlpha(DEFAULT_HIGHLIGHT_ALPHA);
            }
            highlightSpriteRenderer.gameObject.SetActive(isActive);
        }

        private void SetHighlightAlpha(float alpha)
        {
            var color = highlightSpriteRenderer.color;
            color.a = alpha;
            highlightSpriteRenderer.color = color;
        }

        private void SetEmpty()
        {
            stoneSpriteRenderer.gameObject.SetActive(false);
            currentStoneType = null;
        }

        public async UniTask PutStone(StoneType stoneType, CancellationToken token)
        {
            currentStoneType = stoneType;
            SetStoneColor(stoneType);
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