using System.Threading;
using Cysharp.Threading.Tasks;

namespace Game.Cycle
{
    public class GameCycle
    {
        public enum GamePhase
        {
            Initialize,
            Play,
            Finish
        }

        public enum TurnPhase
        {
            SelectCell,
            PutStone,
            ReverseStones
        }
        
        public delegate UniTask GamePhaseDelegate(GamePhase phase, CancellationToken token);
        public delegate UniTask TurnPhaseDelegate(TurnPhase phase, bool isPlayerTurn, CancellationToken token);
        public delegate bool CheckDelegate();

        public event GamePhaseDelegate OnGamePhaseChanged;
        public event TurnPhaseDelegate OnTurnPhaseChanged;
        public event CheckDelegate IsFinishedGame;

        public async UniTask PlayGame(CancellationToken token)
        {
            await (OnGamePhaseChanged?.Invoke(GamePhase.Initialize, token) ?? UniTask.CompletedTask);
            await UniTask.Delay(800, cancellationToken: token);
            await (OnGamePhaseChanged?.Invoke(GamePhase.Play, token) ?? UniTask.CompletedTask);

            var turnCount = 0;
            while (true)
            {
                var isPlayerTurn = turnCount++ % 2 == 0;
                await PlayTurn(isPlayerTurn, token);

                if (IsFinishedGame?.Invoke() ?? true)
                {
                    break;
                }
            }
            
            await (OnGamePhaseChanged?.Invoke(GamePhase.Finish, token) ?? UniTask.CompletedTask);
        }
        
        private async UniTask PlayTurn(bool isPlayerTurn, CancellationToken token)
        {
            await (OnTurnPhaseChanged?.Invoke(TurnPhase.SelectCell, isPlayerTurn, token) ?? UniTask.CompletedTask);
            await (OnTurnPhaseChanged?.Invoke(TurnPhase.PutStone, isPlayerTurn, token) ?? UniTask.CompletedTask);
            await UniTask.Delay(100, cancellationToken: token);
            await (OnTurnPhaseChanged?.Invoke(TurnPhase.ReverseStones, isPlayerTurn, token) ?? UniTask.CompletedTask);
            await UniTask.Delay(400, cancellationToken: token);
        }
    }
}