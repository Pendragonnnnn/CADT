using UnityEngine;

namespace PhysicsDemo
{
    /// <summary>
    /// Adds a "Camera focus" dropdown to the HUD that jumps the orbit camera to named areas
    /// of a scene (useful when a scene shows several setups side by side).
    /// </summary>
    public class CameraFocusMenu : MonoBehaviour
    {
        [System.Serializable]
        public struct Spot
        {
            public string name;
            public Vector3 point;
            public float distance;
            [Tooltip("Objects of this area, put back by StationReset. Leave empty (e.g. \"Overview\") to reset every area.")]
            public Transform root;
        }

        public OrbitCamera orbitCamera;
        public Spot[] spots;

        /// <summary>Index of the spot chosen in the dropdown.</summary>
        public int Current { get; private set; }

        // Dropdown choice kept across SceneLoader.Reload (the camera view itself is kept by OrbitCamera).
        static string savedScene;
        static int savedIndex;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics() => savedScene = null;

        void Start()
        {
            if (spots == null || spots.Length == 0 || orbitCamera == null) return;
            if (SceneLoader.IsReload && savedScene == gameObject.scene.name && savedIndex < spots.Length) Current = savedIndex;
            var names = new string[spots.Length];
            for (int i = 0; i < spots.Length; i++) names[i] = spots[i].name;
            DemoHUD.Instance.AddDropdown("Camera focus", names, Current, i =>
            {
                Current = i;
                orbitCamera.FocusOn(spots[i].point, spots[i].distance);
            });
        }

        void OnDestroy()
        {
            savedScene = gameObject.scene.name;
            savedIndex = Current;
        }
    }
}
