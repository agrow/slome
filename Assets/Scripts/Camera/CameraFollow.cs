using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;        // Assign the player transform here
    public Vector3 offset = new Vector3(0, 5, -10); // Offset from the player
    public float smoothSpeed = 0.125f; // How smooth the follow is

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        transform.LookAt(target); // Optional: keeps the camera facing the player
    }
}
