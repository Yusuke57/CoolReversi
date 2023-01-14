using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.Cycle
{
    public class GameCycleExecutor : MonoBehaviour
    {
        [SerializeField] private GameObject[] subscribeObjects;
        [SerializeField] private GameObject finishGameCheckerObject;
        private ISubscribeGamePhase[] gamePhaseSubscribers;
        private ISubscribeTurnPhase[] turnPhaseSubscribers;
        private ICheckFinishGame finishGameChecker;
        
        private GameCycle cycle;
        private CancellationTokenSource tokenSource;

        private void Start()
        {
            CacheInterfaces();
            Execute();
        }

        private void CacheInterfaces()
        {
            gamePhaseSubscribers = subscribeObjects
                .Select(obj => obj.GetComponent<ISubscribeGamePhase>())
                .Where(component => component != null)
                .ToArray();
            
            turnPhaseSubscribers = subscribeObjects
                .Select(obj => obj.GetComponent<ISubscribeTurnPhase>())
                .Where(component => component != null)
                .ToArray();

            finishGameChecker = finishGameCheckerObject.GetComponent<ICheckFinishGame>();
        }

        private void Execute()
        {
            try
            {
                cycle = new GameCycle();
                tokenSource = new CancellationTokenSource();
                
                cycle.OnGamePhaseChanged += OnGamePhaseChanged;
                cycle.OnTurnPhaseChanged += OnTurnPhaseChanged;
                cycle.IsFinishedGame += () => finishGameChecker.IsFinishedGame();
                cycle.PlayGame(tokenSource.Token).Forget();
            }
            catch (Exception e)
            {
                // do nothing
            }
        }

        private UniTask OnGamePhaseChanged(GameCycle.GamePhase phase, CancellationToken token)
        {
            var tasks = gamePhaseSubscribers
                .Select(subscriber => subscriber.OnGamePhaseChanged(phase, token))
                .ToList();

            return UniTask.WhenAll(tasks);
        }

        private UniTask OnTurnPhaseChanged(GameCycle.TurnPhase phase, bool isPlayerTurn, CancellationToken token)
        {
            var tasks = turnPhaseSubscribers
                .Select(subscriber => subscriber.OnTurnPhaseChanged(phase, isPlayerTurn, token))
                .ToList();

            return UniTask.WhenAll(tasks);
        }
    }
}