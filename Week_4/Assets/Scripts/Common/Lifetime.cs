using UnityEngine;

namespace PhysicsDemo
{
    /// <summary>Destroys the object after a delay (or when it falls far below the world).</summary>
    public class Lifetime : MonoBehaviour
    {
        public float seconds = 8f;
        public float killBelowY = -50f;

        void Start() => Destroy(gameObject, seconds);

        void Update()
        {
            if (transform.position.y < killBelowY) Destroy(gameObject);
        }
    }
}
