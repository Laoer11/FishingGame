// TutorialState: 教学阶段。首局由组合根(GameManager)切入,停机等待 TutorialView
// 演示完毕;非首局组合根直入 Countdown。唯一出口 TutorialDoneCommand(X/最后
// 一步),收到后落 PlayerPrefs 并切倒计时——View 无状态机依赖,状态机不碰 UI。
// 红线:OnEnter 内不切状态(嵌套 SwitchTo 会导致事件乱序)。
using FishingGame.Core;
using UnityEngine;

namespace FishingGame.States
{
    public class TutorialState : GameState
    {
        private const string DoneKey = "tutorial_done";

        public TutorialState(GameStateMachine machine) : base(machine) { }

        public override GamePhase Phase => GamePhase.Tutorial;

        public override void OnEnter()
        {
            Debug.Log("[TutorialState] 首局教学,等待面板完成(玩家控制节奏,无自动推进)");
            EventBus.Instance.Subscribe<TutorialDoneCommand>(OnTutorialDone);
        }

        public override void OnUpdate(float dt)
        {
        }

        public override void OnExit()
        {
            if(EventBus.Instance != null) EventBus.Instance.Unsubscribe<TutorialDoneCommand>(OnTutorialDone);
        }

        private void OnTutorialDone(TutorialDoneCommand evt)
        {
            PlayerPrefs.SetInt(DoneKey, 1);
            PlayerPrefs.Save();
            Machine.SwitchTo(GamePhase.Countdown);
        }
    }
}