using System.Collections;
using FishingGame.Core;
using UnityEngine;

namespace FishingGame.Controllers
{
    public class PlayerIntroController : MonoBehaviour
    {
        [SerializeField] private float _walkSpeed = 3f;
        [SerializeField] private string _walkState = "Walk";
        [SerializeField] private string _idleState = "Idle";
        [SerializeField] private bool _flipOnWalk = false;

        /// <summary>入场期间隐藏、到位后显现的装备物体(鱼竿/鱼钩/鱼线)。</summary>
        [SerializeField] private GameObject[] _equipObjects;

        /// <summary>到位后到拿出装备的间隔(秒)。</summary>
        [SerializeField] private float _drawDelay = 0.5f;

        /// <summary>装备显现后到发到位事件的间隔(秒)。</summary>
        [SerializeField] private float _readyDelay = 1f;

        private Animator _animator;
        private SpriteRenderer _sprite;
        private Vector3 _targetPos;
        private bool _defaultFlipX;
        private bool _walking;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _sprite = GetComponent<SpriteRenderer>();
            _targetPos = transform.position;
            if(_sprite != null) _defaultFlipX = _sprite.flipX;

            if(_equipObjects != null)
            {
                foreach(GameObject go in _equipObjects)
                {
                    if(go != null) go.SetActive(false);
                }
            }
        }

        private void Start()
        {
            ResetToIntro();
        }

        private void OnPhaseChanged(PhaseChangedEvent evt)
        {
            if(evt.Phase == GamePhase.Start) ResetToIntro();
        }

        private void ResetToIntro()
        {
            _walking = false;
            StopAllCoroutines();

            Camera cam = Camera.main;
            float halfWidth = cam != null ? cam.orthographicSize * cam.aspect : 8f;
            transform.position = new Vector3(_targetPos.x - halfWidth - 2f, _targetPos.y, _targetPos.z);

            if(_equipObjects != null)
            {
                foreach(GameObject go in _equipObjects)
                {
                    if(go != null) go.SetActive(false);
                }
            }
        }

        private void OnEnable()
        {
            EventBus.Instance.Subscribe<StartCommand>(OnStartCommand);
            EventBus.Instance.Subscribe<PhaseChangedEvent>(OnPhaseChanged);
        }

        private void OnDisable()
        {
            if(EventBus.Instance != null)
            {
                EventBus.Instance.Unsubscribe<StartCommand>(OnStartCommand);
                EventBus.Instance.Unsubscribe<PhaseChangedEvent>(OnPhaseChanged);
            }
        }

        private void OnStartCommand(StartCommand evt)
        {
            if(_walking) return;
            _walking = true;
            if(_sprite != null) _sprite.flipX = _flipOnWalk;
            if(_animator != null && !string.IsNullOrEmpty(_walkState)) _animator.CrossFade(_walkState, 0f);
        }

        private void Update()
        {
            if(!_walking) return;

            Vector3 pos = transform.position;
            pos.x = Mathf.MoveTowards(pos.x, _targetPos.x, _walkSpeed * Time.deltaTime);
            transform.position = pos;

            if(Mathf.Approximately(pos.x, _targetPos.x))
            {
                _walking = false;
                if(_sprite != null) _sprite.flipX = _defaultFlipX;
                if(_animator != null && !string.IsNullOrEmpty(_idleState)) _animator.CrossFade(_idleState, 0f);
                StartCoroutine(DrawOutEquipment());
            }
        }

        private IEnumerator DrawOutEquipment()
        {
            yield return new WaitForSeconds(_drawDelay);

            if(_equipObjects != null)
            {
                foreach(GameObject go in _equipObjects)
                {
                    if(go != null) go.SetActive(true);
                }
            }
            AudioManager.Instance?.PlayDraw();

            yield return new WaitForSeconds(_readyDelay);
            EventBus.Instance.Post(new PlayerArrivedEvent());
        }
    }
}