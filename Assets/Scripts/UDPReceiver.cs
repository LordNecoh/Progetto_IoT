using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;
using UnityEngine;

public class UDPReceiver : MonoBehaviour
{
    [Header("Impostazioni di Rete")]
    public int port = 8888; //Check corrispondence with howdy app!!!

    private UdpClient udpClient;
    private Thread receiveThread;
    private bool isRunning = false;

    private ConcurrentQueue<string> messageQueue = new ConcurrentQueue<string>();

    [Header("Dati Ricevuti")]
    public int currentHeartRate = 0;
    public int currentRespiratoryRate = 0;
    public Vector3 currentAcceleration = Vector3.zero;
    
    // NUOVO: Variabile per salvare l'array dell'ECG in formato testo
    public string currentEcgData = "[]"; 

    void Start()
    {
        isRunning = true;
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();

        Debug.Log($"In ascolto UDP sulla porta {port}...");
    }

    private void ReceiveData()
    {
        udpClient = new UdpClient(port);
        IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, port);

        while (isRunning)
        {
            try
            {
                byte[] data = udpClient.Receive(ref anyIP);
                string text = Encoding.UTF8.GetString(data);
                messageQueue.Enqueue(text);
            }
            catch (Exception e)
            {
                if (isRunning) Debug.LogWarning($"Errore di ricezione UDP: {e.Message}");
            }
        }
    }

    void Update()
    {
        while (messageQueue.TryDequeue(out string message))
        {
            string cleanJson = message.Replace(" ", "");

            // Dati Sensore ComfTech
            if (cleanJson.Contains("\"MeasureType\":0"))
            {
                currentHeartRate = ExtractSingleValue(cleanJson);
            }
            else if (cleanJson.Contains("\"MeasureType\":1"))
            {
                currentRespiratoryRate = ExtractSingleValue(cleanJson);
            }
            // NUOVO: Dati ECG (MeasureType 6)
            else if (cleanJson.Contains("\"MeasureType\":6"))
            {
                currentEcgData = ExtractArrayString(cleanJson);
            }
            // Dati Telefono (Accelerometro)
            else if (cleanJson.Contains("android.sensor.accelerometer"))
            {
                currentAcceleration = ExtractVector3(message);
            }
        }
    }

    // Estrae l'array di 3 valori dalle parentesi quadre
    private Vector3 ExtractVector3(string data)
    {
        int startIndex = data.IndexOf("[");
        int endIndex = data.IndexOf("]");

        if (startIndex == -1 || endIndex == -1) return Vector3.zero;

        string arrayContent = data.Substring(startIndex + 1, endIndex - startIndex - 1);
        string[] values = arrayContent.Split(',');

        if (values.Length >= 3)
        {
            float x = ParseFloatSafe(values[0]);
            float y = ParseFloatSafe(values[1]);
            float z = ParseFloatSafe(values[2]);
            return new Vector3(x, y, z);
        }
        return Vector3.zero;
    }

    private float ParseFloatSafe(string val)
    {
        if (float.TryParse(val.Trim(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out float result))
        {
            return result;
        }
        return 0f;
    }

    private int ExtractSingleValue(string json)
    {
        string searchStr = "\"Value\":";
        int startIndex = json.IndexOf(searchStr);
        if (startIndex == -1) return 0;

        startIndex += searchStr.Length;
        int endIndex = json.IndexOf(",", startIndex);
        if (endIndex == -1) endIndex = json.IndexOf("}", startIndex); // Se è l'ultimo elemento

        string valueStr = json.Substring(startIndex, endIndex - startIndex).Trim();

        if (float.TryParse(valueStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out float result))
        {
            return Mathf.RoundToInt(result);
        }

        return 0;
    }

    // NUOVO: Estrae l'intero array dell'ECG come stringa testuale (es. "[-0.02, -0.019, ... ]")
    private string ExtractArrayString(string json)
    {
        int startIndex = json.IndexOf("\"Value\":[");
        if (startIndex == -1) return "[]";
        
        startIndex += 8; // Salta la parola "Value": e si posiziona sulla parentesi [
        int endIndex = json.IndexOf("]", startIndex);
        if (endIndex == -1) return "[]";

        // Restituisce la stringa compresa tra le parentesi quadre
        return json.Substring(startIndex, endIndex - startIndex + 1);
    }

    void OnApplicationQuit()
    {
        isRunning = false;
        if (udpClient != null)
        {
            udpClient.Close();
        }
        if (receiveThread != null && receiveThread.IsAlive)
        {
            receiveThread.Abort();
        }
    }
}