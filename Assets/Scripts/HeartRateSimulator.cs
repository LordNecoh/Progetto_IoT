using UnityEngine;
using TMPro;

public class HeartRateSimulator : MonoBehaviour
{
    [Header("Valori BPM")]
    public int currentBPM = 70;
    private int restingBPM = 70;
    private int peakBPM = 100;

    [Header("Dinamiche di Fluttuazione")]
    public bool isStressed = false; // Se true, il battito sale verso peakBPM. Se false, scende verso restingBPM.
    private bool autoCycle = false;   // Alterna automaticamente salita e discesa per testare il sistema
    private float cycleDuration = 25f; // Quanti secondi dura una fase (salita o discesa)

    private float dataTimer = 0f;
    private float cycleTimer = 0f;

    private int sogliaCavallo = 90; // Soglia per far apparire il cavallo

    private GameObject horseObjet;
    private HorseController horse;

    public TextMeshProUGUI bpmText;

    void Start()
    {
        horseObjet = GameObject.Find("Horse");
        if (horseObjet != null)
        {
            horse = horseObjet.GetComponent<HorseController>();
        }
        else
        {
            Debug.LogError("Oggetto Cavallo non trovato nella scena!");
        }
    }

    void Update()
    {
        // 1. Gestione del ciclo automatico (per testare i continui picchi)
        if (autoCycle)
        {
            cycleTimer += Time.deltaTime;
            if (cycleTimer >= cycleDuration)
            {
                isStressed = !isStressed; // Inverte lo stato (da salita a discesa o viceversa)
                cycleTimer = 0f;
            }
        }

        // 2. Simulazione della ricezione dati Bluetooth (1 Hz)
        dataTimer += Time.deltaTime;
        if (dataTimer >= 1f)
        {
            dataTimer = 0f;
            CalculateNextBPM();
        }

        if (currentBPM < sogliaCavallo) // !!! IMPORTANTE: POI DA SPOSTARE NEL GAME MANAGER
        {
            horse.gameObject.SetActive(true);
            horse.StartSequence();
        }

        UpdateBPMUI(currentBPM);    // !!! IMPORTANTE: POI DA SPOSTARE NEL GAME MANAGER
    }

    void CalculateNextBPM()
    {
        int targetBPM = isStressed ? peakBPM : restingBPM;
        int step = 0;

        // Rampa di salita (veloce) o discesa (lenta)
        if (currentBPM < targetBPM)
        {
            step = Random.Range(4, 7); // Aumenta di 4-6 battiti
        }
        else if (currentBPM > targetBPM)
        {
            step = -Random.Range(4, 7); // Scende di 4-6 battiti
        }

        // Micro-fluttuazione naturale (Aritmia respiratoria)
        int noise = Random.Range(-2, 2); // Oscilla di +/- 2

        // Calcolo finale e applicazione limiti
        currentBPM += step + noise;
        currentBPM = Mathf.Clamp(currentBPM, restingBPM - 5, peakBPM + 5);

        Debug.Log($"[BPM]: {currentBPM} | Stato: {(isStressed ? "SFORZO ↗" : "RECUPERO ↘")}");
    }

    public void UpdateBPMUI(int currentBpm) // !!! IMPORTANTE: POI DA SPOSTARE NEL GAME MANAGER
    {
        if (bpmText != null)
            bpmText.text = "BPM: " + currentBpm;
    }

    // Funzioni pubbliche da richiamare in futuro per attivare picchi manualmente
    public void TriggerStress() => isStressed = true;
    public void TriggerRelax() => isStressed = false;
}