using System.Text;
using FishingGame.Models;
using System.Collections.Generic;
using UnityEngine;

namespace FishingGame.Controllers
{
    /// <summary>鱼对象池:按鱼种分桶缓存,避免反复 Instantiate/Destroy 造成的 GC 尖峰。</summary>
    public class FishPool
    {
        private readonly GameObject _prefab;
        /// <summary>鱼种 → 待复用实例队列</summary>
        private readonly Dictionary<FishDef, Queue<FishController>> _buckets = new Dictionary<FishDef, Queue<FishController>>();
        /// <summary>当前活跃(在场)的鱼,回收与全清时遍历用。</summary>
        private readonly List<FishController> _active = new List<FishController>();

        public FishPool(GameObject prefab)
        {
            _prefab = prefab;
        }

        /// <summary>取一条鱼:桶里有则复用(先清脏状态),没有才实例化。之后统一SetUp重配+激活。</summary>
        public FishController Spawn(FishDef def, Vector2 pos)
        {
            FishController fish;
            if (_buckets.TryGetValue(def, out Queue<FishController> bucket) && bucket.Count > 0)
            {
                fish = bucket.Dequeue();
                fish.PrepareDespawn();
            }
            else
            {
                GameObject go = Object.Instantiate(_prefab, pos, Quaternion.identity);
                fish = go.GetComponent<FishController>();
            }

            fish.transform.position = pos;
            fish.SetUp(def);
            fish.gameObject.SetActive(true);
            _active.Add(fish);
            return fish;
        }

        /// <summary>回收入桶:脱离活跃列表→清状态→隐藏→按鱼种入桶。非活跃实例直接忽略(防重复回收)。</summary>
        public void Despawn(FishController fish)
        {
            if (fish == null || !_active.Contains(fish)) return;

            _active.Remove(fish);
            FishDef def = fish.Def;
            fish.PrepareDespawn();
            fish.gameObject.SetActive(false);

            if (!_buckets.TryGetValue(def, out Queue<FishController> bucket))
            {
                bucket = new Queue<FishController>();
                _buckets[def] = bucket;
            }
            bucket.Enqueue(fish);
        }

        /// <summary>倒序遍历回收全部(倒序防遍历中移除导致跳元素)。</summary>
        public void DespawnAll()
        {
            for (int i = _active.Count - 1; i >= 0; i--)
            {
                Despawn(_active[i]);
            }
        }

        public string GetDebugInfo()
        {
            var counts = new Dictionary<FishDef, int>();
            foreach (FishController f in _active)
            {
                counts[f.Def] = counts.TryGetValue(f.Def, out int n) ? n + 1 : 1;
            }
            var sb = new StringBuilder();
            foreach (var pair in _buckets)
            {
                int act = counts.TryGetValue(pair.Key, out int n) ? n : 0;
                sb.AppendLine(pair.Key.name + ": 活跃 " + act + " / 池内 " + pair.Value.Count);
            }
            sb.AppendLine("总活跃 " + _active.Count);
            return sb.ToString();
        }

    }
}