using FishingGame.Core;
using UnityEngine;
using UnityEngine.UI;

namespace FishingGame.Views
{
    /// <summary>暂停小按钮</summary>
    public class PauseButtonView : MonoBehaviour
    {
        [SerializeField] private Button _button;

        private CanvasGroup _group;

        private void Awake()
        {
            _group = GetComponent<CanvasGroup>();
            if(_group == null) _group = gameObject.AddComponent<CanvasGroup>();
        }

        private void OnEnable()
        {
            EventBus.Instance.Subscribe<PhaseChangedEvent>(OnPhaseChanged);
            _button.onClick.AddListener(OnClicked);
        }

        private void OnDisable()
        {
            if(EventBus.Instance != null) EventBus.Instance.Unsubscribe<PhaseChangedEvent>(OnPhaseChanged);
            _button.onClick.RemoveListener(OnClicked);
        }

        /// <summary>阶段控制:开始阶段隐藏;游戏内三阶段可点;教学/结算阶段可见但禁用。</summary>
        private void OnPhaseChanged(PhaseChangedEvent evt)
        {
            if(evt.Phase == GamePhase.Start)
            {
                _group.alpha = 0f;
                _group.blocksRaycasts = false;
                _button.interactable = false;
                return;
            }

            _group.alpha = 1f;
            _group.blocksRaycasts = true;
            _button.interactable =
                evt.Phase == GamePhase.Countdown ||
                evt.Phase == GamePhase.Descending ||
                evt.Phase == GamePhase.Ascending;
        }

        private void OnClicked()
        {
            AudioManager.Instance?.PlayClick();
            EventBus.Instance.Post(new PauseCommand());
        }
    }
}
