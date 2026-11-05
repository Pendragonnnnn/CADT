using UnityEngine;
using UnityEngine.InputSystem;

namespace PhysicsDemo
{
    /// <summary>
    /// Moves a kinematic Rigidbody with the arrow keys (Z / X = down / up when enabled).
    /// Kinematic bodies ignore forces but push dynamic bodies, which is exactly what we want
    /// for a user-controlled "pusher" (e.g. the sphere under the cloth, the player in scene 08).
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class KeyboardMover : MonoBehaviour
    {
        public float speed = 3f;
        public bool allowVertical = false;
        public bool relativeToCamera = true;

        Rigidbody body;
        Vector3 input;

        void Awake()
        {
            body = GetComponent<Rigidbody>();
            body.isKinematic = true;
            body.interpolation = RigidbodyInterpolation.Interpolate;
        }

        void Update()
        {
            input = new Vector3(InputUtil.Axis(Key.LeftArrow, Key.RightArrow), 0f, InputUtil.Axis(Key.DownArrow, Key.UpArrow));
            if (relativeToCamera && Camera.main != null)
            {
                Vector3 fwd = Camera.main.transform.forward;
                fwd.y = 0f;
                if (fwd.sqrMagnitude > 0.001f) input = Quaternion.LookRotation(fwd.normalized) * input;
            }
            if (allowVertical) input.y = InputUtil.Axis(Key.Z, Key.X);
        }

        void FixedUpdate()
        {
            if (input.sqrMagnitude < 0.001f) return;
            // MovePosition moves a kinematic body so the solver can push dynamic bodies out of the way.
            body.MovePosition(body.position + input.normalized * (speed * Time.fixedDeltaTime));
        }
    }
}
