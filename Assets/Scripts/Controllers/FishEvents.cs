using UnityEngine;

namespace FishingGame.Controllers
{
    /// <summary>钩子碰鱼事件(扣盾或捕获)</summary>
    public readonly struct HookHitFishEvent
    {
        /// <summary>被碰到的鱼实例</summary>
        public readonly FishController Fish;
        /// <summary>捕获挂载点</summary>
        public readonly Transform CatchPoint;

        public HookHitFishEvent(FishController fish, Transform catchPoint)
        {
            Fish = fish;
            CatchPoint = catchPoint;
        }
    }
}