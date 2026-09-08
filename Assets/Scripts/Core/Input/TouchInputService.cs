using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FishingGame.Core
{
    /// <summary>触摸输入实现(移动端)</summary>
    public class TouchInputService : MonoBehaviour, IInputService
    {
        public event Action<Vector2> OnDragDelta;

        /// <summary>逐帧读首指:仅 Moved 阶段发增量。</summary>
        private void Update()
        {
            if(Input.touchCount <= 0) return;

            Touch t = Input.GetTouch(0);
            if(t.phase != TouchPhase.Moved) return;
            if(EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(t.fingerId)) return;

            OnDragDelta?.Invoke(t.deltaPosition);
        }
    }
}