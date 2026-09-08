using System;
using System.Collections.Generic;

namespace FishingGame.Models
{
    public class GameModel
    {
        private readonly GameConfig _config;
        private readonly Dictionary<FishDef, int> _catchDict = new Dictionary<FishDef, int>();
        private int _catchCount;

        /// <summary>当前深度(米,整数,SetDepth 钳制在 [0, MaxDepth])。</summary>
        public int Depth { get; private set; }

        /// <summary>本局达到过的最深深度。</summary>
        public int MaxDepthReached { get; private set; }

        /// <summary>剩余技能次数。</summary>
        public int SkillLeft { get; private set; }

        /// <summary>技能总次数(来自配置)。</summary>
        public int SkillTotal => _config.SkillCount;

        /// <summary>本局金币累计。</summary>
        public int Coin { get; private set; }

        /// <summary>本局贝壳累计。</summary>
        public int Shell { get; private set; }

        /// <summary>捕获容量(来自配置)。</summary>
        public int CatchCapacity => _config.CatchCapacity;

        /// <summary>当前捕获总条数(O(1) 计数器,无 LINQ 分配)。</summary>
        public int CatchCount => _catchCount;

        /// <summary>捕获数是否已达容量。</summary>
        public bool IsFull => _catchCount >= _config.CatchCapacity;

        /// <summary>按鱼种分组的捕获统计(只读视图,结算遍历用)。</summary>
        public IReadOnlyDictionary<FishDef, int> CatchDict => _catchDict;

        /// <summary>深度变化(米)。仅值变化时触发——整数深度天然节流,每帧重复写同值不广播。</summary>
        public event Action<int> DepthChanged;

        /// <summary>捕获一条鱼。</summary>
        public event Action<FishDef> FishCaught;

        /// <summary>捕获计数变化 (当前数, 容量)。</summary>
        public event Action<int, int> CatchCountChanged;

        /// <summary>技能次数变化 (剩余, 总数)。</summary>
        public event Action<int, int> SkillCountChanged;

        public GameModel(GameConfig config)
        {
            _config = config;
            SkillLeft = config.SkillCount;
        }

        /// <summary>写入当前深度。自动钳制到 [0, MaxDepth];值未变化时不广播(节流)。</summary>
        /// <param name="depth">目标深度(米)。</param>
        public void SetDepth(int depth)
        {
            int max = (int)_config.MaxDepth;
            int clamped = depth < 0 ? 0 : (depth > max ? max : depth);
            if (clamped == Depth) return;

            Depth = clamped;
            if (clamped > MaxDepthReached) MaxDepthReached = clamped;
            DepthChanged?.Invoke(clamped);
        }

        /// <summary>添加一条捕获记录。</summary>
        /// <param name="def">鱼种定义。</param>
        public void AddCatch(FishDef def)
        {
            _catchDict[def] = _catchDict.TryGetValue(def, out int n) ? n + 1 : 1; 
            _catchCount++;
            Coin += def.CoinReward;
            Shell += def.ShellReward;

            FishCaught?.Invoke(def);
            CatchCountChanged?.Invoke(_catchCount, CatchCapacity);
        }

        /// <summary>消耗一次技能。次数不足时不做任何事。</summary>
        public void UseSkill()
        {
            if (SkillLeft <= 0) return;

            SkillLeft--;
            SkillCountChanged?.Invoke(SkillLeft, _config.SkillCount);
        }

        /// <summary>生成本局结算快照。内部字典为拷贝,与后续 Model 变化隔离。</summary>
        public RunResult Snapshot()
        {
            return new RunResult(MaxDepthReached, new Dictionary<FishDef, int>(_catchDict), Coin, Shell);
        }

        /// <summary>单局重置唯一入口。清空捕获字典、深度、技能、货币,并广播归零事件让 HUD 自动复位。</summary>
        public void Reset()
        {
            _catchDict.Clear();
            _catchCount = 0;
            Depth = 0;
            MaxDepthReached = 0;
            SkillLeft = _config.SkillCount;
            Coin = 0;
            Shell = 0;

            DepthChanged?.Invoke(0);
            CatchCountChanged?.Invoke(0, _config.CatchCapacity);
            SkillCountChanged?.Invoke(SkillLeft, _config.SkillCount);
        }
    }
}