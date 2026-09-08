using System.Collections;
using FishingGame.Core;
using UnityEngine;
using UnityEngine.UI;

namespace FishingGame.Views
{
    /// <summary>开始面板</summary>
    public class StartView : UIViewBase
    {
        [SerializeField] private Button _startBtn;
        [SerializeField] private Button _exitBtn;
        [SerializeField] private float _slideSeconds = 0.6f;

        private RectTransform _rect;
        private Coroutine _slide;

        protected override void Awake()
        {
            base.Awake();
            _rect = (RectTransform)transform;
        }

        protected override void Subscribe()
        {
            EventBus.Instance.Subscribe<PhaseChangedEvent>(OnPhaseChanged);
            _startBtn.onClick.AddListener(OnStartClicked);
            _exitBtn.onClick.AddListener(OnExitClicked);
        }

        protected override void Unsubscribe()
        {
            if(EventBus.Instance != null) EventBus.Instance.Unsubscribe<PhaseChangedEvent>(OnPhaseChanged);
            _startBtn.onClick.RemoveListener(OnStartClicked);
            _exitBtn.onClick.RemoveListener(OnExitClicked);
        }

        public override void Show()
        {
            _rect.anchoredPosition = Vector2.zero;
            base.Show();
        }

        private void OnPhaseChanged(PhaseChangedEvent evt)
        {
            if(evt.Phase == GamePhase.Start) Show();
            else Hide();
        }

        /// <summary>开始点击:播音效→发命令→启动滑出</summary>
        private void OnStartClicked()
        {
            AudioManager.Instance?.PlayClick();
            EventBus.Instance.Post(new StartCommand());
            if(_slide != null) StopCoroutine(_slide);
            _slide = StartCoroutine(SlideOut());
        }

        /// <summary>退出:编辑器与真机分支(条件编译)</summary>
        private void OnExitClicked()
        {
            AudioManager.Instance?.PlayClick();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        /// <summary>滑出退场:面板上滑+透明度渐隐</summary>
        private IEnumerator SlideOut()
        {
            Group.interactable = false;
            Group.blocksRaycasts = false;

            Vector2 start = _rect.anchoredPosition;
            float t = 0f;
            while(t < _slideSeconds)
            {
                t += Time.deltaTime;
                float k = t / _slideSeconds;
                _rect.anchoredPosition = new Vector2(start.x, start.y + k * _rect.rect.height);
                Group.alpha = 1f - k;
                yield return null;
            }
            Hide();
            _slide = null;
        }
    }
}