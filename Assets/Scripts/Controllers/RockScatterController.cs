using System.Collections.Generic;
using FishingGame.Core;
using UnityEngine;

namespace FishingGame.Controllers
{
    /// <summary>背景岩石散布器:Countdown 时沿 0~-MaxDepth 在相机左右边缘随机摆放,露出30%~70%,纯背景无碰撞。</summary>
    public class RockScatterController : MonoBehaviour
    {
        [SerializeField] private GameObject[] _rockPrefabs;
        [SerializeField] private int _count = 16;
        [SerializeField] private float _visibleMin = 0.3f;
        [SerializeField] private float _visibleMax = 0.7f;
        [SerializeField] private float _yJitter = 2.5f;
        [SerializeField] private float _startDepth = 15f;
        [SerializeField] private int _sortOrder = 0;

        private readonly List<GameObject> _rocks = new List<GameObject>();
        private Camera _camera;
        private float _maxDepth = 100f;

        private void Awake()
        {
            _camera = Camera.main;
            if(GameManager.Instance != null && GameManager.Instance.Config != null)
            {
                _maxDepth = GameManager.Instance.Config.MaxDepth;
            }
        }

        private void OnEnable()
        {
            EventBus.Instance.Subscribe<PhaseChangedEvent>(OnPhaseChanged);
        }

        private void OnDisable()
        {
            if(EventBus.Instance != null) EventBus.Instance.Unsubscribe<PhaseChangedEvent>(OnPhaseChanged);
        }

        private void OnPhaseChanged(PhaseChangedEvent evt)
        {
            if(evt.Phase != GamePhase.Countdown) return;
            Scatter();
        }

        /// <summary>布石:预制的岩体在屏幕左右边缘各露一部分(visible 30%~70%),Y 均匀铺满深度带加随机抖动。</summary>
        private void Scatter()
        {
            Clear();

            if(_rockPrefabs == null || _rockPrefabs.Length == 0 || _camera == null)
            {
                Debug.LogError("[RockScatter] RockPrefabs 为空或相机缺失,请在 Inspector 赋值");
                return;
            }

            float halfW = _camera.orthographicSize * _camera.aspect;

            for(int i = 0; i < _count; i++)
            {
                GameObject prefab = _rockPrefabs[Random.Range(0, _rockPrefabs.Length)];
                GameObject rock = Instantiate(prefab, transform);

                SpriteRenderer sr = rock.GetComponentInChildren<SpriteRenderer>();
                if(sr == null)
                {
                    Destroy(rock);
                    continue;
                }
                sr.sortingOrder = _sortOrder;

                float width = sr.bounds.size.x;
                float visible = Random.Range(_visibleMin, _visibleMax);
                bool leftSide = Random.value < 0.5f;

                float x = leftSide ? -halfW + width * visible * 0.5f
                                   : halfW - width * visible * 0.5f;

                float y = -(_startDepth + (_maxDepth - _startDepth) * ((i + 0.5f) / _count))
                          + Random.Range(-_yJitter, _yJitter);
                rock.transform.position = new Vector3(x, y, 0f);

                _rocks.Add(rock);
            }
        }

        /// <summary>清场:销毁上一局所有岩石实例(岩石未池化,数量少直接 Destroy 可接受)。</summary>
        private void Clear()
        {
            for(int i = 0; i < _rocks.Count; i++)
            {
                if(_rocks[i] != null) Destroy(_rocks[i]);
            }
            _rocks.Clear();
        }
    }
}