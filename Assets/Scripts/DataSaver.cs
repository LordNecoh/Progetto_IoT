using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataSaver : MonoBehaviour
{
    [Header("Sorgenti Dati")]
    private UDPReceiver udpReceiver;
    private SpeedCalculator speedCalculator;
    public GazeTracker gazeTracker;

    private readonly float recordInterval = 0.5f;
    private List<string> csvData = new List<string>();

    void Start()
    {
        csvData.Clear();
        // Intestazione del CSV globale
        csvData.Add("Time,HeartRate,RespiratoryRate,Speed,GazeDirection,VisualAngle,LookedObject");

        InvokeRepeating(nameof(RecordData), recordInterval, recordInterval);
        speedCalculator = GetComponent<SpeedCalculator>();
        //gazeTracker = GameObject.Find("Main Camera").GetComponent<GazeTracker>();
        udpReceiver = GetComponent<UDPReceiver>();

    }

    void RecordData()
    {
        if (udpReceiver == null || speedCalculator == null || gazeTracker == null) return;

        string time = Time.time.ToString("F1");
        int hr = udpReceiver.currentHeartRate;
        int rr = udpReceiver.currentRespiratoryRate;
        float speed = speedCalculator.currentSpeed;

        string dir = gazeTracker.currentDirection;
        float angle = gazeTracker.currentVisualAngle;
        string obj = gazeTracker.currentLookedObject;

        // Aggiunge la riga con tutti i dati formattati
        csvData.Add($"{time},{hr},{rr},{speed:F1},{dir},{angle:F1},{obj}");
    }

    public void SaveCSVReport()
    {
        string fileName = $"SimulationReport_{System.DateTime.Now:yyyyMMdd_HHmmss}.csv";
        string path = Path.Combine(Application.persistentDataPath, fileName);

        File.WriteAllLines(path, csvData);
        Debug.Log($"CSV globale salvato in: {path}");
    }
}