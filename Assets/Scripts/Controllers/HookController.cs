using FishingGame.Core;
using UnityEngine;

namespace FishingGame.Controllers
{
    /// <summary>钩子控制器:阶段驱动升降(速度全部来自 Config)、水平拖拽、碰撞转发。</summary>
    public class HookController : MonoBehaviour
    {
        /// <summary>船底挂点(Inspector 拖,鱼线起点)。</summary>
        public Transform RopeAnchor;

        /// <summary>鱼获挂载点(钩下方,第 6 章鱼成串用)。</summary>
        public Transform CatchPoint;

        /// <summary>水平钳制的屏幕边距(约等于钩半径)。</summary>
        [SerializeField] private float _xMargin = 0.3f;

        /// <summary>水面世界坐标 Y(与 WaterSurface 物体 Y 一致,钩越过此线时播入水音)。</summary>
        [SerializeField] private float _waterLineY = 0f;

        private IInputService _input;
        private Vector3 _startPos;
        private bool _diving;
        private bool _ascending;
        private float _maxDepth = 100f;
        private float _descentSpeed = 6f;
        private float _ascentSpeed = 6f;
        private float _fastAscentSpeed = 24f;
        private float _pendingDragX;
        private bool _enteredWater;

        /// <summary>输入与配置拉取:速度四项来自 GameConfig;水平拖拽来自 InputBootstrap 的服务。</summary>
        private void Start()
        {
            _startPos = transform.position;

            if(GameManager.Instance != null && GameManager.Instance.Config  != null)
            {
                _maxDepth = GameManager.Instance.Config.MaxDepth;
                _descentSpeed = GameManager.Instance.Config.DescentSpeed;
                _ascentSpeed = GameManager.Instance.Config.AscentSpeed;
                _fastAscentSpeed = GameManager.Instance.Config.FastAscentSpeed;
            }
            else
            {
                Debug.LogError("[HookController] GameManager 未就绪或未拖 GameConfig,速度退回默认值");
            }

            InputBootstrap bootstrap = FindObjectOfType<InputBootstrap>();
            if(bootstrap != null && bootstrap.Service != null)
            {
                _input = bootstrap.Service;
                _input.OnDragDelta += OnDragDelta;
            }
            else
            {
                Debug.LogError("[HookController] 场景中找不到 InputBootstrap,水平拖拽不可用");
            }
            EventBus.Instance.Subscribe<PhaseChangedEvent>(OnPhaseChanged);
        }

        private void OnDestroy()
        {
            if(_input != null) _input.OnDragDelta -= OnDragDelta;
            if(EventBus.Instance != null) EventBus.Instance.Unsubscribe<PhaseChangedEvent>(OnPhaseChanged);
        }

        private void OnDragDelta(Vector2 delta)
        {
            _pendingDragX += delta.x;
        }

        /// <summary>阶段响应:开启下潜/上升标志;倒计时/回开始时钩子归位并清零深度。拖拽累积量同步清零防阶段间残留。</summary>
        private void OnPhaseChanged(PhaseChangedEvent evt)
        {
            _diving = evt.Phase == GamePhase.Descending;
            _ascending = evt.Phase == GamePhase.Ascending;
            _pendingDragX = 0f;
            _enteredWater = false;

            if(evt.Phase == GamePhase.Countdown || evt.Phase == GamePhase.Start)
            {
                transform.position = _startPos;
                GameManager.Instance.Model.SetDepth(0);
            }
        }

        /// <summary>主循环:下潜(触底钳制+首次入水播音效)/上升(满载切快升速度)→消费拖拽累积量→钳制屏幕内→同步深度到 Model。</summary>
        private void Update()
        {
            if(!_diving && !_ascending) return;

            Camera cam = Camera.main;
            if(cam == null) return;

            GameManager gm = GameManager.Instance;
            Vector3 pos = transform.position;

            if(_diving)
            {
                pos.y = Mathf.Max(pos.y - _descentSpeed * Time.deltaTime, -_maxDepth);

                if(!_enteredWater && pos.y <= _waterLineY)
                {
                    _enteredWater = true;
                    AudioManager.Instance?.PlayDive();
                }
            }
            else if(gm != null && gm.Model != null)
            {
                float speed = gm.Model.IsFull ? _fastAscentSpeed : _ascentSpeed;
                pos.y = Mathf.Min(pos.y + speed * Time.deltaTime, _startPos.y);
            }

            if(_pendingDragX != 0f)
            {
                float worldPerPixel = 2f * cam.orthographicSize / Screen.height;
                pos.x += _pendingDragX * worldPerPixel;
                _pendingDragX = 0f;
            }

            float halfWidth = cam.orthographicSize * cam.aspect;
            float limit = Mathf.Max(0.05f,halfWidth - _xMargin);
            pos.x = Mathf.Clamp(pos.x, -limit, limit);
            transform.position = pos;

            if(GameManager.Instance != null && GameManager.Instance.Model != null)
            {
                GameManager.Instance.Model.SetDepth(Mathf.RoundToInt(Mathf.Max(0f, -pos.y)));
            }
        }
        /// <summary>碰撞转发:只发事件不管后果,鱼已捕获则忽略(防重复触发)。判定语义见两个状态类。</summary>
        private void OnTriggerEnter2D(Collider2D other)
        {
            FishController fish = other.GetComponent<FishController>();
            if(fish == null || fish.IsCaught) return;

            EventBus.Instance.Post(new HookHitFishEvent(fish, CatchPoint));
        }
    }
}