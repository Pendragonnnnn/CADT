using System;
using UnityEngine;

namespace PhysicsDemo
{
    /// <summary>
    /// An enemy that "sees" with raycasts. It rotates and casts a fan of rays forward.
    /// Each ray stops at the FIRST collider it hits (within <see cref="sightMask"/>), so a wall
    /// in front of the player blocks line of sight. If a ray's first hit is on the Player layer,
    /// the player is spotted. Layers left out of the mask (e.g. Glass) are invisible to the rays.
    /// </summary>
    public class GuardVision : MonoBehaviour
    {
        [Header("Vision")]
        public float range = 14f;
        [Tooltip("Full width of the vision cone in degrees.")]
        public float coneAngle = 40f;
        [Range(1, 31)] public int rayCount = 9;
        public LayerMask sightMask = ~0;
        public string playerLayerName = "Player";

        [Header("Rotation")]
        [Tooltip("If >= 180 the guard spins continuously, otherwise it sweeps left/right by this angle.")]
        public float sweepAngle = 70f;
        public float turnSpeed = 40f;

        [Header("Feedback")]
        public Renderer bodyRenderer;
        public Color calmColor = new Color(0.2f, 0.8f, 0.3f);
        public Color alertColor = new Color(1f, 0.15f, 0.1f);

        /// <summary>True while any ray reaches the player.</summary>
        public bool PlayerSpotted { get; private set; }

        /// <summary>Distance and collider of the centre ray's hit (for the UI).</summary>
        public string CenterHitInfo { get; private set; } = "-";

        /// <summary>Fired on the frame the player first becomes visible.</summary>
        public event Action Spotted;

        int playerLayer;
        float baseYaw;
        float sweepTime;

        void Awake()
        {
            playerLayer = LayerMask.NameToLayer(playerLayerName);
            baseYaw = transform.eulerAngles.y;
        }

        void Update()
        {
            Rotate();
            Look();
        }

        void Rotate()
        {
            sweepTime += Time.deltaTime;
            float yaw = sweepAngle >= 180f
                ? baseYaw + sweepTime * turnSpeed
                : baseYaw + Mathf.Sin(sweepTime * turnSpeed * Mathf.Deg2Rad) * sweepAngle;
            transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        }

        void Look()
        {
            bool seen = false;
            Vector3 eye = transform.position;
            for (int i = 0; i < rayCount; i++)
            {
                float t = rayCount == 1 ? 0.5f : (float)i / (rayCount - 1);
                float angle = Mathf.Lerp(-coneAngle * 0.5f, coneAngle * 0.5f, t);
                Vector3 dir = Quaternion.AngleAxis(angle, Vector3.up) * transform.forward;
                bool isCenter = i == rayCount / 2;

                if (Physics.Raycast(eye, dir, out RaycastHit hit, range, sightMask, QueryTriggerInteraction.Ignore))
                {
                    bool hitPlayer = hit.collider.gameObject.layer == playerLayer;
                    seen |= hitPlayer;
                    DebugLines.Line(eye, hit.point, hitPlayer ? alertColor : new Color(1f, 1f, 0.3f, 0.8f));
                    DebugLines.Cross(hit.point, 0.1f, Color.white);
                    if (isCenter) CenterHitInfo = $"{hit.collider.name} at {hit.distance:0.0} m";
                }
                else
                {
                    DebugLines.Line(eye, eye + dir * range, new Color(0.4f, 1f, 0.4f, 0.5f));
                    if (isCenter) CenterHitInfo = "nothing";
                }
            }

            if (seen && !PlayerSpotted) Spotted?.Invoke();
            PlayerSpotted = seen;
            if (bodyRenderer != null) bodyRenderer.material.color = seen ? alertColor : calmColor;
        }
    }
}
