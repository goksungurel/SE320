using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    public Transform target;
    public float xOffset = 2f;
    public float smoothTime = 0.1f;
    public bool lockY = true;
    public float fixedY = 0f;

    Vector3 velocity;

    void LateUpdate()
    {
        if (!target) return;
        float y = lockY ? fixedY : target.position.y;
        Vector3 desired = new Vector3(target.position.x + xOffset, y, transform.position.z);
        transform.position = Vector3.SmoothDamp(transform.position, desired, ref velocity, smoothTime);
    }
}
