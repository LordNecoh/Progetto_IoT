using UnityEngine;

public class VRFollowPanel : MonoBehaviour
{
    public Transform vrCamera;

    void Update()
    {
        if (vrCamera == null) return;

        Vector3 lookAtPosition = vrCamera.position;
        lookAtPosition.y = transform.position.y;
        transform.LookAt(2 * transform.position - lookAtPosition);
    }
}