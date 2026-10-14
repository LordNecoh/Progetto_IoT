using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

//Alla fine non usato perchè dava problemi
//La logica sarebbe avere un file .json non pubblicato sul git che fa un po'  da .env per non mettere le chiavi in chiaro nel codice
public class SecretsManager : MonoBehaviour
{
    
    [System.Serializable]
    private class Secrets { public string geminiKey; public string ubidotsKey; }

    public static string GeminiKey { get; private set; }
    public static string UbidotsKey { get; private set; }

    private IEnumerator Start()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "secrets.json");
        string json = "";

        // Android richiede UnityWebRequest per leggere da StreamingAssets
        if (Application.platform == RuntimePlatform.Android)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(path))
            {
                yield return request.SendWebRequest();
                if (request.result == UnityWebRequest.Result.Success)
                    json = request.downloadHandler.text;
            }
        }
        else
        {
            if (File.Exists(path)) json = File.ReadAllText(path);
        }

        if (!string.IsNullOrEmpty(json))
        {
            Secrets s = JsonUtility.FromJson<Secrets>(json);
            GeminiKey = s.geminiKey;
            UbidotsKey = s.ubidotsKey;
        }
    }
}