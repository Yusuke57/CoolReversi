using UnityEngine;

namespace Game.Board.Enemy
{
    public static class EnemyLevelSetting
    {
        private const string PREF_KEY_ENEMY_LEVEL = "ENEMY_LEVEL";
        
        public static EnemyLevel CurrentEnemyLevel
        {
            get => (EnemyLevel) PlayerPrefs.GetInt(PREF_KEY_ENEMY_LEVEL, 1);
            set => PlayerPrefs.SetInt(PREF_KEY_ENEMY_LEVEL, (int) value);
        }
    }

    public enum EnemyLevel
    {
        Easy = 0,
        Normal = 1,
        Hard = 3
    }
}