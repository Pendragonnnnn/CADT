using UnityEngine;

namespace PhysicsDemo
{
    /// <summary>
    /// Remembers where an object started so a demo can put it back (and stop it) without reloading the scene.
    /// </summary>
    public class ResettablePose : MonoBehaviour
    {
        Vector3 startPosition;
        Quaternion startRotation;
        Rigidbody body;

        void Awake()
        {
            startPosition = transform.position;
            startRotation = transform.rotation;
            body = GetComponent<Rigidbody>();
        }

        /// <summary>Teleports back to the start pose and clears velocity.</summary>
        public void ResetPose() => ResetPose(startPosition, startRotation);

        /// <summary>Teleports to a given pose and clears velocity.</summary>
        public void ResetPose(Vector3 position, Quaternion rotation)
        {
            if (body != null)
            {
                // Velocity can only be set on non-kinematic bodies.
                if (!body.isKinematic)
                {
                    body.linearVelocity = Vector3.zero;
                    body.angularVelocity = Vector3.zero;
                }
                body.position = position;
                body.rotation = rotation;
                body.WakeUp();
            }
            transform.SetPositionAndRotation(position, rotation);
        }

        public Vector3 StartPosition => startPosition;
    }
}
