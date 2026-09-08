using UnityEngine;

namespace FishingGame.Controllers
{
    /// <summary>鱼线:LineRenderer 两点(船底→钩),LateUpdate 逐帧跟随(Late 确保钩先动绳后画)。</summary>
    [RequireComponent(typeof(LineRenderer))]
    public class RopeController : MonoBehaviour
    {
        /// <summary>船底挂点(Inspector 拖)。</summary>
        public Transform RopeAnchor;

        /// <summary>绳尾挂点(Inspector 拖)。</summary>
        public Transform LineTiePoint;

        [SerializeField] private float _width = 0.05f;
        [SerializeField] private Color _color = Color.white;

        private LineRenderer _line;

        /// <summary>初始化:世界坐标模式+宽度/颜色,运行时 new 材质避免打包资源引用。</summary>
        private void Awake()
        {
            _line = GetComponent<LineRenderer>();
            _line.useWorldSpace = true;
            _line.positionCount = 2;
            _line.widthMultiplier = _width;
            _line.startColor = _color;
            _line.endColor = _color;

            Shader shader = Shader.Find("Sprites/Default");
            if (shader != null) _line.material = new Material(shader);
        }

        private void LateUpdate()
        {
            if (RopeAnchor == null) return;
            _line.SetPosition(0, RopeAnchor.position);
            _line.SetPosition(1, LineTiePoint.transform.position);
        }
    }
}