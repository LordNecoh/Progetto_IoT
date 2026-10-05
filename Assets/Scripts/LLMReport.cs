using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LLMReport : MonoBehaviour
{
    [Header("Impostazioni API Gemini")]
    private readonly string primaryModel = "gemini-3.1-flash-lite";
    private readonly string fallbackModel = "gemma-4-31b-it";

    [Header("UI Elementi")]
    public GameObject popupPanel;
    public TextMeshProUGUI reportText;

    public void OpenReportPopup()
    {
        popupPanel.SetActive(true);
        StartCoroutine(SendToGemini());
    }

    public void CloseReportPopup()
    {
        popupPanel.SetActive(false);
    }

    private IEnumerator SendToGemini()
    {
        DirectoryInfo directory = new DirectoryInfo(Application.persistentDataPath);
        FileInfo lastFile = directory.GetFiles("SimulationReport_*.csv")
                                     .OrderByDescending(f => f.LastWriteTime)
                                     .FirstOrDefault();

        if (lastFile == null)
        {
            reportText.text = "Nessun report trovato.";
            yield break;
        }

        string csvData = File.ReadAllText(lastFile.FullName);
        string prompt = "Analizza in modo sintetico questi dati di sessione e fornisci un breve riassunto per il paziente/utilizzatore:\n" + csvData;

        GeminiRequest requestData = new GeminiRequest
        {
            contents = new Content[] { new Content { parts = new Part[] { new Part { text = prompt } } } }
        };
        string jsonData = JsonUtility.ToJson(requestData);

        // Lista dei modelli da provare in ordine
        string[] modelsToTry = { primaryModel, fallbackModel };
        bool success = false;

        foreach (string currentModel in modelsToTry)
        {
            reportText.text = $"Analisi in corso con {currentModel}...\nAttendere prego.";
            string url = $"https://generativelanguage.googleapis.com/v1beta/models/{currentModel}:generateContent?key={SecretsManager.GeminiKey}";

            using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    GeminiResponse response = JsonUtility.FromJson<GeminiResponse>(request.downloadHandler.text);
                    if (response != null && response.candidates != null && response.candidates.Length > 0)
                    {
                        reportText.text = response.candidates[0].content.parts[0].text;
                        yield return null; // Attendere un frame per assicurarsi che il layout sia aggiornato
                        Canvas.ForceUpdateCanvases();
                        LayoutRebuilder.ForceRebuildLayoutImmediate(reportText.transform.parent.GetComponent<RectTransform>());
                        success = true;
                        break; // Esce dal ciclo, richiesta completata
                    }
                }
                else if (request.responseCode == 429 || request.responseCode == 503)
                {
                    // 429 = Too Many Requests, 503 = Service Unavailable
                    Debug.LogWarning($"[LLMReport] Modello {currentModel} non disponibile. Tento il prossimo...");
                    continue; // Passa al prossimo modello nel foreach
                }
                else
                {
                    // Altri errori (es. 400 Bad Request) bloccano l'esecuzione
                    reportText.text = $"Errore con {currentModel}: " + request.responseCode + "\n" + request.downloadHandler.text;
                    break;
                }
            }
        }

        if (!success && reportText.text.Contains("Analisi in corso"))
        {
            reportText.text = "Tutti i modelli sono attualmente sovraccarichi o non disponibili. Riprova tra poco.";
        }
    }

    [System.Serializable] private class GeminiRequest { public Content[] contents; }
    [System.Serializable] private class Content { public Part[] parts; }
    [System.Serializable] private class Part { public string text; }
    [System.Serializable] private class GeminiResponse { public Candidate[] candidates; }
    [System.Serializable] private class Candidate { public Content content; }
}