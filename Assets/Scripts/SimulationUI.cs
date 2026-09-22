using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class UDPHeartRateReceiver : MonoBehaviour
{
    private readonly int listenPort = 5000;
    public int currentHeartRate = 0;

    private UdpClient udpClient;
    private Thread receiveThread;
    private bool isRunning = false;

    void Start()
    {
        isRunning = true;
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    private void ReceiveData()
    {
        udpClient = new UdpClient(listenPort);
        IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, listenPort);

        while (isRunning)
        {
            try
            {
                // Mette in pausa il thread finché non arriva un pacchetto
                byte[] data = udpClient.Receive(ref anyIP);
                string text = Encoding.UTF8.GetString(data).Trim();

                if (int.TryParse(text, out int hr))
                {
                    currentHeartRate = hr;
                }
            }
            catch (Exception e)
            {
                if (isRunning) Debug.LogWarning($"UDP Error: {e.Message}");
            }
        }
    }

    void OnDestroy()
    {
        isRunning = false;
        if (udpClient != null)
        {
            udpClient.Close(); // Chiudere il client sblocca il thread dalla funzione Receive
        }
    }
}