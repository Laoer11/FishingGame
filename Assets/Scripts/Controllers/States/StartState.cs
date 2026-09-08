using FishingGame.Core;
using UnityEngine;

namespace FishingGame.States
{
    /// <summary>开始界面阶段。静态等待玩家点开始→玩家入场→到位后按"是否看过教学"分流。</summary>
    public class StartState : GameState
    {
        public StartState(GameStateMachine machine) : base(machine) { }

        public override GamePhase Phase => GamePhase.Start;

        /// <summary>订阅玩家入场到位事件</summary>
        public override void OnEnter()
        {
            Debug.Log("[StartState] 开始界面,等待玩家点击开始");
            EventBus.Instance.Subscribe<PlayerArrivedEvent>(OnPlayerArrived);
        }

        public override void OnUpdate(float dt)
        {
        }

        public override void OnExit()
        {
            if(EventBus.Instance != null) EventBus.Instance.Unsubscribe<PlayerArrivedEvent>(OnPlayerArrived);
        }

        /// <summary>玩家走到码头后</summary>
        private void OnPlayerArrived(PlayerArrivedEvent evt)
        {
            Machine.SwitchTo(PlayerPrefs.GetInt("tutorial_done", 0) == 1
                ? GamePhase.Countdown : GamePhase.Tutorial);
        }
    }
}