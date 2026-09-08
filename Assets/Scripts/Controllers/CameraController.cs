using FishingGame.Core;
using System.Collections;
using UnityEngine;

namespace FishingGame.Controllers
{
    /// <summary>视差背景层配置:Factor 为跟随系数(0=完全不动远景,1=完全跟随)。</summary>
    [System.Serializable]
    public class ParallaxLayer
    {
        public Transform Layer;
        [Range(0f,1f)] public float Factor = 0.4f;
    }

    /// <summary>镜头控制:跟随钩子下潜(死区+Lerp 平滑)、视差背景、按深度三段插值背景色、捕获屏震。</summary>
    public class CameraController : MonoBehaviour
    {
        public Transform Hook;
        public float FollowLerp = 5f;
        public float DeadZone = 2f;

        [Header("视差层(远景系数小=移动慢,拖入背景 Sprite")]
        [SerializeField] private ParallaxLayer[] _parallaxLayers;

        [Header("背景色三段插值( 0m / 50 / 100m )")]
        [SerializeField] private Color _shallowColor = new Color32(0x1a,0x6f,0xa8,0xff);
        [SerializeField] private Color _midColor = new Color32(0x0b,0x3d,0x5c,0xff);
        [SerializeField] private Color _deepColor = new Color32(0x02,0x0a,0x12,0xff);

        private Camera _camera;
        private Vector3 _startPos;
        private Vector3 _basePos;
        private Vector3 _shakeOffset;
        private Vector3[] _layerStartPos;
        private float _maxDepth = 100f;
        private bool _following;
        private bool _parked;

        private void Start()
        {
            _camera = GetComponent<Camera>();
            _startPos = transform.position;
            _basePos = _startPos;

            if(GameManager.Instance != null && GameManager.Instance.Config != null)
            {
                _maxDepth = GameManager.Instance.Config.MaxDepth;
            }

            if(_parallaxLayers != null)
            {
                _layerStartPos = new Vector3[_parallaxLayers.Length];
                for(int i = 0; i < _parallaxLayers.Length; i++)
                {
                    _layerStartPos[i] = _parallaxLayers[i].Layer != null
                        ? _parallaxLayers[i].Layer.position : Vector3.zero;
                }
            }

            EventBus.Instance.Subscribe<FishCaughtEvent>(OnFishCaught);
            EventBus.Instance.Subscribe<PhaseChangedEvent>(OnPhaseChanged);
        }

        private void OnDestroy()
        {
            if(EventBus.Instance != null) EventBus.Instance.Unsubscribe<FishCaughtEvent>(OnFishCaught);
            if(EventBus.Instance != null) EventBus.Instance.Unsubscribe<PhaseChangedEvent>(OnPhaseChanged);
        }

        /// <summary>捕获反馈:0.12秒随机抖动的协程屏震。</summary>
        private void OnFishCaught(FishCaughtEvent evt)
        {
            StartCoroutine(Shake(0.12f));
        }

        /// <summary>阶段响应:回开始/倒计时时停止跟随并归位,下一局重新从水面下跟。</summary>
        private void OnPhaseChanged(PhaseChangedEvent evt)
        {
            if(evt.Phase == GamePhase.Countdown || evt.Phase == GamePhase.Start)
            {
                _following = false;
                _basePos = _startPos;
                _parked = evt.Phase == GamePhase.Start;
            }
        }

        /// <summary>镜头主循环:死区出界才开始跟随→Lerp 平滑→叠加屏震偏移→视差层按系数位移→深度三段背景色插值。</summary>
        private void LateUpdate()
        {
            if(Hook == null || _camera == null) return;

            float targetY = Mathf.Min(_startPos.y, Hook.position.y);
            if(!_parked && !_following && targetY < _basePos.y - DeadZone) _following = true;
            if(_following)
            {
                _basePos.y = Mathf.Lerp(_basePos.y,targetY,FollowLerp * Time.deltaTime);
            }
            transform.position = _basePos + _shakeOffset;

            if(_parallaxLayers != null)
            {
                float camDelta = _basePos.y - _startPos.y;
                for(int i = 0; i < _parallaxLayers.Length; i++)
                {
                    ParallaxLayer pl = _parallaxLayers[i];
                    if(pl.Layer == null) continue;
                    Vector3 lp = _layerStartPos[i];
                    lp.y += camDelta * pl.Factor;
                    pl.Layer.position = lp;
                }
            }

            float t = Mathf.Clamp01(-Hook.position.y / _maxDepth);
            Color target;
            if(t < 0.5f) target = Color.Lerp(_shallowColor,_midColor,t * 2f);
            else target = Color.Lerp(_midColor,_deepColor,(t - 0.5f) * 2f);
            _camera.backgroundColor = target;
        }

        private IEnumerator Shake(float seconds)
        {
            float t = 0f;
            while(t < seconds)
            {
                t += Time.deltaTime;
                _shakeOffset = new Vector3(
                    Random.Range(-0.08f,0.08f),
                    Random.Range(-0.08f,0.08f),
                    0f);
                yield return null;
            }
            _shakeOffset = Vector3.zero;
        }
    }
}