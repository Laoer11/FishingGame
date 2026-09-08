using UnityEngine;

namespace FishingGame.Models
{
    /// <summary>鱼种定义资产。</summary>
    [CreateAssetMenu(fileName = "FishDef_", menuName = "FishingGame/FishDef")]
    public class FishDef : ScriptableObject
    {
        [Header("基础信息")]
        /// <summary>程序内标识</summary>
        public string Id;
        /// <summary>结算面板展示名</summary>
        public string DisplayName;
        public Sprite Sprite;
        /// <summary>碰撞体与视觉尺寸,FishController 据此设置 BoxCollider2D 大小(× 0.8)。</summary>
        public Vector2 SpriteSize = new Vector2(1.2f, 0.8f);
        /// <summary>稀有度星级(1-3)</summary>
        public int StarLevel = 1;

        [Header("奖励")]
        public int CoinReward = 10;
        public int ShellReward = 10;

        [Header("深度带与行为")]
        /// <summary>生成深度带下限(米)。SpawnerController 只在该深度带内生成此鱼。</summary>
        public float MinDepth = 0f;
        /// <summary>生成深度带上限(米)</summary>
        public float MaxDepth = 100f;
        /// <summary>游动行为类型</summary>
        public FishBehaviorType Behavior = FishBehaviorType.Patrol;
        public float MoveSpeed = 2f;
        /// <summary>碰撞判定半径(米)</summary>
        public float ColliderRadius = 0.5f;
    }

    /// <summary>鱼游动行为类型</summary>
    public enum FishBehaviorType
    {
        /// <summary>左右往返巡逻</summary>
        Patrol,
        /// <summary>原地缓漂</summary>
        Drift,
        /// <summary>蛇形摆动</summary>
        Snake
    }
}