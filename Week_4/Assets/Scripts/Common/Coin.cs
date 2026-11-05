using System;
using UnityEngine;

namespace PhysicsDemo
{
    /// <summary>
    /// A collectible: its collider is a trigger, so things pass through it and we get
    /// OnTriggerEnter instead of a bounce. The coin disappears on the first valid touch.
    /// Remember: trigger messages need a Rigidbody on at least one of the two objects.
    /// </summary>
    public class Coin : MonoBehaviour
    {
        /// <summary>Raised for every coin collected in the scene.</summary>
        public static event Action<Coin, Collider> AnyCollected;

        [Tooltip("Leave empty to accept any Rigidbody, or set e.g. 'Player'.")]
        public string requiredTag = "";
        public float spinSpeed = 120f;

        void Update() => transform.Rotate(0f, spinSpeed * Time.deltaTime, 0f, Space.World);

        void OnTriggerEnter(Collider other)
        {
            if (other.attachedRigidbody == null) return;
            if (!string.IsNullOrEmpty(requiredTag) && !other.attachedRigidbody.CompareTag(requiredTag)) return;
            AnyCollected?.Invoke(this, other);
            gameObject.SetActive(false);
        }
    }
}
