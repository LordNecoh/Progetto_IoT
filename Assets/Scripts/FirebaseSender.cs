using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class FirebaseReportSender : MonoBehaviour
{
    [Header("Impostazioni Firebase")]
    [Tooltip("https://simulazionevr-default-rtdb.europe-west1.firebasedatabase.app/")]
    public string databaseURL = "https://simulazionevr-default-rtdb.europe-west1.firebasedatabase.app/";

    [Tooltip("Il nome del 'cassetto' dove mettere tutti i report")]
    public string cartellaSalvataggio = "Sessioni_VR";

    // Struttura dati (Classe) che definisce cosa vogliamo salvare nel report.
    // System.Serializable permette a Unity di trasformarla automaticamente in JSON.
    [System.Serializable]
    public class ReportSessione
    {
        public string id_utente;
        public string data_sessione;
        public float durata_minuti;
        public int battito_medio;
        public int numero_errori;
    }

    // Metodo pubblico da richiamare quando l'utente finisce la simulazione
    public void InviaReportFineSessione()
    {
        // 1. Popoliamo la struttura con dati fittizi per il nostro primo test
        ReportSessione report = new ReportSessione();
        report.id_utente = "Giocatore_1";
        // Cattura la data e l'ora esatta di fine partita
        report.data_sessione = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        report.durata_minuti = 14.5f;
        report.battito_medio = 82;
        report.numero_errori = 3;

        // 2. Trasformiamo l'oggetto C# in una stringa JSON
        string jsonPayload = JsonUtility.ToJson(report);

        // 3. Avviamo l'invio in background
        StartCoroutine(PostToFirebase(jsonPayload));
    }

    private IEnumerator PostToFirebase(string json)
    {
        // Regola d'oro di Firebase: l'URL finale DEVE finire con .json
        string url = $"{databaseURL}{cartellaSalvataggio}.json";

        // Usiamo il POST per far generare a Firebase un ID univoco per questo pacchetto
        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
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
                // Se ha successo, Firebase ci risponde con l'ID univoco che ha appena generato
                Debug.Log($"[FIREBASE INVIATO] Successo! Chiave generata: {request.downloadHandler.text}");
            }
        }
    }

}
