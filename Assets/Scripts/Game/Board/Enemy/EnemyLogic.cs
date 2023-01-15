using System.Linq;
using UnityEngine;

namespace Game.Board.Enemy
{
    public static class EnemyLogic
    {
        private static readonly int[,] cellEvaluations = new int[,]
        {
            { 30, -12, 0, -1, -1, 0, -12, 30 },
            { -12, -15, -3, -3, -3, -3, -15, -12 },
            { 0, -3, 0, -1, -1, 0, -3, 0 },
            { -1, -3, -1, -1, -1, -1, -3, -1 },
            { -1, -3, -1, -1, -1, -1, -3, -1 },
            { 0, -3, 0, -1, -1, 0, -3, 0 },
            { -12, -15, -3, -3, -3, -3, -15, -12 },
            { 30, -12, 0, -1, -1, 0, -12, 30 },
        };

        private static int GetCellEvaluation(Vector2Int pos)
        {
            return cellEvaluations[pos.x, pos.y];
        }
        
        public static Vector2Int CalculateBestPutStonePos(Board board, StoneType stoneType)
        {
            var canPutPoses = board.GetCanPutPoses(stoneType);

            var bestEvaluationOffset = int.MinValue;
            var bestPos = Vector2Int.zero;
            foreach (var canPutPos in canPutPoses)
            {
                var evaluationOffset = CalculatePutStoneEvaluationOffset(board, stoneType, canPutPos);
                if (evaluationOffset > bestEvaluationOffset)
                {
                    bestEvaluationOffset = evaluationOffset;
                    bestPos = canPutPos;
                }
            }

            return bestPos;
        }

        private static int CalculatePutStoneEvaluationOffset(Board board, StoneType stoneType, Vector2Int putPos)
        {
            var evaluationOffset = 0;

            evaluationOffset += GetCellEvaluation(putPos);
            
            var reversePoses = board.GetReversePoses(stoneType, putPos);
            evaluationOffset += reversePoses
                .SelectMany(poses => poses)
                .Sum(reversePos => GetCellEvaluation(reversePos) * 2);

            return evaluationOffset;
        }
    }
}