using System.Collections.Generic;
using UnityEngine;

namespace PhysicsDemo
{
    /// <summary>
    /// Draws every collider under this object as a wireframe in the Game view, so students can see
    /// that the physics shape is not the same thing as the rendered mesh.
    /// Colours: box = green, sphere = cyan, capsule = magenta, mesh = orange, wheel = yellow, terrain = white.
    /// </summary>
    public class ColliderWireframe : MonoBehaviour
    {
        /// <summary>Global switch (UI toggle in scene 04).</summary>
        public static bool Visible = true;

        public int maxMeshEdges = 1500;

        Collider[] colliders;
        readonly Dictionary<Mesh, (Vector3[] verts, List<(int, int)> edges)> edgeCache = new Dictionary<Mesh, (Vector3[], List<(int, int)>)>();

        float nextRefresh;

        void Update()
        {
            // Re-scan twice a second so colliders added in the Inspector also get a wireframe.
            if (Time.unscaledTime >= nextRefresh)
            {
                colliders = GetComponentsInChildren<Collider>(true);
                nextRefresh = Time.unscaledTime + 0.5f;
            }
            if (!Visible) return;
            foreach (var c in colliders)
            {
                if (c == null || !c.enabled || !c.gameObject.activeInHierarchy) continue;
                Draw(c);
            }
        }

        void Draw(Collider c)
        {
            Transform t = c.transform;
            switch (c)
            {
                case BoxCollider box:
                    DebugLines.WireBox(t.localToWorldMatrix, box.center, box.size, Color.green);
                    break;
                case SphereCollider sphere:
                {
                    Vector3 s = t.lossyScale;
                    float r = sphere.radius * Mathf.Max(Mathf.Abs(s.x), Mathf.Abs(s.y), Mathf.Abs(s.z));
                    DebugLines.WireSphere(t.TransformPoint(sphere.center), r, Color.cyan);
                    break;
                }
                case CapsuleCollider capsule:
                    DrawCapsule(capsule);
                    break;
                case MeshCollider mesh when mesh.sharedMesh != null:
                    DrawMesh(t, mesh.sharedMesh, mesh.convex ? new Color(1f, 0.6f, 0.1f) : new Color(1f, 0.35f, 0.1f));
                    break;
                case WheelCollider wheel:
                {
                    wheel.GetWorldPose(out Vector3 pos, out Quaternion rot);
                    DebugLines.Circle(pos, rot * Vector3.right, wheel.radius, Color.yellow);
                    DebugLines.Line(t.TransformPoint(wheel.center), pos, Color.yellow);
                    break;
                }
                case TerrainCollider terrain when terrain.terrainData != null:
                    DrawTerrain(t, terrain.terrainData);
                    break;
            }
        }

        void DrawCapsule(CapsuleCollider capsule)
        {
            Transform t = capsule.transform;
            Vector3 axis = capsule.direction == 0 ? Vector3.right : capsule.direction == 1 ? Vector3.up : Vector3.forward;
            Vector3 s = t.lossyScale;
            float axisScale = Mathf.Abs(capsule.direction == 0 ? s.x : capsule.direction == 1 ? s.y : s.z);
            float radiusScale = capsule.direction == 0 ? Mathf.Max(Mathf.Abs(s.y), Mathf.Abs(s.z))
                : capsule.direction == 1 ? Mathf.Max(Mathf.Abs(s.x), Mathf.Abs(s.z)) : Mathf.Max(Mathf.Abs(s.x), Mathf.Abs(s.y));
            float r = capsule.radius * radiusScale;
            float half = Mathf.Max(0f, capsule.height * axisScale * 0.5f - r);
            Vector3 c = t.TransformPoint(capsule.center);
            Vector3 dir = t.TransformDirection(axis).normalized;
            DebugLines.WireCapsule(c - dir * half, c + dir * half, r, Color.magenta);
        }

        void DrawMesh(Transform t, Mesh mesh, Color color)
        {
            if (!edgeCache.TryGetValue(mesh, out var cached))
            {
                cached = (mesh.vertices, BuildEdges(mesh));
                edgeCache[mesh] = cached;
            }
            var (verts, edges) = cached;
            var m = t.localToWorldMatrix;
            foreach (var (a, b) in edges) DebugLines.Line(m.MultiplyPoint3x4(verts[a]), m.MultiplyPoint3x4(verts[b]), color);
        }

        List<(int, int)> BuildEdges(Mesh mesh)
        {
            var edges = new List<(int, int)>();
            var seen = new HashSet<long>();
            var tris = mesh.triangles;
            for (int i = 0; i < tris.Length && edges.Count < maxMeshEdges; i += 3)
            {
                for (int k = 0; k < 3; k++)
                {
                    int a = tris[i + k], b = tris[i + (k + 1) % 3];
                    long key = a < b ? ((long)a << 32) | (uint)b : ((long)b << 32) | (uint)a;
                    if (seen.Add(key)) edges.Add((a, b));
                }
            }
            return edges;
        }

        static void DrawTerrain(Transform t, TerrainData data)
        {
            const int steps = 16;
            Vector3 size = data.size;
            Color color = new Color(1f, 1f, 1f, 0.6f);
            for (int i = 0; i <= steps; i++)
            {
                for (int j = 0; j < steps; j++)
                {
                    float u0 = (float)j / steps, u1 = (float)(j + 1) / steps, v = (float)i / steps;
                    DebugLines.Line(TerrainPoint(t, data, size, u0, v), TerrainPoint(t, data, size, u1, v), color);
                    DebugLines.Line(TerrainPoint(t, data, size, v, u0), TerrainPoint(t, data, size, v, u1), color);
                }
            }
        }

        static Vector3 TerrainPoint(Transform t, TerrainData data, Vector3 size, float u, float v)
        {
            float h = data.GetInterpolatedHeight(u, v);
            return t.position + new Vector3(u * size.x, h + 0.05f, v * size.z);
        }
    }
}
