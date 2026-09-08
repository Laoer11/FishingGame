using FishingGame.Models;
using UnityEngine;

namespace FishingGame.Controllers
{
    /// <summary>静态门面</summary>
    public static class FishFactory
    {
        private static FishPool _pool;

        /// <summary>初始化底层池,传入鱼预制体</summary>
        public static void Init(GameObject prefab)
        {
            if(_pool == null) _pool = new FishPool(prefab);
        }

        public static FishController Spawn(FishDef def, Vector2 pos)
        {
            if(_pool == null) return null;
            return _pool.Spawn(def, pos);
        }

        public static void DespawnAll()
        {
            _pool?.DespawnAll();
        }

        /// <summary>调试入口:打印各鱼种活跃/池内数量(P 键触发),验证池化是否生效。</summary>
        public static string GetDebugInfo()
        {
            return _pool != null ? _pool.GetDebugInfo() : "[FishFactory] 池未初始化";
        }

    }
}