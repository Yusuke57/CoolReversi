using System.Threading;
using Cysharp.Threading.Tasks;

namespace Game.Cycle
{
    public interface ISubscribeGamePhase
    {
        public UniTask OnGamePhaseChanged(GameCycle.GamePhase phase, CancellationToken token);
    }
}