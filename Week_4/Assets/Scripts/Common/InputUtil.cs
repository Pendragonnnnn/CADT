using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace PhysicsDemo
{
    /// <summary>
    /// Thin wrapper around the new Input System (device polling) so every demo script
    /// reads input the same way and stays short. Safe to call when no keyboard/mouse exists.
    /// </summary>
    public static class InputUtil
    {
        /// <summary>True on the frame the key went down.</summary>
        public static bool KeyDown(Key key)
        {
            var kb = Keyboard.current;
            return kb != null && kb[key].wasPressedThisFrame;
        }

        /// <summary>True while the key is held.</summary>
        public static bool KeyHeld(Key key)
        {
            var kb = Keyboard.current;
            return kb != null && kb[key].isPressed;
        }

        /// <summary>-1, 0 or +1 depending on which of two keys is held.</summary>
        public static float Axis(Key negative, Key positive)
        {
            return (KeyHeld(positive) ? 1f : 0f) - (KeyHeld(negative) ? 1f : 0f);
        }

        /// <summary>Mouse position in screen pixels.</summary>
        public static Vector2 MousePosition => Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero;

        /// <summary>Mouse movement since last frame, in pixels.</summary>
        public static Vector2 MouseDelta => Mouse.current != null ? Mouse.current.delta.ReadValue() : Vector2.zero;

        /// <summary>Scroll wheel direction this frame: -1, 0 or +1 (platform independent).</summary>
        public static float ScrollSign
        {
            get
            {
                if (Mouse.current == null) return 0f;
                float y = Mouse.current.scroll.ReadValue().y;
                return Mathf.Abs(y) < 0.01f ? 0f : Mathf.Sign(y);
            }
        }

        public static bool LeftDown => Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
        public static bool LeftHeld => Mouse.current != null && Mouse.current.leftButton.isPressed;
        public static bool LeftUp => Mouse.current != null && Mouse.current.leftButton.wasReleasedThisFrame;
        public static bool RightHeld => Mouse.current != null && Mouse.current.rightButton.isPressed;
        public static bool MiddleDown => Mouse.current != null && Mouse.current.middleButton.wasPressedThisFrame;

        /// <summary>True when the mouse is over a UI element (so clicks should not fire into the world).</summary>
        public static bool PointerOverUI => EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();

        /// <summary>Ray from the main camera through the mouse cursor.</summary>
        public static Ray MouseRay()
        {
            var cam = Camera.main;
            return cam != null ? cam.ScreenPointToRay(MousePosition) : new Ray(Vector3.zero, Vector3.forward);
        }
    }
}
