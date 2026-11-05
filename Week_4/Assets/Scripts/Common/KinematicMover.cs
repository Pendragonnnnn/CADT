using UnityEngine;

namespace PhysicsDemo
{
    /// <summary>
    /// Drives a kinematic Rigidbody back and forth (and optionally spins it) with
    /// Rigidbody.MovePosition / MoveRotation. Kinematic = moved by script, not by forces:
    /// gravity and impacts do not affect it, but it still pushes dynamic bodies it touches.
    /// If the body is switched to dynamic at runtime, this script stops driving it.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class KinematicMover : MonoBehaviour
    {
        [Tooltip("Offset from the start position at the far end of the path.")]
        public Vector3 travel = new Vector3(4f, 0f, 0f);
        [Tooltip("Seconds for a full there-and-back cycle.")]
        public float period = 4f;
        public Vector3 spinDegreesPerSecond = Vector3.zero;

        Rigidbody body;
        Vector3 start;
        float time;

        /// <summary>Current platform velocity (used by the game's player to ride along).</summary>
        public Vector3 Velocity { get; private set; }

        void Awake()
        {
            body = GetComponent<Rigidbody>();
            start = body.position;
        }

        void FixedUpdate()
        {
            if (!body.isKinematic)
            {
                Velocity = body.linearVelocity;
                return;
            }

            time += Time.fixedDeltaTime;
            // Smooth 0 -> 1 -> 0 curve so the platform slows down at each end.
            float t = (1f - Mathf.Cos(time * Mathf.PI * 2f / period)) * 0.5f;
            Vector3 target = start + travel * t;
            Velocity = (target - body.position) / Time.fixedDeltaTime;
            body.MovePosition(target);

            if (spinDegreesPerSecond != Vector3.zero)
                body.MoveRotation(body.rotation * Quaternion.Euler(spinDegreesPerSecond * Time.fixedDeltaTime));
        }
    }
}
