using UnityEngine;

namespace PhysicsDemo
{
    /// <summary>
    /// Draws each Joint on this object: a white cross at its anchor and a line to the connected
    /// anchor (on the other body, or a fixed point in the world when connectedBody is null). Toggle with G.
    /// </summary>
    public class JointGizmo : MonoBehaviour
    {
        public Color color = new Color(1f, 1f, 1f, 0.8f);

        Joint[] joints;

        void Start() => joints = GetComponents<Joint>();

        void Update()
        {
            if (!DebugLines.GizmosVisible) return;
            if (joints == null || joints.Length == 0 || joints[0] == null) joints = GetComponents<Joint>();
            foreach (var j in joints)
            {
                if (j == null) continue;
                Vector3 a = transform.TransformPoint(j.anchor);
                Vector3 b = j.connectedBody != null ? j.connectedBody.transform.TransformPoint(j.connectedAnchor) : j.connectedAnchor;
                DebugLines.Cross(a, 0.1f, color);
                DebugLines.Line(a, b, color);
            }
        }
    }
}
