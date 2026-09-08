using FishingGame.Core;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Video;

namespace FishingGame.Views
{
    /// <summary>教学面板</summary>
    public class TutorialView : UIViewBase
    {
        private static readonly string[] Steps = 
        {
            "鱼钩下潜时长按屏幕可左右移动,下潜至极致前尽量不要碰到鱼。",
            "触碰鱼可将其捕获,鱼钩将开始向上回收。",
            "鱼钩向上回收过程中,尽量捕获珍稀鱼类确保收益最大化。"
        };

        [SerializeField] private Text _stepText;
        [SerializeField] private Button _nextBtn;
        [SerializeField] private Button _closeBtn;
        [SerializeField] private VideoPlayer _video;
        [SerializeField] private VideoClip[] _stepClips;

        private int _index;

        protected override void Awake()
        {
            base.Awake();
            Hide();
        }

        protected override void Subscribe()
        {
            EventBus.Instance.Subscribe<PhaseChangedEvent>(OnPhaseChanged);
            _nextBtn.onClick.AddListener(OnNextClicked);
            _closeBtn.onClick.AddListener(OnCloseClicked);
        }

        protected override void Unsubscribe()
        {
            if(EventBus.Instance != null) EventBus.Instance.Unsubscribe<PhaseChangedEvent>(OnPhaseChanged);
            _nextBtn.onClick.RemoveListener(OnNextClicked);
            _closeBtn.onClick.RemoveListener(OnCloseClicked);
        }

        public override void Show()
        {
            Group.alpha = 1f;
            Group.blocksRaycasts = true;
            _index = 0;
            if(_video != null && (_stepClips == null || _stepClips.Length == 0)) _video.Play();
            Refresh();
        }

        public override void Hide()
        {
            Group.alpha = 0f;
            Group.blocksRaycasts = false;
            if(_video != null) _video.Stop();
        }

        private void OnPhaseChanged(PhaseChangedEvent evt)
        {
            if(evt.Phase == GamePhase.Tutorial) Show();
            else Hide();
        }

        /// <summary>刷新当前步:文案、按钮文案(末步变"开始游戏")</summary>
        private void Refresh()
        {
            _stepText.text = Steps[_index];
            Text label = _nextBtn.GetComponentInChildren<Text>();
            if(label != null) label.text = _index >= Steps.Length - 1 ? "开始游戏" : "知道了";

            if(_video != null && _stepClips != null && _index < _stepClips.Length && _stepClips[_index] != null)
            {
                _video.clip = _stepClips[_index];
                _video.Play();
            }
        }

        /// <summary>下一步:非末步推进,末步完成教学。</summary>
        private void OnNextClicked()
        {
            AudioManager.Instance?.PlayClick();
            if(_index < Steps.Length - 1)
            {
                _index++;
                Refresh();
            }
            else FinishTutorial();
        }

        private void OnCloseClicked()
        {
            AudioManager.Instance?.PlayClick();
            FinishTutorial();
        }

        private void FinishTutorial()
        {
            EventBus.Instance.Post(new TutorialDoneCommand());
        }

    }
}