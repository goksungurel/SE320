using UnityEngine;

public class GroundLooper : MonoBehaviour
{
    public Transform cameraT;
    public float width = 40f;

    void Update()
    {
        if (cameraT.position.x - transform.position.x > width)
            transform.position += new Vector3(width * 2f, 0f, 0f);
    }
}

