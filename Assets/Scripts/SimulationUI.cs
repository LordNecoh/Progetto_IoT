using UnityEngine;
using UnityEngine.SceneManagement;

public class SimulationUI : MonoBehaviour
{
    [Header("Riferimenti")]
    private DataSaver dataSaver;
    public string nomeScenaMenu = "MainMenu"; // Inserisci il nome esatto della scena del menù
    
    private void Start()
    {
        dataSaver = GameObject.Find("GameManager").GetComponent<DataSaver>();
        if (dataSaver == null)
        {
            Debug.LogError("SimulationUI: DataSaver non trovato nel GameManager!");
        }
    }

    // Questa funzione va collegata all'evento OnClick() del bottone "Esci"
    public void EsciDallaSimulazione()
    {
        // 1. Salva il report CSV prima di cambiare scena
        if (dataSaver != null)
        {
            dataSaver.SaveCSVReport();
        }
        else
        {
            Debug.LogWarning("GazeTracker non assegnato in SimulationUI!");
        }

        // 2. Carica la scena del menù principale
        SceneManager.LoadScene(nomeScenaMenu, LoadSceneMode.Single);
    }
}