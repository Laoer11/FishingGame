using System.Collections.Generic;
using UnityEngine;

namespace FishingGame.Core
{
    /// <summary>游戏流程发动机。以 GamePhase 为键注册全部状态,驱动一局游戏从开始到结算的阶段流转。</summary>
    public class GameStateMachine
    {
        /// <summary>阶段 → 状态实例。由组合根 GameManager 在初始化时一次性注册齐全。</summary>
        private readonly Dictionary<GamePhase, GameState> _states = new Dictionary<GamePhase, GameState>();

        /// <summary>当前状态。启动前为 null。</summary>
        public GameState Current { get; private set; }

        /// <summary>注册一个状态,键为该状态的 Phase。由组合根(GameManager)在初始化时调用。</summary>
        public void Register(GameState state)
        {
            _states[state.Phase] = state;
        }

        /// <summary>切换状态。未注册的阶段或重复切当前状态时忽略。流程:OnExit → 换引用 → OnEnter → 广播。</summary>
        public void SwitchTo(GamePhase phase)
        {
            if (!_states.TryGetValue(phase, out GameState next) || next == Current) return;
            
            Current?.OnExit();
            Current = next;
            Debug.Log("[StateMachine] 切换 → " + phase);
            next.OnEnter();
            EventBus.Instance.Post(new PhaseChangedEvent(phase));
        }

        /// <summary>每帧驱动当前状态,由 GameManager.Update 调用。</summary>
        public void Tick(float dt)
        {
            Current?.OnUpdate(dt);
        }
    }
}