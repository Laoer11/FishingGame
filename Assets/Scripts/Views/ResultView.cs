using System.Collections.Generic;
using FishingGame.Core;
using FishingGame.Models;
using UnityEngine;
using UnityEngine.UI;

namespace FishingGame.Views
{
    /// <summary>结算面板:订阅 RoundEndEvent 渲染快照(标题/深度/金币/贝壳),动态生成鱼种卡片。</summary>
    public class ResultView : UIViewBase
    {
        [SerializeField] private Text _titleText;
        [SerializeField] private Transform _cardContainer;
        [SerializeField] private FishCardView _cardPrefab;
        [SerializeField] private Text _depthText;
        [SerializeField] private Text _coinText;
        [SerializeField] private Text _shellText;
        [SerializeField] private Button _retryBtn;
        [SerializeField] private Button _exitBtn;

        protected override void Awake()
        {
            base.Awake();
            Hide();
        }

        protected override void Subscribe()
        {
            EventBus.Instance.Subscribe<PhaseChangedEvent>(OnPhaseChanged);
            EventBus.Instance.Subscribe<RoundEndEvent>(OnRoundEnd);
            _retryBtn.onClick.AddListener(OnRetryClicked);
            _exitBtn.onClick.AddListener(OnExitClicked);
        }

        protected override void Unsubscribe()
        {
            if (EventBus.Instance != null)
            {
                EventBus.Instance.Unsubscribe<PhaseChangedEvent>(OnPhaseChanged);
                EventBus.Instance.Unsubscribe<RoundEndEvent>(OnRoundEnd);
            }
            _retryBtn.onClick.RemoveListener(OnRetryClicked);
            _exitBtn.onClick.RemoveListener(OnExitClicked);
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
        }

        private void OnPhaseChanged(PhaseChangedEvent evt)
        {
            if(evt.Phase == GamePhase.Settlement) Show();
            else Hide();
        }

        /// <summary>渲染结算:倒序清掉旧卡片→按捕获数量降序生成新卡片(数量多的排前面)。</summary>
        private void OnRoundEnd(RoundEndEvent evt)
        {
            RunResult r = evt.Result;
            if(_titleText != null)
                _titleText.text = r.Catches.Count == 0 ? "这次颗粒无收" : "本次收获";
            _depthText.text = "下潜深度 " + r.Depth + " 米";
            _coinText.text = "金币 ×" + r.CoinTotal;
            _shellText.text = "贝壳 ×" + r.ShellTotal;

            for(int i = _cardContainer.childCount - 1; i >= 0; i--)
            {
                Destroy(_cardContainer.GetChild(i).gameObject);
            }

            var list = new List<KeyValuePair<FishDef, int>>(r.Catches);
            list.Sort((a, b) => b.Value.CompareTo(a.Value));

            foreach(KeyValuePair<FishDef, int> pair in list)
            {
                FishCardView card = Instantiate(_cardPrefab, _cardContainer);
                card.SetData(pair.Key, pair.Value);
            }
        }

        private void OnRetryClicked()
        {
            AudioManager.Instance?.PlayClick();
            EventBus.Instance.Post(new ResetCommand());
        }

        private void OnExitClicked()
        {
            AudioManager.Instance?.PlayClick();
            EventBus.Instance.Post(new ExitToStartCommand());
        }

    }
}