using UnityEngine;

namespace FishingGame.Views
{
    /// <summary>UI 面板基类:CanvasGroup 控制显隐,</summary>
    public abstract class UIViewBase : MonoBehaviour
    {
        protected CanvasGroup Group { get; private set; }

        protected virtual void Awake()
        {
            Group = GetComponent<CanvasGroup>();
            if(Group == null) 
                Group = gameObject.AddComponent<CanvasGroup>();
            Hide();
        }

        protected virtual void OnEnable()
        {
            Subscribe();
        }

        protected virtual void OnDisable()
        {
            Unsubscribe();
        }

        /// <summary>显示面板。子类可重写追加动画,但不得开启 blocksRaycasts 除非面板需要点击。</summary>
        public virtual void Show()
        {
            Group.alpha = 1f;
            Group.interactable = true;
            Group.blocksRaycasts = true;
        }

        /// <summary>隐藏面板。GameObject 保持激活,仅屏蔽视觉与交互。</summary>
        public virtual void Hide()
        {
            Group.alpha = 0f;
            Group.interactable = false;
            Group.blocksRaycasts = false;
        }

        /// <summary>子类实现:订阅事件与按钮</summary>
        protected abstract void Subscribe();

        /// <summary>与 Subscribe 严格成对(OnDisable 时自动调用)。</summary>
        protected abstract void Unsubscribe();
    }


}