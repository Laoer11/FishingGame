namespace FishingGame.Core
{
    /// <summary>游戏状态抽象基类。</summary>
    public abstract class GameState
    {
        /// <summary>所属状态机。子类通过 Machine.SwitchTo 切换到下一阶段。</summary>
        protected readonly GameStateMachine Machine;

        /// <summary>注入状态机。约定:子类构造函数必须以 : base(machine) 传递。</summary>
        protected GameState(GameStateMachine machine)
        {
            Machine = machine;
        }

        /// <summary>该状态对应的游戏阶段,注册进状态机时的键,切换时据此广播 PhaseChangedEvent。</summary>
        public abstract GamePhase Phase { get; }

        /// <summary>进入本阶段时调用一次。</summary>
        public abstract void OnEnter();

        /// <summary>本阶段内每帧调用,dt 为帧间隔(秒)。</summary>
        public abstract void OnUpdate(float dt);

        /// <summary>离开本阶段时调用一次,负责清理。</summary>
        public abstract void OnExit();
    }
}