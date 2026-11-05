using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PhysicsDemo
{
    /// <summary>
    /// Two resets for a scene split into areas by a <see cref="CameraFocusMenu"/> (the Starter Sandbox):
    /// R = reset only the area the camera is focused on, Shift+R = reload the whole scene (the camera keeps its view).
    /// An area reset puts every object back where it started, stops the Rigidbodies and restarts cloth and particles,
    /// but keeps the components added during Play, so the same demo can run again. It also sends "OnStationReset"
    /// to the area's objects (StartVelocity fires again, CollisionCounter starts from zero).
    /// A spot without a root (e.g. "Overview") resets every area.
    /// </summary>
    [DefaultExecutionOrder(10)] // Start after CameraFocusMenu, so the button sits under the dropdown.
    public class StationReset : MonoBehaviour
    {
        public CameraFocusMenu focusMenu;

        struct Pose
        {
            public Transform transform;
            public Vector3 position;
            public Quaternion rotation;
            public Vector3 scale;
        }

        readonly Dictionary<Transform, List<Pose>> startPoses = new Dictionary<Transform, List<Pose>>();

        void Start()
        {
            foreach (var spot in focusMenu.spots)
            {
                if (spot.root == null || startPoses.ContainsKey(spot.root)) continue;
                var poses = new List<Pose>();
                foreach (var t in spot.root.GetComponentsInChildren<Transform>(true))
                    poses.Add(new Pose { transform = t, position = t.localPosition, rotation = t.localRotation, scale = t.localScale });
                startPoses[spot.root] = poses;
            }
            if (DemoHUD.Instance != null) DemoHUD.Instance.AddButton("Reset station (R)", ResetFocused);
        }

        void Update()
        {
            if (!InputUtil.KeyDown(Key.R)) return;
            if (InputUtil.KeyHeld(Key.LeftShift) || InputUtil.KeyHeld(Key.RightShift)) SceneLoader.Reload();
            else ResetFocused();
        }

        /// <summary>Resets the area the camera is focused on, or every area when that spot has no root.</summary>
        public void ResetFocused()
        {
            var root = focusMenu.spots[focusMenu.Current].root;
            if (root != null) ResetArea(root);
            else foreach (var area in startPoses.Keys) ResetArea(area);
        }

        void ResetArea(Transform root)
        {
            if (!startPoses.TryGetValue(root, out var poses)) return;
            foreach (var pose in poses)
            {
                if (pose.transform == null) continue; // destroyed during Play
                pose.transform.SetLocalPositionAndRotation(pose.position, pose.rotation);
                pose.transform.localScale = pose.scale;
            }
            Physics.SyncTransforms();

            foreach (var body in root.GetComponentsInChildren<Rigidbody>())
            {
                // Velocity can only be set on non-kinematic bodies.
                if (body.isKinematic) continue;
                body.linearVelocity = Vector3.zero;
                body.angularVelocity = Vector3.zero;
                body.WakeUp();
            }
            foreach (var cloth in root.GetComponentsInChildren<Cloth>())
            {
                // Turning the cloth off and on restarts it from the mesh shape.
                cloth.enabled = false;
                cloth.enabled = true;
                cloth.ClearTransformMotion();
            }
            foreach (var particles in root.GetComponentsInChildren<ParticleSystem>())
            {
                particles.Clear(false);
                particles.Play(false);
            }
            root.BroadcastMessage("OnStationReset", SendMessageOptions.DontRequireReceiver);
        }
    }
}
