using UnityEngine;

namespace FishingGame.Controllers
{
    /// <summary>蛇行行为:双倍速横向游动 + 大幅正弦摆尾(0.8m),模拟鳗类快速窜游。</summary>
    public class SnakeBehavior : IFishBehavior
    {
        private float _baseY;
        /// <summary>正弦相位,首帧随机初始化。</summary>
        private float _t;
        private readonly float _dir;
        private bool _init;

        public SnakeBehavior()
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
                _t = Random.value * Mathf.PI * 2f;
                _init = true;
            }

            _t += dt;
            Vector3 p = t.position;
            p.x += _dir * fish.Def.MoveSpeed * 2f * dt;
            p.y = _baseY + Mathf.Sin(_t + 3f) * 0.8f;

            float halfWidth = Camera.main.orthographicSize * Camera.main.aspect + 1.5f;
            if (Mathf.Abs(p.x) > halfWidth) p.x = -Mathf.Sign(p.x) * halfWidth;
            t.position = p;

            fish.Renderer.flipX = _dir < 0f;

        }
    }
}