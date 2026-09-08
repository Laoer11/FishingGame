using FishingGame.Core;
using UnityEngine;
using UnityEngine.UI;

namespace FishingGame.Views
{
    /// <summary>暂停面板</summary>
    public class PauseView : UIViewBase
    {
        [SerializeField] private Button _resumeBtn;
        [SerializeField] private Button _restartBtn;
        [SerializeField] private Button _exitBtn;

        protected override void Awake()
        {
            base.Awake();
            Hide();
        }

        protected override void Subscribe()
        {
            EventBus.Instance.Subscribe<PauseCommand>(OnPause);
            EventBus.Instance.Subscribe<ResetCommand>(OnReset);
            _resumeBtn.onClick.AddListener(OnResumeClicked);
            _restartBtn.onClick.AddListener(OnRestartClicked);
            _exitBtn.onClick.AddListener(OnExitClicked);
        }

        protected override void Unsubscribe()
        {
            if(EventBus.Instance != null)
            {
                EventBus.Instance.Unsubscribe<PauseCommand>(OnPause);
                EventBus.Instance.Unsubscribe<ResetCommand>(OnReset);
            }
            _resumeBtn.onClick.RemoveListener(OnResumeClicked);
            _restartBtn.onClick.RemoveListener(OnRestartClicked);
            _exitBtn.onClick.RemoveListener(OnExitClicked);
        }

        /// <summary>显示并冻结全局时间。timeScale=0 后 deltaTime 归零,钩/鱼/协程全部自然停。</summary>
        public override void Show()
        {
            Group.alpha = 1f;
            Group.blocksRaycasts = true;
            Time.timeScale = 0f;
        }

        /// <summary>隐藏并恢复时间流速。收到 ResetCommand 时也会走这里先解冻再重开。</summary>
        public override void Hide()
        {
            Group.alpha = 0f;
            Group.blocksRaycasts = false;
            Time.timeScale = 1f;
        }

        private void OnPause(PauseCommand evt) { Show(); }

        private void OnReset(ResetCommand evt) { Hide(); }

        private void OnResumeClicked()
        {
            AudioManager.Instance?.PlayClick();
            Hide();
        }

        private void OnRestartClicked()
        {
            AudioManager.Instance?.PlayClick();
            Hide();
            EventBus.Instance.Post(new ResetCommand());
        }

        private void OnExitClicked()
        {
            AudioManager.Instance?.PlayClick();
            Hide();
            EventBus.Instance.Post(new ExitToStartCommand());
        }

        /// <summary>移动端切后台/来电时自动弹出暂停,避免玩家错过游戏进度。</summary>
        private void OnApplicationPause(bool paused)
        {
            if(paused) Show();
        }
    }
}
