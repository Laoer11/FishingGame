using System.Collections.Generic;

namespace FishingGame.Models
{
    /// <summary>单局结算快照(不可变)。由 GameModel.Snapshot() 生成携带字典拷贝,与后续重置隔离。
    /// 随 RoundEndEvent 广播,ResultView 据此渲染结算面板。</summary>
    public class RunResult
    {
        /// <summary>本局达到的最深深度(米)。</summary>
        public int Depth { get; }

        /// <summary>本局捕获的鱼，分类统计。</summary>
        public IReadOnlyDictionary<FishDef,int> Catches { get; }

        /// <summary>本局获得的金币奖励。</summary>
        public int CoinTotal { get; }

        /// <summary>本局获得的贝壳奖励。</summary>
        public int ShellTotal { get; }

        /// <summary>本局捕获的鱼总数。</summary>
        public int TotalCatch { get; }

        public RunResult(int depth, Dictionary<FishDef,int> catches, int coinTotal, int shellTotal)
        {
            Depth = depth;
            Catches = catches;
            CoinTotal = coinTotal;
            ShellTotal = shellTotal;
            // 构造时一次性求和,避免外部每次访问 TotalCatch 都遍历字典
            int sum = 0;
            foreach(int n in catches.Values) sum += n;
            TotalCatch = sum;
        }

    }
}