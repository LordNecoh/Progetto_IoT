using UnityEngine;

public class GazeTracker : MonoBehaviour
{
    public Transform xrOriginRoot;

    [Header("Dati in tempo reale")]
    public float currentVisualAngle;
    public string currentDirection = "[FORWARD]";
    public string currentLookedObject = "None";


    void Update()
    {
        float visualAngle = Vector3.Angle(transform.forward, xrOriginRoot.forward);
        bool isLookingBack = visualAngle > 110f;

        Ray ray = new Ray(transform.position, transform.forward);
        string lookedObject = "None";

        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            lookedObject = hit.collider.gameObject.name;
        }

        currentVisualAngle = visualAngle;
        currentDirection = isLookingBack ? "[BACK]" : "[FORWARD]";
        currentLookedObject = lookedObject;
    }
}