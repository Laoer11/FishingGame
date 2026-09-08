using UnityEngine;

namespace FishingGame.Core
{
    /// <summary>输入组装器:按条件编译挂对应实现(编辑器/PC=鼠标,其余=触摸)。</summary>
    [DisallowMultipleComponent]
    public class InputBootstrap : MonoBehaviour
    {
        public IInputService Service { get; private set; }

        private void Awake()
        {
            if(Service != null) return;

#if UNITY_EDITOR || UNITY_STANDALONE
            Service = gameObject.AddComponent<MouseInputService>();
#else
            Service = gameObject.AddComponent<TouchInputService>();
#endif
        }
    }
}