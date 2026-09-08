using FishingGame.Core;
using UnityEngine;

namespace FishingGame.Controllers
{
    /// <summary>捕获闪光特效:订阅捕获事件播放 ParticleSystem</summary>
    public class CatchFx : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _flash;
        
        private void Start()
        {
            if(_flash != null) EventBus.Instance.Subscribe<FishCaughtEvent>(OnFishCaught);
        }

        private void OnDestroy()
        {
            if(EventBus.Instance != null && _flash != null)
                EventBus.Instance.Unsubscribe<FishCaughtEvent>(OnFishCaught);
        }

        private void OnFishCaught(FishCaughtEvent evt)
        {
            _flash.Play();
        }
    }
}