using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Background;
using Game.Board;
using Game.UI;
using UnityEngine;

namespace Game.Cycle
{
    public class GameCycleExecutor : MonoBehaviour
    {
        [SerializeField] private BoardManager boardManager;
        [SerializeField] private UIManager uiManager;
        [SerializeField] private BackgroundManager backgroundManager;
        [SerializeField] private GameObject finishGameCheckerObject;
        
        private List<ISubscribeGamePhase> gamePhaseSubscribers;
        private List<ISubscribeTurnPhase> turnPhaseSubscribers;
        private ICheckFinishGame finishGameChecker;
        
        private GameCycle cycle;
        private CancellationTokenSource tokenSource;

        private void Start()
        {
            CacheInterfaces();
            InitializeUI();
            Execute();
        }

        private void CacheInterfaces()
        {
            gamePhaseSubscribers = new List<ISubscribeGamePhase>
            {
                boardManager.GetComponent<ISubscribeGamePhase>(),
                uiManager.GetComponent<ISubscribeGamePhase>(),
                backgroundManager.GetComponent<ISubscribeGamePhase>()
            };
            
            turnPhaseSubscribers = new List<ISubscribeTurnPhase>
            {
                boardManager.GetComponent<ISubscribeTurnPhase>(),
                backgroundManager.GetComponent<ISubscribeTurnPhase>()
            };
            
            finishGameChecker = finishGameCheckerObject.GetComponent<ICheckFinishGame>();
        }

        private void InitializeUI()
        {
            uiManager.Initialize(boardManager.OnBoardChangedAsObservable);
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
            var stoneType = isPlayerTurn ? SquareType.Black : SquareType.White;
            var tasks = turnPhaseSubscribers
                .Select(subscriber => subscriber.OnTurnPhaseChanged(phase, stoneType, token))
                .ToList();

            return UniTask.WhenAll(tasks);
        }
    }
}