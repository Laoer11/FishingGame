using UnityEngine;

namespace FishingGame.Controllers
{
    /// <summary>缓漂行为:在基准深度附近小幅正弦上下漂(幅度 0.3m),水平缓移。</summary>
    public class DriftBehavior : IFishBehavior
    {
        /// <summary>出生时锚定的基准深度,漂动围绕它进行。</summary>
        private float _baseY;
        /// <summary>正弦相位,首帧随机初始化避免全场同步漂。</summary>
        private float _phase;
        private readonly float _dir;
        private bool _init;

        public DriftBehavior()
        {
            _dir = Random.value < 0.5f ? -1f : 1f;
        }

        public void Tick(FishController fish, float dt)
        {
            if(fish.Def == null) return;

            Transform t = fish.transform;
            if(!_init)
            {
                _baseY = t.position.y;
                _phase = Random.value * Mathf.PI * 2f;
                _init = true;
            }

            _phase += dt * 1.5f;
            Vector3 p = t.position;
            p.y = _baseY + Mathf.Sin(_phase) * 0.3f;
            p.x += _dir * fish.Def.MoveSpeed * 0.2f * dt;

            float halfWidth = Camera.main.orthographicSize * Camera.main.aspect + 1.5f;
            if (Mathf.Abs(p.x) > halfWidth) p.x = -Mathf.Sign(p.x) * halfWidth;
            t.position = p;

            fish.Renderer.flipX = _dir < 0f;
        }
    }
}