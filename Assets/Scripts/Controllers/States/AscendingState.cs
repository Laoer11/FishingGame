// AscendingState: 上升收线阶段。主动撞鱼即捕获;满 10 条切高速收线(速度由
// HookController 读 Model.IsFull 决定)。与 DescendingState 订阅同一
// HookHitFishEvent 但语义不同:捕获+继续——状态模式价值的核心例证。
// 深度回到 0m 切 Settlement(结算快照由 SettlementState 组装广播)。
using FishingGame.Controllers;
using FishingGame.Core;
using FishingGame.Models;
using UnityEngine;

namespace FishingGame.States
{
    public class AscendingState : GameState
    {
        public AscendingState(GameStateMachine machine) : base(machine) { }

        public override GamePhase Phase => GamePhase.Ascending;

        public override void OnEnter()
        {
            Debug.Log("[AscendingState] 收线开始,速度 = Config.AscentSpeed");
            EventBus.Instance.Subscribe<HookHitFishEvent>(OnHookHitFish);
        }

        /// <summary>每帧检测回到水面</summary>
        public override void OnUpdate(float dt)
        {
            GameModel model = GameManager.Instance != null ? GameManager.Instance.Model : null;
            if (model == null) return;

            if (model.Depth <= 0)
            {
                Machine.SwitchTo(GamePhase.Settlement);
            }
        }

        public override void OnExit()
        {
            if (EventBus.Instance != null) EventBus.Instance.Unsubscribe<HookHitFishEvent>(OnHookHitFish);
        }

        /// <summary>钩碰鱼</summary>
        private void OnHookHitFish(HookHitFishEvent evt)
        {
            GameModel model = GameManager.Instance.Model;
            if (model.IsFull) return;

            evt.Fish.OnCaught(evt.CatchPoint);
            model.AddCatch(evt.Fish.Def);
            Debug.Log("[AscendingState] 捕获 " + evt.Fish.Def.DisplayName
                + ",进度 " + model.CatchCount + "/" + model.CatchCapacity);

            if (model.IsFull)
            {
                EventBus.Instance.Post(new ToastEvent("收网!"));
                Debug.Log("[AscendingState] 满载,切高速收线 "
                    + GameManager.Instance.Config.FastAscentSpeed + "m/s");
            }
        }
    }
}
