using System.Threading;
using Common;
using Cysharp.Threading.Tasks;
using Game.Board;
using Game.Cycle;
using UnityEngine;

namespace Game.Background
{
    public class BackgroundColorChanger : MonoBehaviour, IGamePhaseEvent, ITurnPhaseEvent
    {
        [SerializeField] private SpriteRenderer turnColorSpriteRenderer;
        [SerializeField] private ColorPalette colorPalette;

        public UniTask OnGamePhaseChanged(GameCycle.GamePhase phase, CancellationToken token)
        {
            turnColorSpriteRenderer.gameObject.SetActive(phase == GameCycle.GamePhase.Play);
            return UniTask.CompletedTask;
        }

        public UniTask OnTurnPhaseChanged(GameCycle.TurnPhase phase, StoneType stoneType, CancellationToken token)
        {
            turnColorSpriteRenderer.color = colorPalette.GetStoneColor(stoneType);
            turnColorSpriteRenderer.gameObject.SetActive(true);
            return UniTask.CompletedTask;
        }
    }
}