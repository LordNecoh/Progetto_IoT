using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class UbidotsStreamer : MonoBehaviour
{
    [Header("Credenziali Ubidots")]
    private readonly string deviceLabel = "visore-vr";
    public readonly float sendInterval = 4.0f; 

    [Header("Sorgenti Dati (Trascina gli script qui)")]
    private UDPReceiver udpReceiver;
    private SpeedCalculator speedCalculator;
    public GazeTracker gazeTracker;

    private bool isStreaming = false;

    void Start()
    {
        isStreaming = true;
        StartCoroutine(StreamDataLoop());
        Debug.Log("Avviato invio dati verso Ubidots...");
        udpReceiver = GetComponent<UDPReceiver>();
        speedCalculator = GetComponent<SpeedCalculator>();
        //gazeTracker = GameObject.Find("Main Camera").GetComponent<GazeTracker>();
    }

    IEnumerator StreamDataLoop()
    {
        while (isStreaming)
        {
            if (udpReceiver != null && speedCalculator != null && gazeTracker != null)
            {
                //Medical values
                int heartRate = udpReceiver.currentHeartRate;
                int respRate = udpReceiver.currentRespiratoryRate;
                float speed = speedCalculator.currentSpeed;

                //Gaze variables
                float angle = gazeTracker.currentVisualAngle;
                string dir = gazeTracker.currentDirection;
                string obj = gazeTracker.currentLookedObject;


                int directionNum = 0;
                switch (gazeTracker.currentDirection)
                {
                    case "[FORWARD]":
                        directionNum = 0;
                        break;
                    case "[RIGHT]":
                        directionNum = 1;
                        break;
                    case "[LEFT]":
                        directionNum = 2;
                        break;
                    case "[BACK]":
                        directionNum = 3;
                        break;
                }

                //JSON payload for Ubidots
                // "context" is used by Ubidots to save text attached to a numeric value (we use the angle)
                string jsonPayload = $@"{{
                    ""heart_rate"": {heartRate},
                    ""respiratory_rate"": {respRate},
                    ""speed"": {speed.ToString("0.0", System.Globalization.CultureInfo.InvariantCulture)},
                    ""gaze_angle"": {angle.ToString("0.0", System.Globalization.CultureInfo.InvariantCulture)},
                    ""gaze_direction"": {directionNum},
                    ""looked_object"": {{
                        ""value"": 1,
                        ""context"": {{
                            ""item"": ""{obj}""
                        }}
                    }}
                }}";

                //Debug.Log($"UbidotsStreamer: JSON Payload: {jsonPayload}");

                yield return StartCoroutine(PostToUbidots(jsonPayload));
            }
            else
            {
                Debug.LogWarning("UbidotsStreamer: Mancano i riferimenti agli script sorgente (UDP, Speed, Gaze).");
                if (udpReceiver == null) Debug.LogWarning("UbidotsStreamer: UDPReceiver non trovato.");
                if (speedCalculator == null) Debug.LogWarning("UbidotsStreamer: SpeedCalculator non trovato.");
                if (gazeTracker == null) Debug.LogWarning("UbidotsStreamer: GazeTracker non trovato.");
            }

            yield return new WaitForSeconds(sendInterval);
        }
    }

    IEnumerator PostToUbidots(string json)
    {
        string url = $"https://things.ubidots.com/api/v1.6/devices/{deviceLabel}";

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();

            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("X-Auth-Token", SecretsManager.UbidotsKey);

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"[UBIDOTS ERROR] {request.error}");
            }
        }
    }

    void OnDestroy()
    {
        isStreaming = false;
    }
}