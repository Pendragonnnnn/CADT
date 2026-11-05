using UnityEngine;
using UnityEngine.InputSystem;

namespace PhysicsDemo
{
    /// <summary>
    /// Fires a ball from the camera towards the mouse cursor using
    /// Rigidbody.AddForce(direction * impulse, ForceMode.Impulse).
    /// Impulse = an instant change of momentum (N·s), so the ball's speed = impulse / mass.
    /// </summary>
    public class Launcher : MonoBehaviour
    {
        public enum FireButton { LeftMouse, KeyF }

        public Rigidbody projectilePrefab;
        public FireButton fireButton = FireButton.LeftMouse;
        [Tooltip("Momentum given to the ball in N·s.")]
        public float impulse = 15f;
        public float spawnOffset = 1f;

        /// <summary>Number of balls fired since the scene started.</summary>
        public int ShotsFired { get; private set; }

        void Update()
        {
            bool pressed = fireButton == FireButton.LeftMouse
                ? InputUtil.LeftDown && !InputUtil.PointerOverUI
                : InputUtil.KeyDown(Key.F);
            if (pressed) Fire(InputUtil.MouseRay());
        }

        /// <summary>Spawns a projectile on the ray and applies the impulse.</summary>
        public Rigidbody Fire(Ray ray)
        {
            if (projectilePrefab == null) return null;
            Vector3 start = ray.origin + ray.direction * spawnOffset;
            Rigidbody ball = Instantiate(projectilePrefab, start, Quaternion.identity);
            // ForceMode.Impulse: applied once, ignores time step, depends on mass.
            ball.AddForce(ray.direction * impulse, ForceMode.Impulse);
            ShotsFired++;
            return ball;
        }
    }
}
