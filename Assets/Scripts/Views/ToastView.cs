using System.Collections;
using FishingGame.Core;
using UnityEngine;

namespace FishingGame.Views
{
    /// <summary>轻提示:订阅 ToastEvent,显示后停顿再淡出。</summary>
    public class ToastView : UIViewBase
    {
        [SerializeField] private UnityEngine.UI.Text _label;
        [SerializeField] private float _holdSeconds = 1.5f;

        private Coroutine _fade;

        protected override void Awake()
        {
            base.Awake();
            Hide();
        }

        protected override void Subscribe()
        {
            EventBus.Instance.Subscribe<ToastEvent>(OnToast);
        }

        protected override void Unsubscribe()
        {
            if(EventBus.Instance != null) EventBus.Instance.Unsubscribe<ToastEvent>(OnToast);
        }

        public override void Show() {Group.alpha = 1f;}

        public override void Hide() {Group.alpha = 0f;}

        /// <summary>收到提示:换文案立即显示,并重启淡出协程(新提示打断旧淡出)。</summary>
        private void OnToast(ToastEvent evt)
        {
            if(_label == null) return;
            _label.text = evt.Message;
            Show();
            if(_fade != null) StopCoroutine(_fade);
            _fade = StartCoroutine(FadeOut());
        }

        /// <summary>淡出节奏:前 60% 时长完整停留,后 40% 渐隐(透明度走 CanvasGroup,不动 Text 颜色)。</summary>
        private IEnumerator FadeOut()
        {
            yield return new WaitForSeconds(_holdSeconds * 0.6f);

            float t = 0f;
            float fade = Mathf.Max(0.1f, _holdSeconds * 0.4f);
            while(t < fade)
            {
                t += Time.deltaTime;
                Group.alpha = Mathf.Lerp(1f, 0f, t / fade);
                yield return null;
            }
            Hide();
            _fade = null;
        }
    }
}