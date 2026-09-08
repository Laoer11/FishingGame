using FishingGame.Core;
using UnityEngine;

namespace FishingGame.Controllers
{
    /// <summary>护盾可视化:订阅护盾数量事件,Left>0 显示护盾泡泡,归零隐藏。挂 Fishhook 根物体。</summary>
    public class ShieldBubbleView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _bubble;

        /// <summary>启动时按当前剩余护盾初始化可见性</summary>
        private void Start()
        {
            if(GameManager.Instance != null && GameManager.Instance.Model != null)
            {
                SetVisible(GameManager.Instance.Model.SkillLeft > 0);
            }
        }

        private void OnEnable()
        {
            EventBus.Instance.Subscribe<SkillCountChangedEvent>(OnShieldChanged);
        }

        private void OnDisable()
        {
            if(EventBus.Instance != null) EventBus.Instance.Unsubscribe<SkillCountChangedEvent>(OnShieldChanged);
        }

        /// <summary>护盾剩余>0 显示气泡,归零隐藏。切上升阶段时由 GameManager 发事件自动隐藏。</summary>
        private void OnShieldChanged(SkillCountChangedEvent evt)
        {
            SetVisible(evt.Left > 0);
        }

        private void SetVisible(bool visible)
        {
            if(_bubble == null) return;
            _bubble.enabled = visible;
        }
    }
}