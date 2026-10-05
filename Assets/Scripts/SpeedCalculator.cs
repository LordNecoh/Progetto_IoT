using UnityEngine;

public class SpeedCalculator : MonoBehaviour
{
    [Header("Riferimenti")]
    private UDPReceiver udpReceiver;

    [Header("Taratura Camminata")]
    private float baseGravity = 9.81f;
    private float stepThreshold = 1.2f;    // Forza netta per far scattare un passo (prova tra 1.0 e 2.0)
    private float stepCooldown = 0.55f;    // Tempo minimo tra due passi (evita doppi conteggi)
    private float strideLength = 0.7f;     // Lunghezza media di un passo (0.7 metri)

    [Header("Statistiche Reali")]
    public int stepCount = 0;             // Passi totali
    public float totalDistance = 0f;      // Metri percorsi
    public float currentSpeed = 0f;       // Velocità attuale (m/s)

    private float lastStepTime = 0f;
    private float targetSpeed = 0f;

    private void Start()
    {
        udpReceiver = GetComponent<UDPReceiver>();
    }

    void Update()
    {
        if (udpReceiver == null) return;

        float magnitude = udpReceiver.currentAcceleration.magnitude;
        float movementForce = Mathf.Abs(magnitude - baseGravity);

        // 1. RILEVAMENTO DEL PASSO (Peak Detection)
        // Se l'accelerazione supera la soglia e è passato abbastanza tempo dall'ultimo passo...
        if (movementForce > stepThreshold && Time.time - lastStepTime > stepCooldown)
        {
            float timeSinceLastStep = Time.time - lastStepTime;
            lastStepTime = Time.time;
            
            // Aggiorniamo i contatori
            stepCount++;
            totalDistance += strideLength;

            // La velocità è semplicemente lo Spazio diviso il Tempo (m/s)
            if (timeSinceLastStep > 0)
            {
                targetSpeed = strideLength / timeSinceLastStep;
                
                // Filtro di sicurezza: se la velocità calcolata è assurda (es. oltre 4 m/s), la cappiamo
                if (targetSpeed > 4f) targetSpeed = 4f; 
            }
        }

        // 2. GESTIONE DA FERMO
        // Se non facciamo passi per più di 1.2 secondi, stiamo rallentando o ci siamo fermati
        if (Time.time - lastStepTime > 1.2f)
        {
            targetSpeed = 0f;
        }

        // 3. FLUIDITÀ VISIVA
        // Smussiamo la velocità in modo che non vada a scatti ma salga e scenda dolcemente
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * 2.5f);
    }
}