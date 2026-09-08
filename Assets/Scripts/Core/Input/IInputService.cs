using System;
using UnityEngine;

namespace FishingGame.Core
{
    /// <summary>输入服务抽象:跨平台拖拽输入的统一契约</summary>
    public interface IInputService
    {
        /// <summary>拖动增量(像素/帧),按住期间每帧触发。</summary>
        event Action<Vector2> OnDragDelta;
    }
}