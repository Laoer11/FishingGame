using FishingGame.Core;
using UnityEngine;

namespace FishingGame.Core
{
    /// <summary>调试按钮(OnGUI 直绘,零 UI 依赖):跳阶段/重开。⚠验收/发布前需从 SYSTEM 移除本组件。</summary>
    public class StateDebugButtons : MonoBehaviour
    {
        /// <summary>OnGUI 直绘两按钮:Next 循环推进阶段(超 Settlement 回 Countdown),Reset 广播重开命令。</summary>
        private void OnGUI()
        {
            if (GameManager.Instance == null || GameManager.Instance.Machine == null) return;
            GameStateMachine machine = GameManager.Instance.Machine;
            if (machine.Current == null) return;

            if (GUI.Button(new Rect(20, 20, 180, 60), "Next 推进"))
            {
                GamePhase next = machine.Current.Phase + 1;
                if (next > GamePhase.Settlement) next = GamePhase.Countdown;
                machine.SwitchTo(next);
            }

            if (GUI.Button(new Rect(20, 100, 180, 60), "Reset 重开"))
            {
                EventBus.Instance.Post(new ResetCommand());
            }
        }
    }
}
