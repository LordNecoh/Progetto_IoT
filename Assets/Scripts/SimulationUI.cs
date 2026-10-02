using UnityEngine;
using UnityEngine.SceneManagement;

public class SimulationUI : MonoBehaviour
{
    [Header("Riferimenti")]
    private DataSaver dataSaver;
    public string nomeScenaMenu = "MainMenu";
    
    private void Start()
    {
        dataSaver = GameObject.Find("GameManager").GetComponent<DataSaver>();
        if (dataSaver == null)
        {
            Debug.LogError("SimulationUI: DataSaver non trovato nel GameManager!");
        }
    }

    public void EsciDallaSimulazione()
    {
        if (dataSaver != null)
        {
            dataSaver.SaveCSVReport();
        }
        else
        {
            Debug.LogWarning("GazeTracker non assegnato in SimulationUI!");
        }

        SceneManager.LoadScene(nomeScenaMenu, LoadSceneMode.Single);
    }
}