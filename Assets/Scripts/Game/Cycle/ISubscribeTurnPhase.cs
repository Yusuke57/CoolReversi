using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Board;

namespace Game.Cycle
{
    public interface ISubscribeTurnPhase
    {
        public UniTask OnTurnPhaseChanged(GameCycle.TurnPhase phase, SquareType stoneType, CancellationToken token);
    }
}