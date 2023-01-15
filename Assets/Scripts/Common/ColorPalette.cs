using System;
using Game.Board;
using UnityEngine;

namespace Common
{
    [CreateAssetMenu(fileName = "ColorPalette", menuName = "ColorPalette", order = 0)]
    public class ColorPalette : ScriptableObject
    {
        public Color playerStoneColor;
        public Color enemyStoneColor;

        public Color GetStoneColor(StoneType stoneType)
        {
            return stoneType switch
            {
                StoneType.Player => playerStoneColor,
                StoneType.Enemy => enemyStoneColor,
                StoneType.Empty => Color.clear,
                _ => throw new ArgumentOutOfRangeException(nameof(stoneType), stoneType, null)
            };
        }
    }
}