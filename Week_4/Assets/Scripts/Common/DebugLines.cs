using System.Collections.Generic;
using UnityEngine;

namespace PhysicsDemo
{
    /// <summary>
    /// Immediate-mode line drawing that is visible in the Game view (Debug.DrawLine and Gizmos
    /// only show in the Scene view). Call the methods from Update or LateUpdate every frame;
    /// the list is cleared automatically at the start of the next frame.
    /// </summary>
    public static class DebugLines
    {
        /// <summary>Global "gizmo" switch toggled with the G key (velocity arrows, contact points...).</summary>
        public static bool GizmosVisible = true;

        internal struct Segment
        {
            public Vector3 A, B;
            public Color Color;
        }

        internal static readonly List<Segment> Segments = new List<Segment>(1024);

        /// <summary>Draws one line segment this frame.</summary>
        public static void Line(Vector3 a, Vector3 b, Color color)
        {
            DebugLinesRenderer.EnsureExists();
            Segments.Add(new Segment { A = a, B = b, Color = color });
        }

        /// <summary>Draws an arrow starting at <paramref name="from"/> along <paramref name="vector"/>.</summary>
        public static void Arrow(Vector3 from, Vector3 vector, Color color)
        {
            if (vector.sqrMagnitude < 1e-6f) return;
            Vector3 to = from + vector;
            Line(from, to, color);
            Vector3 dir = vector.normalized;
            Vector3 side = Vector3.Cross(dir, Mathf.Abs(dir.y) > 0.9f ? Vector3.right : Vector3.up).normalized;
            float head = Mathf.Min(0.25f, vector.magnitude * 0.3f);
            Line(to, to - dir * head + side * head * 0.5f, color);
            Line(to, to - dir * head - side * head * 0.5f, color);
        }

        /// <summary>Small 3-axis cross, handy for marking points.</summary>
        public static void Cross(Vector3 p, float size, Color color)
        {
            Line(p - Vector3.right * size, p + Vector3.right * size, color);
            Line(p - Vector3.up * size, p + Vector3.up * size, color);
            Line(p - Vector3.forward * size, p + Vector3.forward * size, color);
        }

        /// <summary>Circle around <paramref name="normal"/>.</summary>
        public static void Circle(Vector3 center, Vector3 normal, float radius, Color color, int segments = 24)
        {
            Vector3 u = Vector3.Cross(normal, Mathf.Abs(normal.y) > 0.9f ? Vector3.right : Vector3.up).normalized;
            Vector3 v = Vector3.Cross(normal, u).normalized;
            Vector3 prev = center + u * radius;
            for (int i = 1; i <= segments; i++)
            {
                float a = i * Mathf.PI * 2f / segments;
                Vector3 next = center + (u * Mathf.Cos(a) + v * Mathf.Sin(a)) * radius;
                Line(prev, next, color);
                prev = next;
            }
        }

        /// <summary>Three great circles approximating a sphere.</summary>
        public static void WireSphere(Vector3 center, float radius, Color color)
        {
            Circle(center, Vector3.up, radius, color);
            Circle(center, Vector3.right, radius, color);
            Circle(center, Vector3.forward, radius, color);
        }

        /// <summary>Box defined in the local space of <paramref name="localToWorld"/>.</summary>
        public static void WireBox(Matrix4x4 localToWorld, Vector3 center, Vector3 size, Color color)
        {
            Vector3 h = size * 0.5f;
            var c = new Vector3[8];
            for (int i = 0; i < 8; i++)
            {
                var local = center + new Vector3((i & 1) == 0 ? -h.x : h.x, (i & 2) == 0 ? -h.y : h.y, (i & 4) == 0 ? -h.z : h.z);
                c[i] = localToWorld.MultiplyPoint3x4(local);
            }
            int[] edges = { 0, 1, 2, 3, 4, 5, 6, 7, 0, 2, 1, 3, 4, 6, 5, 7, 0, 4, 1, 5, 2, 6, 3, 7 };
            for (int i = 0; i < edges.Length; i += 2) Line(c[edges[i]], c[edges[i + 1]], color);
        }

        /// <summary>Capsule between two sphere centres.</summary>
        public static void WireCapsule(Vector3 p0, Vector3 p1, float radius, Color color)
        {
            Vector3 axis = p1 - p0;
            Vector3 dir = axis.sqrMagnitude > 1e-6f ? axis.normalized : Vector3.up;
            Vector3 u = Vector3.Cross(dir, Mathf.Abs(dir.y) > 0.9f ? Vector3.right : Vector3.up).normalized;
            Vector3 v = Vector3.Cross(dir, u);
            Circle(p0, dir, radius, color);
            Circle(p1, dir, radius, color);
            Line(p0 + u * radius, p1 + u * radius, color);
            Line(p0 - u * radius, p1 - u * radius, color);
            Line(p0 + v * radius, p1 + v * radius, color);
            Line(p0 - v * radius, p1 - v * radius, color);
            HalfCircle(p1, u, dir, radius, color);
            HalfCircle(p1, v, dir, radius, color);
            HalfCircle(p0, u, -dir, radius, color);
            HalfCircle(p0, v, -dir, radius, color);
        }

        static void HalfCircle(Vector3 c, Vector3 a, Vector3 b, float r, Color color)
        {
            Vector3 prev = c + a * r;
            for (int i = 1; i <= 12; i++)
            {
                float t = i * Mathf.PI / 12f;
                Vector3 next = c + (a * Mathf.Cos(t) + b * Mathf.Sin(t)) * r;
                Line(prev, next, color);
                prev = next;
            }
        }
    }
}
