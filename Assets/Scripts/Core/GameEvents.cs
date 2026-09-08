using FishingGame.Models;

namespace FishingGame.Core
{
    /// <summary>游戏阶段</summary>
    public enum GamePhase
    {
        /// <summary>开始界面阶段</summary>
        Start,
        /// <summary>教程阶段</summary>
        Tutorial,
        /// <summary>倒计时阶段</summary>
        Countdown,
        /// <summary>下降阶段</summary>
        Descending,
        /// <summary>上升阶段</summary>
        Ascending,
        /// <summary>结算阶段</summary>
        Settlement
    }

    /// <summary>游戏阶段改变事件</summary>
    public readonly struct PhaseChangedEvent
    {
        public readonly GamePhase Phase;
        public PhaseChangedEvent(GamePhase phase) { Phase = phase; }
    }

    /// <summary>倒计时数字变化事件。Number: 3/2/1,0 表示 GO。</summary>
    public readonly struct CountdownTickEvent
    {
        public readonly int Number;
        public CountdownTickEvent(int number) { Number = number; }
    }

    /// <summary>游戏深度改变事件</summary>
    public readonly struct DepthChangedEvent
    {
        public readonly int Depth;
        public DepthChangedEvent(int depth) { Depth = depth; }
    }

    /// <summary>捕获鱼事件</summary>
    public readonly struct FishCaughtEvent
    {
        public readonly FishDef Def;
        public FishCaughtEvent(FishDef def) { Def = def; }
    }

    /// <summary>捕获数量改变事件</summary>
    public readonly struct CatchCountChangedEvent
    {
        public readonly int Current;
        public readonly int Capacity;
        public CatchCountChangedEvent(int current, int capacity) { Current = current; Capacity = capacity; }
    }

    /// <summary>护盾(技能)剩余次数改变事件。由 GameManager 发出,HUD/护盾气泡/音效订阅刷新。</summary>
    public readonly struct SkillCountChangedEvent
    {
        public readonly int Left;
        public readonly int Total;
        public SkillCountChangedEvent(int left, int total) { Left = left; Total = total; }
    }

    /// <summary>单局结束事件。由 SettlementState 发出携带结算数据,ResultView/ AudioManager 订阅。</summary>
    public readonly struct RoundEndEvent
    {
        public readonly RunResult Result;
        public RoundEndEvent(RunResult result) { Result = result; }
    }

    /// <summary>Toast 提示事件。Message: 提示文本,显示后自动淡出。</summary>
    public readonly struct ToastEvent
    {
        public readonly string Message;
        public ToastEvent(string message) { Message = message; }
    }

    /// <summary>教学完成命令。由 TutorialView(X 或最后一步)发出,TutorialState 收到后落 PlayerPrefs 并切 Countdown。</summary>
    public readonly struct TutorialDoneCommand { }

    /// <summary>开始游戏命令。由 StartView 的开始按钮发出,PlayerIntroController 收到后开始入场。</summary>
    public readonly struct StartCommand { }

    /// <summary>玩家入场到位事件。由 PlayerIntroController 在走到码头后发出,StartState 收到后再等 0.5 秒切教学/倒计时。</summary>
    public readonly struct PlayerArrivedEvent { }

    /// <summary>退回开始界面命令。由 ResultView/PauseView 的退出按钮发出,GameManager 收到后重置数据并切回 Start 阶段。</summary>
    public readonly struct ExitToStartCommand { }

    /// <summary>重置命令</summary>
    public readonly struct ResetCommand { }

    /// <summary>请求技能命令</summary>
    public readonly struct SkillRequestCommand { }

    /// <summary>暂停命令</summary>
    public readonly struct PauseCommand { }

    /// <summary>恢复命令</summary>
    public readonly struct ResumeCommand { }

}