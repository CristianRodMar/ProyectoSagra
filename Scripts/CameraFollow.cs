using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float maxDistanceFromTarget = 2f;
    public float smoothTime = 0.2f;
    public float cameraOffset = -10;
    Vector3 offset;
    Vector3 desiredPosition;
    Vector3 smoothedPosition;
    Vector3 Velocity = Vector3.zero;
    PlayerController playerController;

    private void Awake()
    {
        playerController = FindObjectOfType<PlayerController>();
        offset = new Vector3(0, 0, cameraOffset);
    }

    private void LateUpdate()
    {
        Vector3 targetPosition = target.position;
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = -Camera.main.transform.position.z;
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector3 targetToMouse = mouseWorldPosition - targetPosition;
        targetToMouse.z = 0;
        if (targetToMouse.magnitude > maxDistanceFromTarget)
        {
            targetToMouse = targetToMouse.normalized * maxDistanceFromTarget;
        }
        desiredPosition = targetPosition + targetToMouse + offset;
        if (playerController.isAiming)
        {
            smoothTime = 0.5f;
            maxDistanceFromTarget = 4.5f;
        } else
        {
            smoothTime = 0.2f;
            maxDistanceFromTarget = 2f;
        }
        smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref Velocity, smoothTime);
        transform.position = smoothedPosition;
    }
}

