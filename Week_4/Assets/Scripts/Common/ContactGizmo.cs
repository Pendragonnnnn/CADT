using System.Collections.Generic;
using UnityEngine;

namespace PhysicsDemo
{
    /// <summary>
    /// Shows contact points (red crosses) and contact normals (cyan lines) reported in
    /// OnCollisionStay. Each contact is where the solver pushes the two bodies apart. Toggle with G.
    /// </summary>
    public class ContactGizmo : MonoBehaviour
    {
        readonly List<ContactPoint> buffer = new List<ContactPoint>(16);
        readonly List<(Vector3 point, Vector3 normal)> contacts = new List<(Vector3, Vector3)>(16);
        float lastContactTime = -1f;

        void OnCollisionStay(Collision collision) => Record(collision);
        void OnCollisionEnter(Collision collision) => Record(collision);

        void Record(Collision collision)
        {
            if (Time.time != lastContactTime) contacts.Clear();
            lastContactTime = Time.time;
            collision.GetContacts(buffer);
            foreach (var c in buffer) contacts.Add((c.point, c.normal));
        }

        void Update()
        {
            // Contacts older than a few physics steps are stale (the bodies separated).
            if (!DebugLines.GizmosVisible || Time.time - lastContactTime > 0.1f) return;
            foreach (var (point, normal) in contacts)
            {
                DebugLines.Cross(point, 0.08f, Color.red);
                DebugLines.Line(point, point + normal * 0.4f, Color.cyan);
            }
        }
    }
}
