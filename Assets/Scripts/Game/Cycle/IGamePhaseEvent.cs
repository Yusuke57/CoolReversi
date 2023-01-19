using System.Threading;
using Cysharp.Threading.Tasks;

namespace Game.Cycle
{
    public interface IGamePhaseEvent
    {
        public UniTask OnGamePhaseChanged(GameCycle.GamePhase phase, CancellationToken token);
    }
}