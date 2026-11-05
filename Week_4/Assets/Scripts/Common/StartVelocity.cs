using UnityEngine;
using UnityEngine.InputSystem;

namespace PhysicsDemo
{
    /// <summary>
    /// Gives this object's Rigidbody a starting velocity (the fast balls in the Starter Sandbox).
    /// It launches as soon as the object has a Rigidbody, so the Rigidbody can be added before Play or during Play.
    /// The relaunch key puts the object back at its start and launches it again, e.g. after changing Collision Detection.
    /// </summary>
    public class StartVelocity : MonoBehaviour
    {
        [Tooltip("World-space velocity in m/s, applied once per launch.")]
        public Vector3 velocity = new Vector3(100f, 0f, 0f);
        [Tooltip("Puts the object back at its start position and launches it again.")]
        public Key relaunchKey = Key.Space;

        Vector3 startPosition;
        Quaternion startRotation;
        bool launched;
        bool relaunch;

        void Start()
        {
            startPosition = transform.position;
            startRotation = transform.rotation;
            if (!TryGetComponent(out Rigidbody _))
                Debug.Log($"[StartVelocity] '{name}' has no Rigidbody yet: add one (also works during Play) to launch it.");
        }

        void Update()
        {
            if (InputUtil.KeyDown(relaunchKey)) relaunch = true;
        }

        /// <summary>Sent by <see cref="StationReset"/>: back to the start and launch again.</summary>
        void OnStationReset() => relaunch = true;

        void FixedUpdate()
        {
            TryGetComponent(out Rigidbody rb);
            if (relaunch)
            {
                relaunch = false;
                launched = false;
                transform.SetPositionAndRotation(startPosition, startRotation);
                if (rb != null)
                {
                    rb.position = startPosition;
                    rb.rotation = startRotation;
                    if (!rb.isKinematic) rb.angularVelocity = Vector3.zero;
                }
            }
            if (rb == null || rb.isKinematic || launched) return;
            rb.linearVelocity = velocity;
            launched = true;
        }
    }
}
