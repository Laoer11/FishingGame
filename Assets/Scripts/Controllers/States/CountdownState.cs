// CountdownState: 倒计时状态(第 4 章重点实现)。进入时清零计时,逐秒广播
// CountdownTickEvent(3→2→1→0,0 表示 GO),约 3.8 秒后切到 Descending。
// 计时真相在状态内,CountdownView 只订阅事件播动画——单一真相原则。
using FishingGame.Core;

namespace FishingGame.States
{
    public class CountdownState : GameState
    {
        /// <summary>每个数字持续秒数</summary>
        private const float StepSeconds = 1f;
        /// <summary>GO 画面停留时长</summary>
        private const float GoHoldSeconds = 0.8f;
        /// <summary>起始数字(3→2→1→GO)</summary>
        private const int StartNumber = 3;

        private float _timer;
        /// <summary>已播报的最近数字,用于检测整数递减(3→2→1→0)。</summary>
        private int _lastNumber;

        public CountdownState(GameStateMachine machine) : base(machine) { }

        public override GamePhase Phase => GamePhase.Countdown;

        public override void OnEnter()
        {
            _timer = 0f;
            _lastNumber = StartNumber + 1;
        }

        /// <summary>累加计时并推导当前应显示的数字;换数才广播,避免每帧重复播报。</summary>
        public override void OnUpdate(float dt)
        {
            _timer += dt;

            // 数字推导:耗时每满1秒数字递减1(3→2→1→0=GO)
            int number = StartNumber - (int)(_timer / StepSeconds);
            if(number < _lastNumber && number >= 0)
            {
                _lastNumber = number;
                EventBus.Instance.Post(new CountdownTickEvent(number));
            }

            // 3个数播完+GO停留后进入下潜(3×1s + 0.8s = 3.8s)
            if(_timer >= StartNumber * StepSeconds + GoHoldSeconds)
            {
                Machine.SwitchTo(GamePhase.Descending);
            }
        }

        public override void OnExit() {  }
    }
}