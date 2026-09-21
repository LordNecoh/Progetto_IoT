using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GazeTracker : MonoBehaviour
{
    private readonly float RegisterInterval = 0.5f;
    private List<string> GazeList = new List<string>();

    public Transform xrOriginRoot; // Riferimento al root dell'XR Origin

    void Start()
    {
        GazeList.Clear();
        GazeList.Add("Time, Direction, VisualAngle, LookedObject");
        InvokeRepeating(nameof(RegisterGaze), RegisterInterval, RegisterInterval);
    }

    void RegisterGaze()
    {
        //Checking if is looking back
        float visualAngle = Vector3.Angle(transform.forward, xrOriginRoot.forward);
        bool isLookingBack = visualAngle > 130f;

        //Checking the object being looked at
        Ray ray = new Ray(transform.position, transform.forward);
        string lookedObject = "None";

        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            lookedObject = hit.collider.gameObject.name;
        }

        //Saving the gaze information
        string direction = isLookingBack ? "[BACK]" : "[FORWARD]";
        string log = $"[{Time.time:F1}s] {direction} Looked at: {lookedObject}";

        GazeList.Add($"{Time.time:F1},{direction},{visualAngle:F1},{lookedObject}");
    }

    public void SaveCSVReport()
    {
        string fileName = $"Report_{System.DateTime.Now:yyyyMMdd_HHmmss}.csv";
        string path = Path.Combine(Application.persistentDataPath, fileName);

        File.WriteAllLines(path, GazeList);
        Debug.Log($"CSV saved in: {path}");
    }


}