using UnityEngine;

namespace FishingGame.Models
{
    /// <summary>全局数值配置资产。</summary>
    [CreateAssetMenu(fileName = "GameConfig", menuName = "FishingGame/GameConfig")]
    public class GameConfig : ScriptableObject
    {
        [Header("深度与速度（米/秒,1m = 1unit")]
        /// <summary>最大可下潜深度(米)</summary>
        public float MaxDepth = 100f;
        /// <summary>下潜阶段钩子速度</summary>
        public float DescentSpeed = 6;
        /// <summary>上升阶段钩子速度</summary>
        public float AscentSpeed = 6f;
        /// <summary>满载后快速上浮速度(结算更快返回)。</summary>
        public float FastAscentSpeed = 24f;

        [Header("容量与技能")]
        /// <summary>单局最多捕获条数,达上限强制进入快速上浮。</summary>
        public int CatchCapacity = 10;
        /// <summary>单局护盾技能可用次数。</summary>
        public int SkillCount = 2;

        [Header("奖励")]
        public int CoinReward = 10;
        public int ShellReward = 10;
    }
}