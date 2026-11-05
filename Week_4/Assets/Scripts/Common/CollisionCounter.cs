using UnityEngine;

namespace PhysicsDemo
{
    /// <summary>
    /// Counts the collisions this object receives and shows them on its <see cref="WorldLabel"/> (the thin walls in the
    /// Starter Sandbox). A static collider also gets OnCollisionEnter when a Rigidbody hits it, so a ball that
    /// tunnels through the wall adds no hit.
    /// </summary>
    public class CollisionCounter : MonoBehaviour
    {
        int hits;
        WorldLabel label;
        string baseText;

        void Start()
        {
            label = GetComponent<WorldLabel>();
            if (label == null) return;
            baseText = label.Text;
            label.Text = $"{baseText}\nhits: 0";
        }

        /// <summary>Sent by <see cref="StationReset"/>: start counting from zero again.</summary>
        void OnStationReset()
        {
            hits = 0;
            if (label != null) label.Text = $"{baseText}\nhits: 0";
        }

        void OnCollisionEnter(Collision collision)
        {
            hits++;
            string mode = collision.rigidbody != null ? collision.rigidbody.collisionDetectionMode.ToString() : "no Rigidbody";
            float speed = collision.relativeVelocity.magnitude;
            if (label != null) label.Text = $"{baseText}\nhits: {hits} (last: {mode})";
            Debug.Log($"[CollisionCounter] '{name}' was hit by '{collision.gameObject.name}' ({mode}, {speed:0} m/s)");
        }
    }
}
