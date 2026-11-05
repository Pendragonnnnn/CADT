using UnityEngine;

namespace PhysicsDemo
{
    /// <summary>
    /// Slow motion helper. Physics runs in fixed steps of Time.fixedDeltaTime (default 0.02 s = 50 Hz).
    /// When we slow time down we must shrink the fixed step by the same factor, otherwise physics
    /// would only update 10 times per real second and look choppy.
    /// </summary>
    public static class TimeControl
    {
        /// <summary>Default physics step (50 Hz).</summary>
        public const float NormalFixedDelta = 0.02f;

        /// <summary>Time scale used in slow motion.</summary>
        public const float SlowScale = 0.2f;

        /// <summary>True while slow motion is active.</summary>
        public static bool IsSlowMotion => Time.timeScale > 0f && Time.timeScale < 0.99f;

        /// <summary>Turns slow motion on or off and keeps the physics step in sync.</summary>
        public static void SetSlowMotion(bool on)
        {
            Time.timeScale = on ? SlowScale : 1f;
            Time.fixedDeltaTime = NormalFixedDelta * Time.timeScale;
        }

        /// <summary>Back to real-time speed with the default physics step.</summary>
        public static void ResetToNormal()
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = NormalFixedDelta;
        }
    }
}
