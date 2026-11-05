using UnityEngine;

namespace PhysicsDemo
{
    /// <summary>
    /// Pins cloth vertices in place. Each cloth vertex has a skinning coefficient "maxDistance":
    /// how far the simulated vertex may move away from its original (skinned) position.
    /// 0 = pinned (glued to the pole), float.MaxValue = completely free.
    /// MinXEdge pins the edge next to a flag pole; MaxYEdge pins the top edge of a hanging banner.
    /// </summary>
    [RequireComponent(typeof(Cloth))]
    public class ClothPinner : MonoBehaviour
    {
        public enum PinMode { None, MinXEdge, MaxYEdge }

        public PinMode pinMode = PinMode.MinXEdge;
        public float edgeTolerance = 0.01f;

        void Start()
        {
            var cloth = GetComponent<Cloth>();
            var mesh = GetComponent<SkinnedMeshRenderer>().sharedMesh;
            var coefficients = cloth.coefficients;
            Vector3[] verts = mesh.vertexCount == coefficients.Length ? mesh.vertices : cloth.vertices;

            float edge = pinMode == PinMode.MaxYEdge ? float.MinValue : float.MaxValue;
            foreach (var v in verts) edge = pinMode == PinMode.MaxYEdge ? Mathf.Max(edge, v.y) : Mathf.Min(edge, v.x);

            for (int i = 0; i < coefficients.Length; i++)
            {
                float coord = pinMode == PinMode.MaxYEdge ? verts[i].y : verts[i].x;
                bool pinned = pinMode != PinMode.None && Mathf.Abs(coord - edge) < edgeTolerance;
                coefficients[i].maxDistance = pinned ? 0f : float.MaxValue;
            }
            cloth.coefficients = coefficients;
        }
    }
}
