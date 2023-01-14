using Game.Board;
using UnityEngine;

namespace Game.Square
{
    public class SquareView : MonoBehaviour
    {
        [SerializeField] private GameObject highlight;
        [SerializeField] private SpriteRenderer stoneSpriteRenderer;

        [SerializeField] private Sprite blackStoneSprite;
        [SerializeField] private Sprite whiteStoneSprite;

        public void Initialize(Vector2 pos)
        {
            transform.position = pos;
            SetHighlight(false);
            SetStone(SquareType.Empty);
        }

        public void SetHighlight(bool isActive)
        {
            highlight.SetActive(isActive);
        }

        public void SetStone(SquareType type)
        {
            if (type == SquareType.Empty)
            {
                stoneSpriteRenderer.gameObject.SetActive(false);
                return;
            }

            var sprite = type == SquareType.Black ? blackStoneSprite : whiteStoneSprite;
            stoneSpriteRenderer.sprite = sprite;
            stoneSpriteRenderer.gameObject.SetActive(true);
        }
    }
}