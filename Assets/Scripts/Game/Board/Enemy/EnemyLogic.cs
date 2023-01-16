using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.Board.Enemy
{
    public static class EnemyLogic
    {
        private static readonly int[,] cellEvaluations =
        {
            { 100, -12, 0, -1, -1, 0, -12, 100 },
            { -12, -15, -3, -3, -3, -3, -15, -12 },
            { 0, -3, 0, -1, -1, 0, -3, 0 },
            { -1, -3, -1, -1, -1, -1, -3, -1 },
            { -1, -3, -1, -1, -1, -1, -3, -1 },
            { 0, -3, 0, -1, -1, 0, -3, 0 },
            { -12, -15, -3, -3, -3, -3, -15, -12 },
            { 100, -12, 0, -1, -1, 0, -12, 100 },
        };

        private static int GetCellEvaluation(Vector2Int pos)
        {
            return cellEvaluations[pos.x, pos.y];
        }
        
        public static async UniTask<Vector2Int> CalculatePutStonePos
            (Board board, StoneType stoneType, EnemyLevel level, CancellationToken token)
        {
            var calculateTurnCount = (int) level;
            
            // ランダムで置く
            if (calculateTurnCount == 0)
            {
                var canPutPoses = board.GetCanPutPoses(stoneType);
                return canPutPoses[Random.Range(0, canPutPoses.Count)];
            }
            
            var opponentStoneType = stoneType == StoneType.Player ? StoneType.Enemy : StoneType.Player;
            var calculatedMyTurnCandidates = CalculateAllOnNextTurn(board, stoneType);
            var calculatedOpponentTurnCandidates = new List<(Board reversedBoard, Vector2Int pos, int evaluation)>();

            for (var i = 1; i < calculateTurnCount; i++)
            {
                calculatedOpponentTurnCandidates.Clear();
                foreach (var (reversedBoard, firstPos, _) in calculatedMyTurnCandidates)
                {
                    // 次の相手のターン
                    var opponentTurnCandidates = CalculateAllOnNextTurn(reversedBoard, opponentStoneType);
                    // 相手が石を置けない状況は確定最善手とする
                    if (!opponentTurnCandidates.Any())
                    {
                        return firstPos;
                    }
                    
                    // 相手は最善手を打つ
                    var opponentTurn = opponentTurnCandidates.OrderByDescending(ev => ev.evaluation).First();
                    opponentTurn.pos = firstPos;
                    calculatedOpponentTurnCandidates.Add(opponentTurn);
                    
                    //await UniTask.NextFrame(token);
                }
                
                var isMyTurnCandidatesCleared = false;
                foreach (var (reversedBoard, firstPos, _) in calculatedOpponentTurnCandidates)
                {
                    // その次の自分のターン
                    var calculatedMyTurnCandidatesOnThisBoard = CalculateAllOnNextTurn(reversedBoard, stoneType);
                    if (calculatedMyTurnCandidatesOnThisBoard.Any())
                    {
                        if (!isMyTurnCandidatesCleared)
                        {
                            // このターンで一手も打てない場合は前ターンのcalculatedMyTurnCandidatesが残るようにする
                            // => 候補が見つかってからClear()する
                            calculatedMyTurnCandidates.Clear();
                            isMyTurnCandidatesCleared = true;
                        }
                        calculatedMyTurnCandidates.AddRange(calculatedMyTurnCandidatesOnThisBoard
                            .Select(info => (info.reversedBoard, firstPos, info.evaluation)));
                    }
                    
                    //await UniTask.NextFrame(token);
                }
            }
            await UniTask.NextFrame(token);

            // 最終的に一番評価値が高かった盤面の、最初の一手を返す
            var bestFirstPos = calculatedMyTurnCandidates
                .OrderByDescending(candidate => candidate.evaluation)
                .First()
                .pos;
            return bestFirstPos;
        }

        private static List<(Board reversedBoard, Vector2Int pos, int evaluation)> 
            CalculateAllOnNextTurn(Board board, StoneType stoneType)
        {
            var resultList = new List<(Board reversedBoard, Vector2Int pos, int evaluation)>();
            var canPutPoses = board.GetCanPutPoses(stoneType);
            foreach (var canPutPos in canPutPoses)
            {
                var reversedBoard = CreateStoneReversedBoard(board, stoneType, canPutPos);
                var evaluation = CalculateBoardEvaluation(reversedBoard, stoneType);
                resultList.Add((reversedBoard, canPutPos, evaluation));
            }

            return resultList;
        }

        // 石を置いた後の盤面を作成
        private static Board CreateStoneReversedBoard(Board originBoard, StoneType stoneType, Vector2Int putPos)
        {
            var board = originBoard.Clone();
            board.SetStone(stoneType, putPos);
            
            var reversePoses = board.GetReversePoses(stoneType, putPos).SelectMany(poses => poses);
            foreach (var reversePos in reversePoses)
            {
                board.SetStone(stoneType, reversePos);
            }

            return board;
        }

        // 盤面の評価値を計算
        private static int CalculateBoardEvaluation(Board board, StoneType targetStoneType)
        {
            var playerEvaluation = 0;
            var enemyEvaluation = 0;
            for (var row = 0; row < board.RowCount; row++)
            {
                for (var col = 0; col < board.ColCount; col++)
                {
                    var pos = new Vector2Int(col, row);
                    var type = board.GetStoneType(pos);
                    var cellEvaluation = GetCellEvaluation(pos);
                    if (type == StoneType.Player)
                    {
                        playerEvaluation += cellEvaluation;
                    }
                    else if (type == StoneType.Enemy)
                    {
                        enemyEvaluation += cellEvaluation;
                    }
                }
            }
            
            return targetStoneType == StoneType.Player
                ? playerEvaluation - enemyEvaluation
                : enemyEvaluation - playerEvaluation;
        }
    }
}