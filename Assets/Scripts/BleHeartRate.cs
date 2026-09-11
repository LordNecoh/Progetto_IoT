using UnityEngine;

public class BleHeartRate : MonoBehaviour
{
    [SerializeField]
    private string targetDeviceName = "SensoreVR";
    private string targetDeviceId = null;
    private bool isScanning = false;
    private bool isSubscribed = false;

    void Start()
    {
        Debug.Log("Avvio scansione BLE...");
        BleApi.StartDeviceScan();
        isScanning = true;
    }

    void Update()
    {
        // 1. Fase di scansione
        if (isScanning && string.IsNullOrEmpty(targetDeviceId))
        {
            BleApi.DeviceUpdate res = new BleApi.DeviceUpdate();
            BleApi.ScanStatus status = BleApi.PollDevice(ref res, false);

            if (status == BleApi.ScanStatus.AVAILABLE)
            {
                if (!string.IsNullOrEmpty(res.name))
                {
                    Debug.Log($"Dispositivo rilevato: {res.name}");
                }

                if (res.name == targetDeviceName)
                {
                    targetDeviceId = res.id;
                    isScanning = false;
                    BleApi.StopDeviceScan();
                    Debug.Log($"Trovato {targetDeviceName}! Invio richiesta di connessione...");
                }
            }
        }

        // 2. Iscrizione alla caratteristica
        if (!string.IsNullOrEmpty(targetDeviceId) && !isSubscribed)
        {
            BleApi.SubscribeCharacteristic(
                targetDeviceId,
                "{0000180D-0000-1000-8000-00805f9b34fb}",
                "{00002A37-0000-1000-8000-00805f9b34fb}",
                false);

            isSubscribed = true;
            Debug.Log("Sottoscrizione inviata! In attesa di notifiche...");
        }

        // 3. Lettura dati
        if (isSubscribed)
        {
            BleApi.BLEData data = new BleApi.BLEData();
            while (BleApi.PollData(out data, false)) // Usa 'while' per svuotare la coda
            {
                if (data.buf != null)
                {
                    Debug.Log($"Messaggio ricevuto! Lunghezza in byte: {data.buf.Length}");

                    if (data.buf.Length >= 2)
                    {
                        int bpm = data.buf[1];
                        Debug.Log($"[BPM]: {bpm}");
                    }
                    else if (data.buf.Length == 1)
                    {
                        // Nel caso tu stia inviando un solo byte
                        int bpm = data.buf[0];
                        Debug.Log($"[BPM]: {bpm}");
                    }
                }
            }
        }
    }

    void OnApplicationQuit()
    {
        if (isScanning)
        {
            BleApi.StopDeviceScan();
        }
        BleApi.Quit();
    }
}