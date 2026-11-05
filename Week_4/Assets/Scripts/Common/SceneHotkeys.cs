using UnityEngine;
using UnityEngine.InputSystem;

namespace PhysicsDemo
{
    /// <summary>
    /// Keys shared by every demo scene:
    /// R = reset, Tab = info panel, T = slow motion, G = gizmos.
    /// </summary>
    public class SceneHotkeys : MonoBehaviour
    {
        public bool allowReset = true;

        void Update()
        {
            if (allowReset && InputUtil.KeyDown(Key.R)) SceneLoader.Reload();
            if (InputUtil.KeyDown(Key.Tab) && DemoHUD.Instance != null) DemoHUD.Instance.TogglePanels();
            if (InputUtil.KeyDown(Key.T)) TimeControl.SetSlowMotion(!TimeControl.IsSlowMotion);
            if (InputUtil.KeyDown(Key.G)) DebugLines.GizmosVisible = !DebugLines.GizmosVisible;
        }
    }
}
