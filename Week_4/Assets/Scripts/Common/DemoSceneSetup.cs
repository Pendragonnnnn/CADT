using UnityEngine;

namespace PhysicsDemo
{
    /// <summary>
    /// Physics settings such as Physics.gravity and Time.timeScale are global and survive scene loads.
    /// This component puts them back to Unity's defaults when a scene starts, so one demo
    /// (e.g. the gravity slider in scene 01) never leaks into another.
    /// </summary>
    [DefaultExecutionOrder(-200)]
    public class DemoSceneSetup : MonoBehaviour
    {
        public Vector3 gravity = new Vector3(0f, -9.81f, 0f);

        void Awake()
        {
            Physics.gravity = gravity;
            TimeControl.ResetToNormal();
        }
    }
}
