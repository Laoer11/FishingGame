using FishingGame.Core;
using UnityEngine;

namespace FishingGame.Core
{
    /// <summary>音效门面:事件驱动自动配音效(捕获/扣盾/倒计时/结算)</summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [SerializeField] private AudioClip _diveClip;
        [SerializeField] private AudioClip _catchClip;
        [SerializeField] private AudioClip _shieldClip;
        [SerializeField] private AudioClip _tickClip;
        [SerializeField] private AudioClip _settleClip;
        [SerializeField] private AudioClip _clickClip;
        [SerializeField] private AudioClip _drawClip;
        [SerializeField] private AudioClip _bgmClip;
        [SerializeField, Range(0f, 1f)] private float _bgmVolume = 0.4f;

        /// <summary>音效轮询池</summary>
        private AudioSource[] _pool;
        private AudioSource _bgm;
        /// <summary>轮询下标,循环复用。</summary>
        private int _next;

        /// <summary>单例守门:重复实例自毁;构造 2 源轮询池 + 循环 BGM 源。</summary>
        private void Awake()
        {
            if(Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            _pool = new AudioSource[2];
            for(int i = 0; i < _pool.Length; i++)
            {
                _pool[i] = gameObject.AddComponent<AudioSource>();
                _pool[i].playOnAwake = false;
            }

            _bgm = gameObject.AddComponent<AudioSource>();
            _bgm.playOnAwake = false;
            _bgm.loop = true;
            _bgm.volume = _bgmVolume;
            if(_bgmClip != null)
            {
                _bgm.clip = _bgmClip;
                _bgm.Play();
            }
        }

        private void OnDestroy()
        {
            if(Instance == this) Instance = null;
        }

        private void OnEnable()
        {
            EventBus.Instance.Subscribe<FishCaughtEvent>(OnFishCaught);
            EventBus.Instance.Subscribe<SkillCountChangedEvent>(OnSkillCountChanged);
            EventBus.Instance.Subscribe<CountdownTickEvent>(OnCountdownTick);
            EventBus.Instance.Subscribe<RoundEndEvent>(OnRoundEnd);
        }

        private void OnDisable()
        {
            if(EventBus.Instance == null) return;
            EventBus.Instance.Unsubscribe<FishCaughtEvent>(OnFishCaught);
            EventBus.Instance.Unsubscribe<SkillCountChangedEvent>(OnSkillCountChanged);
            EventBus.Instance.Unsubscribe<CountdownTickEvent>(OnCountdownTick);
            EventBus.Instance.Unsubscribe<RoundEndEvent>(OnRoundEnd);
        }

        /// <summary>池化播放:轮询取源 PlayOneShot(叠加不打断),clip 为空静默跳过。</summary>
        private void Play(AudioClip clip)
        {
            if(clip == null) return;
            AudioSource src = _pool[_next];
            _next = (_next + 1) % _pool.Length;
            src.PlayOneShot(clip);
        }

        public void PlayClick()
        {
            Play(_clickClip);
        }

        public void PlayDraw()
        {
            Play(_drawClip);
        }

        public void PlayDive()
        {
            Play(_diveClip);
        }

        private void OnFishCaught(FishCaughtEvent evt)
        {
            Play(_catchClip);
        }

        /// <summary>扣盾才响(重置恢复时不响,避免每局开头误播)。</summary>
        private void OnSkillCountChanged(SkillCountChangedEvent evt)
        {
            if(evt.Left < evt.Total) Play(_shieldClip);
        }

        private void OnCountdownTick(CountdownTickEvent evt)
        {
            if(evt.Number > 0) Play(_tickClip);
        }

        private void OnRoundEnd(RoundEndEvent evt)
        {
            Play(_settleClip);
        }
    }
}