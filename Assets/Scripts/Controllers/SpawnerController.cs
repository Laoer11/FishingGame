using System.Collections.Generic;
using FishingGame.Core;
using FishingGame.Models;
using UnityEngine;

namespace FishingGame.Controllers
{
    /// <summary>布场器:监听阶段事件,倒计时开始时按鱼种深度带随机生成整场鱼;
    /// 每帧做视口裁剪(视野外鱼休眠省 Update 开销)。生成数量由 TotalCount 在 Inspector 决定。</summary>
    public class SpawnerController : MonoBehaviour
    {
        public List<FishDef> FishDefs;
        public GameObject FishPrefab;
        /// <summary>每场生成总数。</summary>
        public int TotalCount = 40;
        /// <summary>同屏活跃上限(备用参数,当前未做同屏限流)。</summary>
        public int MaxOnScreen = 15;

        /// <summary>视口裁剪的纵向余量(米)</summary>
        [SerializeField] private float _viewMargin = 3f;

        private readonly List<FishController> _spawned = new List<FishController>();
        private Camera _camera;

        private void Awake()
        {
            _camera = Camera.main;
            FishFactory.Init(FishPrefab);
        }

        private void OnEnable()
        {
            EventBus.Instance.Subscribe<PhaseChangedEvent>(OnPhaseChanged);
        }

        private void OnDisable()
        {
            if(EventBus.Instance != null) EventBus.Instance.Unsubscribe<PhaseChangedEvent>(OnPhaseChanged);
        }

        /// <summary>阶段响应:回开始界面→全清;进倒计时→全清后重新布场(每局鱼种分布独立)。</summary>
        private void OnPhaseChanged(PhaseChangedEvent evt)
        {
            if(evt.Phase == GamePhase.Start)
            {
                FishFactory.DespawnAll();
                _spawned.Clear();
                return;
            }

            if(evt.Phase != GamePhase.Countdown) return;

            FishFactory.DespawnAll();
            _spawned.Clear();
            SpawnAll();
        }

        /// <summary>随机布场:每条鱼随机选种→随机深度落在该鱼种的深度带内→随机横向位置(留 1m 边距防贴边)。</summary>
        private void SpawnAll()
        {
            if (FishDefs == null || FishDefs.Count == 0 || _camera == null)
            {
                Debug.LogError("[Spawner] FishDefs 列表为空或相机缺失,请在 Inspector 赋值");
                return;
            }

            float halfWidth = _camera.orthographicSize * _camera.aspect;
            for (int i = 0; i < TotalCount; i++)
            {
                FishDef def = FishDefs[Random.Range(0, FishDefs.Count)];
                float depth = Random.Range(def.MinDepth, def.MaxDepth);
                float x = Random.Range(-halfWidth + 1f, halfWidth - 1f);
                FishController fish = FishFactory.Spawn(def, new Vector2(x, -depth));
                if (fish != null) _spawned.Add(fish);
            }
            Debug.Log("[Spawner] 布场完成,共 " + _spawned.Count + " 条 / " + FishDefs.Count + " 种");
        }

        /// <summary>视口裁剪:视野外(含余量)的鱼 SetActive(false)休眠,回视野唤醒——手写行为级 LOD。
        /// P 键打印对象池状态(验证池化效果)。</summary>
        private void Update()
        {
            if (_camera == null) return;

            float camY = _camera.transform.position.y;
            float halfH = _camera.orthographicSize + _viewMargin;
            for (int i = 0; i < _spawned.Count; i++)
            {
                FishController f = _spawned[i];
                if (f == null || f.IsCaught) continue;

                bool inView = Mathf.Abs(f.transform.position.y - camY) <= halfH;
                if (inView != f.gameObject.activeSelf) f.gameObject.SetActive(inView);
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                Debug.Log("[Spawner] 对象池状态(P键):\n" + FishFactory.GetDebugInfo());
            }
        }
    }
}
