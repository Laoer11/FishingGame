using System;
using System.Collections.Generic;
using UnityEngine;

namespace FishingGame.Core
{
    /// <summary>全局事件中心(发布/订阅模式)。各系统通过事件通信,互不引用:鱼被钓到→HUD 刷新计数。</summary>
    public class EventBus : MonoBehaviour
    {
        /// <summary>全局单例。须设 Script Execution Order = -100,确保先于所有 View 完成初始化。</summary>
        public static EventBus Instance { get; private set; }

        /// <summary>事件类型 → 委托链。同一类型可挂多个订阅者,由 Delegate.Combine 串成链。</summary>
        private readonly Dictionary<Type, Delegate> _handlers = new Dictionary<Type, Delegate>();
        
        /// <summary>已存在实例则销毁自身;否则登记并跨场景存活。</summary>
        private void Awake()
        {
            if(Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>发布事件,通知所有订阅者。无订阅者时为空操作。</summary>
        public void Post<T>(T evt)
        {
            if(_handlers.TryGetValue(typeof(T),out Delegate d))
            {
                ((Action<T>)d)(evt);
            }
        }

        /// <summary>订阅事件。同一 handler 重复订阅会被合并去重(Delegate.Combine 语义)。</summary>
        public void Subscribe<T>(Action<T> handler)
        {
            _handlers.TryGetValue(typeof(T), out Delegate existing);
            _handlers[typeof(T)] = Delegate.Combine(existing, handler);
        }

        /// <summary> 退订事件 </summary>
        public void Unsubscribe<T>(Action<T> handler)
        {
            if (!_handlers.TryGetValue(typeof(T), out Delegate existing)) return;

            Delegate removed = Delegate.Remove(existing, handler);
            if (removed == null) _handlers.Remove(typeof(T));
            else _handlers[typeof(T)] = removed;
        }
    }
}