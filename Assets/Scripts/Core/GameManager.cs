using FishingGame.Models;
using FishingGame.States;
using UnityEngine;

namespace FishingGame.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        /// <summary>全局数值配置,Inspector 拖 GameConfig.asset。</summary>
        public GameConfig Config;

        /// <summary>运行时数据源,Awake 中创建。</summary>
        public GameModel Model { get; private set; }

        /// <summary>游戏状态机(组合根:五个具体状态在此注册,Core 层不依赖玩法)。</summary>
        public GameStateMachine Machine { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Application.targetFrameRate = 60;

            if (Config == null)
            {
                Debug.LogError("[GameManager] 未拖 GameConfig.asset,请在 Inspector 赋值后重跑");
                return;
            }
            Model = new GameModel(Config);

            Machine = new GameStateMachine();
            Machine.Register(new StartState(Machine));
            Machine.Register(new TutorialState(Machine));
            Machine.Register(new CountdownState(Machine));
            Machine.Register(new DescendingState(Machine));
            Machine.Register(new AscendingState(Machine));
            Machine.Register(new SettlementState(Machine));
        }

        private void Start()
        {
            Model.DepthChanged += OnModelDepthChanged;
            Model.FishCaught += OnModelFishCaught;
            Model.CatchCountChanged += OnModelCatchCountChanged;
            Model.SkillCountChanged += OnModelSkillCountChanged;

            EventBus.Instance.Subscribe<ResetCommand>(OnResetCommand);
            EventBus.Instance.Subscribe<ExitToStartCommand>(OnExitToStart);

            // 初始态:开始界面(首局教学判断移至 StartState,入场到位后再切,避免教学面板在开始界面下偷跑)
            Machine.SwitchTo(GamePhase.Start);
        }

        private void Update()
        {
            Machine?.Tick(Time.deltaTime);
        }
        
        /// <summary>成对退订,防止单例与静态引用导致的泄漏。</summary>
        private void OnDestroy()
        {
            if (Model != null)
            {
                Model.DepthChanged -= OnModelDepthChanged;
                Model.FishCaught -= OnModelFishCaught;
                Model.CatchCountChanged -= OnModelCatchCountChanged;
                Model.SkillCountChanged -= OnModelSkillCountChanged;
            }
            if (EventBus.Instance != null)
            {
                EventBus.Instance.Unsubscribe<ResetCommand>(OnResetCommand);
                EventBus.Instance.Unsubscribe<ExitToStartCommand>(OnExitToStart);
            }
        }

        // ---- Model → EventBus 桥接(数据变更的广播通道) ----

        private void OnModelDepthChanged(int depth)
        {
            EventBus.Instance.Post(new DepthChangedEvent(depth));
        }

        private void OnModelFishCaught(FishDef def)
        {
            EventBus.Instance.Post(new FishCaughtEvent(def));
        }

        private void OnModelCatchCountChanged(int current, int capacity)
        {
            EventBus.Instance.Post(new CatchCountChangedEvent(current, capacity));
        }

        private void OnModelSkillCountChanged(int left, int total)
        {
            EventBus.Instance.Post(new SkillCountChangedEvent(left, total));
        }

        /// <summary>暂停/结算面板的"重开":清空数据并直接进入倒计时(跳过开始界面)。</summary>
        private void OnResetCommand(ResetCommand evt)
        {
            Model.Reset();
            Machine.SwitchTo(GamePhase.Countdown);
        }

        /// <summary>结算/暂停面板的"退出":清空数据并回到开始界面。</summary>
        private void OnExitToStart(ExitToStartCommand evt)
        {
            Model.Reset();
            Machine.SwitchTo(GamePhase.Start);
        }
    }
}
