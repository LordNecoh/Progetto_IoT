using UnityEngine;

public class HeartRateSimulator : MonoBehaviour
{
    [Header("Valori BPM")]
    public int currentBPM = 70;
    public int restingBPM = 70;
    public int peakBPM = 120;

    [Header("Dinamiche di Fluttuazione")]
    public bool isStressed = false; // Se true, il battito sale verso peakBPM. Se false, scende verso restingBPM.
    [SerializeField] private bool autoCycle = true;   // Alterna automaticamente salita e discesa per testare il sistema
    [SerializeField] private float cycleDuration = 15f; // Quanti secondi dura una fase (salita o discesa)

    private float dataTimer = 0f;
    private float cycleTimer = 0f;

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
    }

    void CalculateNextBPM()
    {
        int targetBPM = isStressed ? peakBPM : restingBPM;
        int step = 0;

        // Rampa di salita (veloce) o discesa (lenta)
        if (currentBPM < targetBPM)
        {
            step = Random.Range(3, 6); // Aumenta di 3-5 battiti
        }
        else if (currentBPM > targetBPM)
        {
            step = -Random.Range(1, 4); // Scende di 1-3 battiti
        }

        // Micro-fluttuazione naturale (Aritmia respiratoria)
        int noise = Random.Range(-2, 3); // Oscilla di +/- 2

        // Calcolo finale e applicazione limiti
        currentBPM += step + noise;
        currentBPM = Mathf.Clamp(currentBPM, restingBPM - 5, peakBPM + 5);

        Debug.Log($"[BPM]: {currentBPM} | Stato: {(isStressed ? "SFORZO ↗" : "RECUPERO ↘")}");
    }

    // Funzioni pubbliche da richiamare in futuro per attivare picchi manualmente
    public void TriggerStress() => isStressed = true;
    public void TriggerRelax() => isStressed = false;
}