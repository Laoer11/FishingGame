// DescendingState: 下潜阶段。钩子移动由 HookController 驱动(监听 PhaseChangedEvent,
// 速度取自 Config,禁止硬编码);本状态只负责流程判定:
// 深度达上限时切 Ascending(触鱼判定第 6 章接入)。对应视频"下潜避鱼"阶段。
using FishingGame.Controllers;
using FishingGame.Core;
using FishingGame.Models;
using UnityEngine;

namespace FishingGame.States
{
    public class DescendingState : GameState
    {
        public DescendingState(GameStateMachine machine) : base(machine) { }

        public override GamePhase Phase => GamePhase.Descending;

        /// <summary>护盾被撞后的免疫时长:无敌帧。</summary>
        private const float HitImmuneSeconds = 0.8f;
        /// <summary>免疫截止时刻(Time.time 基准)。</summary>
        private float _immuneUntil;

        public override void OnEnter()
        {
            Debug.Log("[DescendingState] 下潜开始,钩子按 Config.DescentSpeed 下沉");
            EventBus.Instance.Subscribe<HookHitFishEvent>(OnHookHitFish);
        }

        /// <summary>每帧检测触底</summary>
        public override void OnUpdate(float dt)
        {
            GameManager gm = GameManager.Instance;
            if (gm == null || gm.Model == null || gm.Config == null) return;

            if (gm.Model.Depth >= (int)gm.Config.MaxDepth)
            {
                Machine.SwitchTo(GamePhase.Ascending);
            }
        }

        public override void OnExit()
        {
            if (EventBus.Instance != null) EventBus.Instance.Unsubscribe<HookHitFishEvent>(OnHookHitFish);
        }

        /// <summary>钩碰鱼</summary>
        private void OnHookHitFish(HookHitFishEvent evt)
        {
            if (Time.time < _immuneUntil) return;

            GameModel model = GameManager.Instance.Model;

            if (model.SkillLeft > 0)
            {
                model.UseSkill();
                _immuneUntil = Time.time + HitImmuneSeconds;
                EventBus.Instance.Post(new ToastEvent("护盾-1"));
                Debug.Log("[DescendingState] 护盾抵挡,剩余护盾 " + model.SkillLeft
                    + "/" + model.SkillTotal + ",继续下潜");
            }
            else
            {
                Debug.Log("[DescendingState] 护盾耗尽,立即回收");
                Machine.SwitchTo(GamePhase.Ascending);
            }
        }
    }
}