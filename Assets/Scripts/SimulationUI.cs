using UnityEngine;
using UnityEngine.SceneManagement;

public class SimulationUI : MonoBehaviour
{
    [Header("Riferimenti")]
    public GazeTracker trackerSguardo; // Trascina qui l'oggetto con il GazeTracker
    public string nomeScenaMenu = "MainMenu"; // Inserisci il nome esatto della scena del menù

    // Questa funzione va collegata all'evento OnClick() del bottone "Esci"
    public void EsciDallaSimulazione()
    {
        // 1. Salva il report CSV prima di cambiare scena
        if (trackerSguardo != null)
        {
            trackerSguardo.SaveCSVReport();
        }
        else
        {
            Debug.LogWarning("GazeTracker non assegnato in SimulationUI!");
        }

        // 2. Carica la scena del menù principale
        SceneManager.LoadScene(nomeScenaMenu, LoadSceneMode.Single);
    }
}