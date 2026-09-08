using System.Collections;
using FishingGame.Core;
using UnityEngine;
using UnityEngine.UI;

namespace FishingGame.Views
{
    /// <summary>倒计时面板:订阅 CountdownTickEvent 播数字动画</summary>
    public class CountdownView : UIViewBase
    {
        [SerializeField] private Text _label;
        [SerializeField] private float _animSeconds = 0.7f;

        private Color _baseColor;
        private Coroutine _anim;

        protected override void Awake()
        {
            base.Awake();
            if(_label == null)
            {
                Debug.LogError("[CountdownView] 未拖 _label,请在 Inspector 赋值");
                return;
            }
            _baseColor = _label.color;
        }

        protected override void Subscribe()
        {
            EventBus.Instance.Subscribe<PhaseChangedEvent>(OnPhaseChanged);
            EventBus.Instance.Subscribe<CountdownTickEvent>(OnCountdownTick);
        }

        protected override void Unsubscribe()
        {
            EventBus.Instance.Unsubscribe<PhaseChangedEvent>(OnPhaseChanged);
            EventBus.Instance.Unsubscribe<CountdownTickEvent>(OnCountdownTick);
        }
        
        public override void Show()
        {
            Group.alpha = 1f;
            Group.blocksRaycasts = true;
        }

        public override void Hide()
        {
            Group.alpha = 0f;
            Group.blocksRaycasts = false;
            if(_anim != null)
            {
                StopCoroutine(_anim);
                _anim = null;
            }
        }

        private void OnPhaseChanged(PhaseChangedEvent evt)
        {
            if(evt.Phase == GamePhase.Countdown) Show();
            else Hide();
        }

        /// <summary>收到数字:停掉上一个动画再播新的</summary>
        private void OnCountdownTick(CountdownTickEvent evt)
        {
            if(_label == null) return;
            if(_anim != null) StopCoroutine(_anim);
            _anim = StartCoroutine(PlayNumber(evt.Number));
        }

        /// <summary>数字动画:1.6→1 缩放 + 透明度 1→0.5 衰减,0 显示为 GO。</summary>
        private IEnumerator PlayNumber(int number)
        {
            _label.text = number > 0 ? number.ToString() : "GO";
            _label.color = _baseColor;

            RectTransform rect = _label.rectTransform;
            float t = 0f;
            while (t < _animSeconds)
            {
                t += Time.deltaTime;
                float k = Mathf.Clamp01(t / _animSeconds);
                rect.localScale = Vector3.one * Mathf.Lerp(1.6f, 1f, k);
                Color c = _baseColor;
                c.a = Mathf.Lerp(1f, 0.5f, k);
                _label.color = c;
                yield return null;
            }
            _anim = null;
        }
    }
}