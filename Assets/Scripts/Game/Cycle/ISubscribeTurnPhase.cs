using System.Threading;
using Cysharp.Threading.Tasks;

namespace Game.Cycle
{
    public interface ISubscribeTurnPhase
    {
        public UniTask OnTurnPhaseChanged(GameCycle.TurnPhase phase, bool isPlayerTurn, CancellationToken token);
    }
}