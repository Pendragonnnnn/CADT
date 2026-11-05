using UnityEngine;

namespace PhysicsDemo
{
    /// <summary>
    /// Spring-based mouse drag. On click we create an invisible kinematic "hand" body at the hit
    /// point and connect it to the clicked Rigidbody with a SpringJoint. Moving the hand pulls the
    /// object, so it still collides, swings on its joints and keeps its mass (unlike setting the
    /// transform directly, which would teleport it through other objects).
    /// </summary>
    public class MouseGrabber : MonoBehaviour
    {
        [Tooltip("Spring strength per kg of the grabbed body.")]
        public float springPerKg = 60f;
        [Tooltip("Damping per kg (stops the object oscillating around the cursor).")]
        public float damperPerKg = 6f;
        public float maxDistance = 100f;
        public LayerMask grabMask = ~0;

        Rigidbody hand;
        SpringJoint spring;
        Rigidbody grabbed;
        float grabDepth;
        Vector3 targetPoint;

        /// <summary>The body being dragged, or null.</summary>
        public Rigidbody Grabbed => grabbed;

        void Update()
        {
            if (InputUtil.LeftDown && !InputUtil.PointerOverUI) TryGrab();
            if (InputUtil.LeftUp) Release();

            if (grabbed != null)
            {
                Ray ray = InputUtil.MouseRay();
                targetPoint = ray.origin + ray.direction * grabDepth;
                Vector3 attach = grabbed.transform.TransformPoint(spring.connectedAnchor);
                DebugLines.Line(targetPoint, attach, new Color(1f, 0.5f, 1f));
                DebugLines.Cross(targetPoint, 0.1f, new Color(1f, 0.5f, 1f));
            }
        }

        void FixedUpdate()
        {
            // Kinematic bodies should be moved with MovePosition inside FixedUpdate.
            if (grabbed != null) hand.MovePosition(targetPoint);
        }

        void TryGrab()
        {
            Ray ray = InputUtil.MouseRay();
            if (!Physics.Raycast(ray, out RaycastHit hit, maxDistance, grabMask, QueryTriggerInteraction.Ignore)) return;
            Rigidbody body = hit.rigidbody;
            if (body == null || body.isKinematic) return;

            if (hand == null)
            {
                var go = new GameObject("MouseHand");
                hand = go.AddComponent<Rigidbody>();
                hand.isKinematic = true;
            }
            hand.position = hit.point;
            hand.transform.position = hit.point;
            targetPoint = hit.point;
            grabDepth = hit.distance;

            spring = hand.gameObject.AddComponent<SpringJoint>();
            spring.autoConfigureConnectedAnchor = false;
            spring.anchor = Vector3.zero;
            spring.connectedBody = body;
            spring.connectedAnchor = body.transform.InverseTransformPoint(hit.point);
            spring.spring = springPerKg * body.mass;
            spring.damper = damperPerKg * body.mass;
            spring.minDistance = 0f;
            spring.maxDistance = 0f;
            grabbed = body;
            body.WakeUp();
        }

        void Release()
        {
            if (spring != null) Destroy(spring);
            spring = null;
            grabbed = null;
        }
    }
}
