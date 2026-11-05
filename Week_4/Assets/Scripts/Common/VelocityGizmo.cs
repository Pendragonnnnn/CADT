using UnityEngine;

namespace PhysicsDemo
{
    /// <summary>
    /// Draws the Rigidbody's velocity as a yellow arrow (visible in the Game view, toggle with G).
    /// Arrow length = velocity (m/s) x <see cref="scale"/>.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class VelocityGizmo : MonoBehaviour
    {
        public float scale = 0.15f;
        public Color color = new Color(1f, 0.9f, 0.2f);

        Rigidbody body;

        void Awake() => body = GetComponent<Rigidbody>();

        void Update()
        {
            if (!DebugLines.GizmosVisible || body.isKinematic) return;
            DebugLines.Arrow(body.worldCenterOfMass, body.linearVelocity * scale, color);
        }
    }
}
