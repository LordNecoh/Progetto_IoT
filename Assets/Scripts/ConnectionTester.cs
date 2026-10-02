using System;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class ConnectionTester : MonoBehaviour
{
    public int port = 8888;
    private UdpClient udpClient;

    async void Start()
    {
        Debug.Log($"[UDP DEBUG] 1. Tento di aprire la porta {port}...");

        try
        {
            udpClient = new UdpClient(port);
            Debug.Log("[UDP DEBUG] 2. Porta aperta con successo. In ascolto...");

            while (true)
            {
                // Aspetta i dati in modo asincrono senza bloccare Unity
                UdpReceiveResult result = await udpClient.ReceiveAsync();

                string text = Encoding.UTF8.GetString(result.Buffer);
                Debug.Log($"[UDP DEBUG] 3. RICEVUTO da {result.RemoteEndPoint.Address}: {text}");
            }
        }
        catch (SocketException se)
        {
            Debug.LogError($"[UDP DEBUG ERRORE] Codice {se.ErrorCode}: {se.Message}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[UDP DEBUG ERRORE] Generico: {e.Message}");
        }
    }

    void OnDestroy()
    {
        if (udpClient != null)
        {
            udpClient.Close();
            Debug.Log("[UDP DEBUG] 4. Socket chiuso.");
        }
    }
}