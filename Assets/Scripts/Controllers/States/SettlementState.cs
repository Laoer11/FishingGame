// SettlementState: 结算阶段。OnEnter 组装 RunResult 快照并广播 RoundEndEvent,
// 弹面板等玩家选择。不写重置逻辑——重置统一走 ResetCommand 链路(红线 7):
// 「继续体验」→ Model.Reset() + Spawner.DespawnAll() + 切 Countdown。
using FishingGame.Core;
using UnityEngine;

namespace FishingGame.States
{
    public class SettlementState : GameState
    {
        public SettlementState(GameStateMachine machine) : base(machine) { }

        public override GamePhase Phase => GamePhase.Settlement;

        public override void OnEnter()
        {
            Debug.Log("[SettlementState] 结算开始");
            EventBus.Instance.Post(new RoundEndEvent(GameManager.Instance.Model.Snapshot()));
        }

        public override void OnUpdate(float dt) { }

        public override void OnExit() { }
    }
}
