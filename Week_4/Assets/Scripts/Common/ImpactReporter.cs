using System;
using UnityEngine;

namespace PhysicsDemo
{
    /// <summary>
    /// Raises <see cref="Impact"/> the first time this body hits something after <see cref="Arm"/>.
    /// OnCollisionEnter is sent by the physics engine when two colliders start touching
    /// and at least one of them has a non-kinematic Rigidbody.
    /// </summary>
    public class ImpactReporter : MonoBehaviour
    {
        /// <summary>Fired once per Arm() with the collision data.</summary>
        public event Action<ImpactReporter, Collision> Impact;

        bool armed;

        /// <summary>Wait for the next impact.</summary>
        public void Arm() => armed = true;

        void OnCollisionEnter(Collision collision)
        {
            if (!armed) return;
            armed = false;
            Impact?.Invoke(this, collision);
        }
    }
}
