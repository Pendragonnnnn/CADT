using UnityEngine;

namespace PhysicsDemo
{
    /// <summary>
    /// Draws the segments queued in <see cref="DebugLines"/> with the low-level GL API.
    /// Created automatically the first time a line is requested; you never add it by hand.
    /// </summary>
    [DefaultExecutionOrder(-1000)]
    public class DebugLinesRenderer : MonoBehaviour
    {
        static DebugLinesRenderer s_Instance;
        Material lineMaterial;

        /// <summary>Creates the hidden renderer object if it does not exist yet.</summary>
        public static void EnsureExists()
        {
            if (s_Instance != null) return;
            var go = new GameObject("[DebugLinesRenderer]");
            go.hideFlags = HideFlags.DontSave;
            DontDestroyOnLoad(go);
            s_Instance = go.AddComponent<DebugLinesRenderer>();
        }

        void Awake()
        {
            // Material lives in Assets/Resources so it is always included in builds.
            // Its shader (PhysicsDemo/DebugLines) draws vertex colours on top of everything.
            var source = Resources.Load<Material>("DebugLines");
            lineMaterial = source != null ? new Material(source) : new Material(Shader.Find("Hidden/Internal-Colored"));
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
        }

        // Runs before every other script's Update: throw away last frame's lines.
        void Update() => DebugLines.Segments.Clear();

        void OnRenderObject()
        {
            if (DebugLines.Segments.Count == 0 || lineMaterial == null) return;
            lineMaterial.SetPass(0);
            GL.PushMatrix();
            GL.MultMatrix(Matrix4x4.identity);
            GL.Begin(GL.LINES);
            foreach (var s in DebugLines.Segments)
            {
                GL.Color(s.Color);
                GL.Vertex(s.A);
                GL.Vertex(s.B);
            }
            GL.End();
            GL.PopMatrix();
        }

        void OnDestroy()
        {
            if (lineMaterial != null) Destroy(lineMaterial);
            if (s_Instance == this) s_Instance = null;
        }
    }
}
