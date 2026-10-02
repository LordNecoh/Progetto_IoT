using UnityEngine;

public class VRFollowPanel : MonoBehaviour
{
    public Transform vrCamera;

    void Update()
    {
        if (vrCamera == null) return;

        // Ruota il pannello per guardare la telecamera mantenendolo dritto verticalmente
        Vector3 lookAtPosition = vrCamera.position;
        lookAtPosition.y = transform.position.y;
        transform.LookAt(2 * transform.position - lookAtPosition);
    }
}