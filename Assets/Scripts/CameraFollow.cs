using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// STARTER — Camera follow for the "Orb Collector" lab goal.
/// Attach to the Main Camera. Drag the Player into the target field
/// (or let the script find it — compare both approaches live).
///
/// ALREADY WORKS: nothing yet — this one is entirely yours.
///
/// YOUR LAB TASKS:
///   TODO 3 support: move the follow logic here if you prefer, or do it in
///   PlayerController.LateUpdate(). Both are valid; LateUpdate is the key idea.
///   TODO 8 (stretch): use Input.GetAxis("Mouse ScrollWheel") to zoom the
///   offset length between minZoom and maxZoom.
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;     // drag the Player here

    [Header("Framing")]
    [SerializeField] private float followSharpness = 5f;

    [Header("Camera Zoom")]
    [SerializeField] private float cameraDistance = 3f;   // Current distance
    [SerializeField] private float minDistance = 1.5f;   // Closest the camera can zoom in
    [SerializeField] private float maxDistance = 8f;     // Farthest the camera can zoom out
    [SerializeField] private float scrollSensitivity = 5f; // Speed of the zoom

    [Header("Zoom (stretch)")]
    [SerializeField] private float minZoom = 6f;
    [SerializeField] private float maxZoom = 18f;

    private void Update()
    {
        // Scroll to zoom
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            cameraDistance -= scroll * scrollSensitivity;
            cameraDistance = Mathf.Clamp(cameraDistance, minDistance, maxDistance); // Keeps it within safe limits
        }
    }
}
