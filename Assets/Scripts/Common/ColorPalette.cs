using System;
using Game.Board;
using UnityEngine;

namespace Common
{
    [CreateAssetMenu(fileName = "ColorPalette", menuName = "ColorPalette", order = 0)]
    public class ColorPalette : ScriptableObject
    {
        public Color blackColor;
        public Color whiteColor;

        public Color GetStoneColor(SquareType stoneType)
        {
            return stoneType switch
            {
                SquareType.Black => blackColor,
                SquareType.White => whiteColor,
                SquareType.Empty => Color.clear,
                _ => throw new ArgumentOutOfRangeException(nameof(stoneType), stoneType, null)
            };
        }
    }
}