using UnityEngine;

namespace FishingGame.Controllers
{
    /// <summary>程序化水面:Y=0 处生成一条正弦波泡沫带,双层波叠加+顶点色向下渐隐。挂空物体,无素材依赖。</summary>
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class WaterSurfaceController : MonoBehaviour
    {
        [SerializeField] private int _segments = 80;
        [SerializeField] private float _width = 30f;
        [SerializeField] private float _thickness = 0.5f;
        [SerializeField] private float _waveHeight = 0.1f;
        [SerializeField] private float _waveLength = 1.8f;
        [SerializeField] private float _scrollSpeed = 0.6f;
        [SerializeField] private Color _foamColor = new Color(1f, 1f, 1f, 0.85f);
        [SerializeField] private int _sortOrder = 5;

        private Mesh _mesh;
        private Vector3[] _verts;
        private Color[] _colors;
        private int _cols;

        private void Start()
        {
            MeshRenderer mr = GetComponent<MeshRenderer>();
            mr.sortingOrder = _sortOrder;
            mr.material = new Material(Shader.Find("Sprites/Default"));
            Build();
        }

        /// <summary>构建静态网格:segments+1 列 × 3 行(泡沫顶/渐变中/透明底),顶点色做向下渐隐,无素材依赖。</summary>
        private void Build()
        {
            _cols = _segments + 1;
            int rows = 3;
            _verts = new Vector3[_cols * rows];
            _colors = new Color[_cols * rows];
            int[] tris = new int[_segments * (rows - 1) * 6];

            Color mid = _foamColor; mid.a *= 0.35f;
            Color fade = _foamColor; fade.a = 0f;

            for(int c = 0; c < _cols; c++)
            {
                float x = -_width * 0.5f + _width * c / _segments;
                _verts[c] = new Vector3(x, 0f, 0f);
                _colors[c] = _foamColor;
                _verts[_cols + c] = new Vector3(x, -_thickness * 0.35f, 0f);
                _colors[_cols + c] = mid;
                _verts[2 * _cols + c] = new Vector3(x, -_thickness, 0f);
                _colors[2 * _cols + c] = fade;
            }

            int t = 0;
            for(int s = 0; s < _segments; s++)
            {
                for(int r = 0; r < rows - 1; r++)
                {
                    int i0 = r * _cols + s;
                    int i1 = r * _cols + s + 1;
                    int i2 = (r + 1) * _cols + s;
                    int i3 = (r + 1) * _cols + s + 1;
                    tris[t++] = i0; tris[t++] = i2; tris[t++] = i1;
                    tris[t++] = i1; tris[t++] = i2; tris[t++] = i3;
                }
            }

            _mesh = new Mesh { name = "WaterSurface" };
            _mesh.vertices = _verts;
            _mesh.colors = _colors;
            _mesh.triangles = tris;
            GetComponent<MeshFilter>().mesh = _mesh;
        }

        /// <summary>逐帧波浪:双正弦叠加(主波+0.5幅度高频副波)驱动顶行/中行 Y,下行固定作渐隐边界。仅改顶点数组后回写。</summary>
        private void Update()
        {
            if(_mesh == null) return;

            float time = Time.time * _scrollSpeed;
            for(int c = 0; c < _cols; c++)
            {
                float x = _verts[c].x;
                float wave = Mathf.Sin(x / _waveLength * 6.2832f + time * 6.2832f) * _waveHeight
                           + Mathf.Sin(x / (_waveLength * 0.53f) * 6.2832f - time * 8.8f) * _waveHeight * 0.5f;
                _verts[c].y = wave;
                _verts[_cols + c].y = wave - _thickness * 0.35f;
            }
            _mesh.vertices = _verts;
        }
    }
}