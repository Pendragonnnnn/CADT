using UnityEngine;
using UnityEngine.InputSystem;

namespace PhysicsDemo
{
    /// <summary>
    /// Free-orbit camera: right mouse drag = orbit, scroll = zoom, WASD = move, Q/E = down/up,
    /// Shift = faster. Uses unscaled time so it still moves smoothly in slow motion.
    /// The view is kept when the scene is reset with <see cref="SceneLoader.Reload"/>.
    /// </summary>
    public class OrbitCamera : MonoBehaviour
    {
        public Vector3 pivot = Vector3.zero;
        public float distance = 15f;
        public float yaw = 0f;
        public float pitch = 25f;

        public float orbitSpeed = 0.25f;
        public float moveSpeed = 8f;
        public float minDistance = 2f;
        public float maxDistance = 80f;
        [Tooltip("Turn off when another script (e.g. a car) needs the WASD keys.")]
        public bool keyboardMove = true;

        // View saved when the scene unloads, restored if the same scene comes back through SceneLoader.Reload.
        static string savedScene;
        static Vector3 savedPivot;
        static float savedDistance, savedYaw, savedPitch;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics() => savedScene = null;

        void Start()
        {
            if (!SceneLoader.IsReload || savedScene != gameObject.scene.name) return;
            pivot = savedPivot;
            distance = savedDistance;
            yaw = savedYaw;
            pitch = savedPitch;
            Apply();
        }

        void OnDestroy()
        {
            savedScene = gameObject.scene.name;
            savedPivot = pivot;
            savedDistance = distance;
            savedYaw = yaw;
            savedPitch = pitch;
        }

        void LateUpdate()
        {
            if (InputUtil.RightHeld)
            {
                Vector2 d = InputUtil.MouseDelta;
                yaw += d.x * orbitSpeed;
                pitch = Mathf.Clamp(pitch - d.y * orbitSpeed, -10f, 89f);
            }

            if (!InputUtil.PointerOverUI)
            {
                float scroll = InputUtil.ScrollSign;
                if (scroll != 0f) distance = Mathf.Clamp(distance * (1f - scroll * 0.1f), minDistance, maxDistance);
            }

            if (keyboardMove)
            {
                Quaternion flatRot = Quaternion.Euler(0f, yaw, 0f);
                Vector3 move = flatRot * new Vector3(InputUtil.Axis(Key.A, Key.D), 0f, InputUtil.Axis(Key.S, Key.W));
                move.y = InputUtil.Axis(Key.Q, Key.E);
                float speed = moveSpeed * (InputUtil.KeyHeld(Key.LeftShift) ? 3f : 1f);
                pivot += move * (speed * Time.unscaledDeltaTime);
            }

            Apply();
        }

        /// <summary>Moves the camera to look at a new point.</summary>
        public void FocusOn(Vector3 point, float newDistance)
        {
            pivot = point;
            distance = newDistance;
            Apply();
        }

        /// <summary>Positions the camera from pivot / yaw / pitch / distance.</summary>
        public void Apply()
        {
            Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);
            transform.SetPositionAndRotation(pivot - rot * Vector3.forward * distance, rot);
        }
    }
}
