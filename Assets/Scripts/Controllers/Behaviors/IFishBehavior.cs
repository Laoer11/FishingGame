namespace FishingGame.Controllers
{
    /// <summary>鱼游动行为策略接口。
    /// FishController.Update 每帧调用,鱼种行为由 Def.Behavior 决定挂哪个实现。</summary>
    public interface IFishBehavior
    {
        /// <summary>推进一帧游动。fish:宿主鱼,dt:帧间隔。</summary>
        void Tick(FishController fish,float dt);
    }
}