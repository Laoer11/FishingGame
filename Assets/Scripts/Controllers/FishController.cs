using FishingGame.Models;
using UnityEngine;

namespace FishingGame.Controllers
{
    /// <summary>鱼实例:管外观配置、碰撞、游动行为调度、被捕获挂钩与复用清理。</summary>
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class FishController : MonoBehaviour
    {
        public FishDef Def { get;private set; }
        /// <summary>鱼是否已被钩住</summary>
        public bool IsCaught { get;private set; }
        public SpriteRenderer Renderer { get;private set; }
        public BoxCollider2D _collider;
        /// <summary>当前游动策略</summary>
        private IFishBehavior _behavior;
        /// <summary>摆尾动画相位计时</summary>
        private float _wobbleT;

        private void Awake()
        {
            Renderer = GetComponent<SpriteRenderer>();
            _collider = GetComponent<BoxCollider2D>();
        }

        /// <summary>池化复用配置入口:重设贴图→按目标尺寸/贴图尺寸比例归一缩放→按Def选行为策略。</summary>
        public void SetUp(FishDef def)
        {
            Def = def;
            IsCaught = false;
            _collider.enabled = true;

            Renderer.sprite = def.Sprite;
            if(def.Sprite != null)
            {
                Bounds b = def.Sprite.bounds;
                if(b.size.x > 0.001f && b.size.y > 0.001f)
                {
                    transform.localScale = new Vector3(
                        def.SpriteSize.x / b.size.x,
                        def.SpriteSize.y / b.size.y,
                        1f
                    );
                }
            }
            _collider.size = def.SpriteSize * 0.8f;

            _behavior = CreateBehavior(def.Behavior);
        }

        /// <summary>策略选择点:按 FishBehaviorType 创建对应行为实例</summary>
        private static IFishBehavior CreateBehavior(FishBehaviorType type)
        {
            switch(type)
            {
                case FishBehaviorType.Drift:
                    return new DriftBehavior();
                case FishBehaviorType.Snake:
                    return new SnakeBehavior();
                default:
                    return new PatrolBehavior();
            }
        }

        private void Update()
        {
            if(Def == null) return;

            if(IsCaught)
            {
                _wobbleT += Time.deltaTime * 6f;
                float phase = -transform.localPosition.y * 2.5f;
                transform.localRotation = Quaternion.Euler(0f,0f,
                    Mathf.Sin(_wobbleT + phase) * 12f);
                return;
            }

            if(_behavior != null) _behavior.Tick(this, Time.deltaTime);
        }

        /// <summary>被捕获:关碰撞→挂到钩子 CatchPoint 下(多条鱼按序号排开)→切摆尾动画。</summary>
        public void OnCaught(Transform parent)
        {
            if(IsCaught) return;
            IsCaught = true;
            _collider.enabled = false;

            transform.SetParent(parent);
            int index = parent.childCount - 1;
            transform.localPosition = new Vector3(0f,-0.8f * index,0f);
            transform.localRotation = Quaternion.identity;
            _wobbleT = 0f;
        }

        /// <summary>复用前清理:脱离父级、恢复缩放。入桶与取桶时各调一次,双重保险防脏状态泄漏。</summary>
        public void PrepareDespawn()
        {
            IsCaught = false;
            _collider.enabled = true;
            transform.SetParent(null);
            transform.localScale = Vector3.one;
        }
    }
}