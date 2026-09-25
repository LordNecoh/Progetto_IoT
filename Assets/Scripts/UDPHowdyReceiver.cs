using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public class HowdyMeasurement
{
    public int MeasureType;
    public float Value;
}

public class UDPHowdyReceiver : MonoBehaviour
{
    private readonly int listenPort = 8090;

    [Header("Dati Ricevuti")]
    public int currentHeartRate = 0;
    public int currentRespiratoryRate = 0;

    private UdpClient udpClient;
    private CancellationTokenSource cancellationTokenSource;
    private readonly object dataLock = new object();

    void Start()
    {
        cancellationTokenSource = new CancellationTokenSource();
        udpClient = new UdpClient(listenPort);
        Debug.Log($"In ascolto UDP sulla porta {listenPort}...");
        _ = ReceiveLoopAsync(cancellationTokenSource.Token);
    }

    private async Task ReceiveLoopAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                UdpReceiveResult result = await udpClient.ReceiveAsync();
                string jsonText = Encoding.UTF8.GetString(result.Buffer).Trim();
                Debug.Log($"[UDP RICEVUTO]: {jsonText}");

                HowdyMeasurement data = JsonUtility.FromJson<HowdyMeasurement>(jsonText);

                lock (dataLock)
                {
                    if (data.MeasureType == 0)
                    {
                        currentHeartRate = Mathf.RoundToInt(data.Value);
                        Debug.Log($"❤️ Battito: {currentHeartRate} bpm");
                    }
                    else if (data.MeasureType == 1)
                    {
                        currentRespiratoryRate = Mathf.RoundToInt(data.Value);
                        Debug.Log($"🫁 Respiro: {currentRespiratoryRate}");
                    }
                }
            }
            catch (ArgumentException ex)
            {
                Debug.LogWarning($"JSON error: {ex.Message}");
            }
            catch (ObjectDisposedException)
            {
                break;
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"UDP Error: {ex.Message}");
            }
        }
    }

    void OnDestroy()
    {
        cancellationTokenSource?.Cancel();
        cancellationTokenSource?.Dispose();
        udpClient?.Close();
        udpClient?.Dispose();
    }
}