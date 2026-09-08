using UnityEngine;

namespace FishingGame.Controllers
{
    /// <summary>巡逻行为:固定方向水平游动,出屏后环绕到另一侧。</summary>
    public class PatrolBehavior : IFishBehavior
    {
        /// <summary>游动方向(±1),构造时随机定。</summary>
        private readonly float _dir;

        public PatrolBehavior()
        {
            _dir = Random.value < 0.5f ? -1f : 1f;
        }

        /// <summary>水平推进 + 出屏环绕 + 按朝向翻转贴图(flipX)。</summary>
        public void Tick(FishController fish, float dt)
        {
            if(fish.Def == null) return;

            Transform t = fish.transform;
            Vector3 p = t.position;
            p.x += _dir * fish.Def.MoveSpeed * dt;
            t.position = p;

            float halfWidth = Camera.main.orthographicSize * Camera.main.aspect + 1.5f;
            if(_dir > 0f && p.x > halfWidth) p.x = -halfWidth;
            else if(_dir < 0f && p.x < -halfWidth) p.x = halfWidth;
            t.position = p;

            fish.Renderer.flipX = _dir < 0f;
        }
    }
}