using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Board;

namespace Game.Cycle
{
    public interface ITurnPhaseEvent
    {
        public UniTask OnTurnPhaseChanged(GameCycle.TurnPhase phase, StoneType stoneType, CancellationToken token);
    }
}