using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FishingGame.Core
{
    /// <summary>鼠标输入实现(PC/编辑器):按住左键期间逐帧发位移增量。</summary>
    public class MouseInputService : MonoBehaviour, IInputService
    {
        public event Action<Vector2> OnDragDelta;

        /// <summary>是否按住中(增量基准点随之更新)。</summary>
        private bool _isHolding;
        private Vector3 _lastPos;

        /// <summary>按住期间逐帧计算位移差并发送(超过死区0.01才触发,防手抖噪声)。</summary>
        private void Update()
        {
            if(Input.GetMouseButtonDown(0))
            {
                _isHolding = true;
                _lastPos = Input.mousePosition;
            }
            else if(Input.GetMouseButtonUp(0))
            {
                _isHolding = false;
            }
            
            if(!_isHolding) return;

            Vector3 cur = Input.mousePosition;
            Vector2 delta = cur - _lastPos;
            _lastPos = cur;

            if (delta.sqrMagnitude > 0.01f)
            {
                OnDragDelta?.Invoke(delta);
            }
        }
        
        /// <summary>指针是否悬停在 UI 上</summary>
        private static bool IsPointerOverUI()
        {
            return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        }
    }
}