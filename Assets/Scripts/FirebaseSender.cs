using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class FirebaseReportSender : MonoBehaviour
{
    [Header("Impostazioni Firebase")]
    public string databaseURL = "https://simulazionevr-default-rtdb.europe-west1.firebasedatabase.app/";
    public string cartellaSalvataggio = "Sessioni_VR";

    [Header("Riferimenti Script Dati")]
    public UDPReceiver ricevitoreDati; 
    public SpeedCalculator calcolatoreVelocita;

    [System.Serializable]
    public class Statistiche
    {
        public string unita; // <-- NUOVO: Aggiunge l'unità di misura nel JSON
        public double media;
        public double massima = -1;
        public string timestamp_max = "";
        public double minima = 9999;
        public string timestamp_min = "";
    }

    [System.Serializable]
    public class ReportSessione
    {
        public string data_inizio;
        public string durata_sessione; 
        public string distanza_percorsa; // <-- Convertito in stringa per ospitare la "m"
        public Statistiche frequenza_cardiaca;
        public Statistiche frequenza_respiratoria;
        public Statistiche velocita;
    }

    private bool sessioneAttiva = false;
    private float tempoInizio;
    private float distanzaTotale = 0f;
    private string idGiocatoreCorrente; 

    private float sommaCuore = 0, sommaRespiro = 0, sommaVelocita = 0;
    private int campionamenti = 0;

    private ReportSessione reportAttuale;

    void Start()
    {
        if (ricevitoreDati == null) ricevitoreDati = GetComponent<UDPReceiver>();
        if (calcolatoreVelocita == null) calcolatoreVelocita = GetComponent<SpeedCalculator>();

        AvviaSessione();
    }

    public void AvviaSessione()
    {
        sessioneAttiva = true;
        tempoInizio = Time.time;
        distanzaTotale = 0f;
        campionamenti = 0;
        sommaCuore = 0; sommaRespiro = 0; sommaVelocita = 0;

        idGiocatoreCorrente = "Giocatore_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss");

        reportAttuale = new ReportSessione
        {
            data_inizio = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            // Assegniamo le unità di misura corrette per ogni metrica
            frequenza_cardiaca = new Statistiche { unita = "bpm" },
            frequenza_respiratoria = new Statistiche { unita = "rpm" },
            velocita = new Statistiche { unita = "m/s" } 
        };

        InvokeRepeating(nameof(RaccogliDatiStatistici), 1f, 1f);
    }

    void Update()
    {
        if (!sessioneAttiva) return;

        if (calcolatoreVelocita != null)
        {
            distanzaTotale += calcolatoreVelocita.currentSpeed * Time.deltaTime;
        }
    }

    private void RaccogliDatiStatistici()
    {
        string oraAttuale = System.DateTime.Now.ToString("HH:mm:ss");

        if (ricevitoreDati != null)
        {
            AggiornaStatistica(reportAttuale.frequenza_cardiaca, ricevitoreDati.currentHeartRate, oraAttuale, ref sommaCuore);
            AggiornaStatistica(reportAttuale.frequenza_respiratoria, ricevitoreDati.currentRespiratoryRate, oraAttuale, ref sommaRespiro);
        }

        if (calcolatoreVelocita != null)
        {
            AggiornaStatistica(reportAttuale.velocita, calcolatoreVelocita.currentSpeed, oraAttuale, ref sommaVelocita);
        }

        campionamenti++;
    }

    private void AggiornaStatistica(Statistiche stat, float valoreAttuale, string timestamp, ref float somma)
    {
        if (valoreAttuale <= 0 && stat != reportAttuale.velocita) return;

        somma += valoreAttuale;

        double valoreArrotondato = System.Math.Round((double)valoreAttuale, 2);

        if (valoreArrotondato > stat.massima)
        {
            stat.massima = valoreArrotondato;
            stat.timestamp_max = timestamp;
        }

        if (valoreArrotondato < stat.minima)
        {
            stat.minima = valoreArrotondato;
            stat.timestamp_min = timestamp;
        }
    }

    public void InviaReportFineSessione()
    {
        if (!sessioneAttiva) return;
        
        sessioneAttiva = false;
        CancelInvoke(nameof(RaccogliDatiStatistici));

        // Formattazione Orologio
        float durataSecondi = Time.time - tempoInizio;
        System.TimeSpan t = System.TimeSpan.FromSeconds(durataSecondi);
        
        if (t.TotalHours >= 1)
            reportAttuale.durata_sessione = string.Format("{0:D2}:{1:D2}:{2:D2}", t.Hours, t.Minutes, t.Seconds);
        else
            reportAttuale.durata_sessione = string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);

        // Aggiunta unità di misura (m) alla distanza finale
        double distanzaArrotondata = System.Math.Round((double)distanzaTotale, 2);
        reportAttuale.distanza_percorsa = distanzaArrotondata.ToString() + " m";

        // Calcolo Medie
        if (campionamenti > 0)
        {
            reportAttuale.frequenza_cardiaca.media = System.Math.Round((double)(sommaCuore / campionamenti), 1);
            reportAttuale.frequenza_respiratoria.media = System.Math.Round((double)(sommaRespiro / campionamenti), 1);
            reportAttuale.velocita.media = System.Math.Round((double)(sommaVelocita / campionamenti), 2);
        }

        if (reportAttuale.frequenza_cardiaca.minima == 9999) reportAttuale.frequenza_cardiaca.minima = 0;
        if (reportAttuale.frequenza_respiratoria.minima == 9999) reportAttuale.frequenza_respiratoria.minima = 0;
        if (reportAttuale.velocita.minima == 9999) reportAttuale.velocita.minima = 0;

        string jsonPayload = JsonUtility.ToJson(reportAttuale);
        StartCoroutine(PutToFirebase(jsonPayload));
    }

    private IEnumerator PutToFirebase(string json)
    {
        string url = $"{databaseURL}{cartellaSalvataggio}/{idGiocatoreCorrente}.json";

        using (UnityWebRequest request = new UnityWebRequest(url, "PUT"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"[FIREBASE ERRORE] {request.error}");
            }
            else
            {
                Debug.Log($"[FIREBASE INVIATO] Successo! Cartella creata: {idGiocatoreCorrente}");
            }
        }
    }
}