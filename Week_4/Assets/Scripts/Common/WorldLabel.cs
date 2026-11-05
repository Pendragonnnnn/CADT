using UnityEngine;
using UnityEngine.UI;

namespace PhysicsDemo
{
    /// <summary>
    /// Screen-space text that follows a 3D object (e.g. "Dynamic", "mass = 100").
    /// Other scripts can change <see cref="Text"/> at runtime.
    /// </summary>
    public class WorldLabel : MonoBehaviour
    {
        [TextArea] public string text = "Label";
        public Vector3 offset = new Vector3(0f, 1.2f, 0f);
        public int fontSize = 15;
        public Color color = Color.white;
        [Tooltip("Hide the label when the camera is further away than this (0 = always show).")]
        public float maxDistance = 0f;

        static Transform s_FallbackLayer;
        UnityEngine.UI.Text uiText;

        /// <summary>Text shown above the object.</summary>
        public string Text
        {
            get => text;
            set
            {
                text = value;
                if (uiText != null) uiText.text = value;
            }
        }

        void Start()
        {
            Transform layer = DemoHUD.Instance != null ? DemoHUD.Instance.LabelLayer : FallbackLayer();
            uiText = UIFactory.CreateText(layer, text, fontSize, color, FontStyle.Bold, TextAnchor.LowerCenter);
            uiText.rectTransform.sizeDelta = new Vector2(260, 60);
            uiText.rectTransform.pivot = new Vector2(0.5f, 0f);
            // Hard 1-pixel shadow keeps labels readable on any background without softening the text.
            var shadow = uiText.gameObject.AddComponent<Shadow>();
            shadow.effectColor = new Color(0, 0, 0, 0.95f);
            shadow.effectDistance = new Vector2(1f, -1f);
        }

        void LateUpdate()
        {
            if (uiText == null) return;
            var cam = Camera.main;
            if (cam == null) return;
            Vector3 screen = cam.WorldToScreenPoint(transform.position + offset);
            bool visible = screen.z > 0f && (maxDistance <= 0f || screen.z < maxDistance);
            uiText.enabled = visible;
            // For a Screen Space Overlay canvas, world position == screen pixels.
            if (!visible) return;
            // Round to whole pixels so moving labels don't blur between pixels.
            uiText.rectTransform.position = new Vector3(Mathf.Round(screen.x), Mathf.Round(screen.y), 0f);
            uiText.color = color;
        }

        // Scenes without a DemoHUD (the game) share one label canvas.
        static Transform FallbackLayer()
        {
            if (s_FallbackLayer == null) s_FallbackLayer = UIFactory.CreateCanvas("World Labels", 0).transform;
            return s_FallbackLayer;
        }

        void OnDisable()
        {
            if (uiText != null) uiText.enabled = false;
        }

        void OnDestroy()
        {
            if (uiText != null) Destroy(uiText.gameObject);
        }
    }
}
