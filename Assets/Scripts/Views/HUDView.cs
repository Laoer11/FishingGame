using FishingGame.Core;
using FishingGame.Models;
using UnityEngine;
using UnityEngine.UI;

namespace FishingGame.Views
{
    /// <summary>局内 HUD:护盾/捕获/深度三项数据绑定。纯被动——只订阅事件刷新文本,不发起任何命令。</summary>
    public class HUDView : UIViewBase
    {
        [SerializeField] private Text _skillText;
        [SerializeField] private Text _catchText;
        [SerializeField] private Text _depthText;

        private int _skillTotal = 2;
        private int _catchCapacity = 10;
        private int _maxDepth = 100;

        private void Start()
        {
            if(GameManager.Instance != null && GameManager.Instance.Config != null)
            {
                GameConfig cfg = GameManager.Instance.Config;
                _skillTotal = cfg.SkillCount;
                _catchCapacity = cfg.CatchCapacity;
                _maxDepth = (int)cfg.MaxDepth;
            }
            RefreshAll();
        }

        protected override void Subscribe()
        {
            EventBus.Instance.Subscribe<SkillCountChangedEvent>(OnSkillChanged);
            EventBus.Instance.Subscribe<CatchCountChangedEvent>(OnCatchChanged);
            EventBus.Instance.Subscribe<DepthChangedEvent>(OnDepthChanged);
            EventBus.Instance.Subscribe<PhaseChangedEvent>(OnPhaseChanged);
        }

        protected override void Unsubscribe()
        {
            EventBus.Instance.Unsubscribe<SkillCountChangedEvent>(OnSkillChanged);
            EventBus.Instance.Unsubscribe<CatchCountChangedEvent>(OnCatchChanged);
            EventBus.Instance.Unsubscribe<DepthChangedEvent>(OnDepthChanged);
            EventBus.Instance.Unsubscribe<PhaseChangedEvent>(OnPhaseChanged);
        }

        private void OnSkillChanged(SkillCountChangedEvent evt)
        {
            _skillTotal = evt.Total;
            if(_skillText != null)
                _skillText.text = "护盾 " + evt.Left + "/" + evt.Total;
        }

        private void OnCatchChanged(CatchCountChangedEvent evt)
        {
            _catchCapacity = evt.Capacity;
            if(_catchText != null)
                _catchText.text = "捕获 " + evt.Current + "/" + evt.Capacity;
        }

        private void OnDepthChanged(DepthChangedEvent evt)
        {
            if (_depthText != null) _depthText.text = "深度 " + evt.Depth + "/" + _maxDepth + "m";
        }

        /// <summary>阶段响应:倒计时/下潜/上升阶段显示,其余隐藏;倒计时进场时先满刷新再显示防旧数据闪现。</summary>
        private void OnPhaseChanged(PhaseChangedEvent evt)
        {
            if (evt.Phase == GamePhase.Countdown)
            {
                RefreshAll();
                Show();
            }
            else if (evt.Phase == GamePhase.Descending || evt.Phase == GamePhase.Ascending)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }

        private void RefreshAll()
        {
            if (_skillText != null) _skillText.text = "护盾 " + _skillTotal + "/" + _skillTotal;
            if (_catchText != null) _catchText.text = "捕获 0/" + _catchCapacity;
            if (_depthText != null) _depthText.text = "深度 0/" + _maxDepth + "m";
        }
    }
}