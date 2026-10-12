using UnityEngine;

public class SpeedCalculator : MonoBehaviour
{
    [Header("Riferimenti")]
    private UDPReceiver udpReceiver;

    [Header("Taratura Camminata (Fluida e Controllata)")]
    public float baseGravity = 9.81f;
    public float walkThreshold = 1.5f;    
    public float walkCooldown = 0.55f;    
    public float walkStride = 0.7f;       
    public float maxWalkSpeed = 1.5f;     // Blocco anti-sgasata per la camminata

    [Header("Taratura Corsa (Super Veloce e Scattante)")]
    public float runThreshold = 2.2f;     // Leggermente più sensibile per attivare la corsa al volo
    public float runCooldown = 0.23f;     // Permette passi ancora più serrati
    public float runStride = 1.6f;        // Falcata allungata a 1.6 metri per volare via
    public float maxRunSpeed = 9.5f;      // Tetto massimo innalzato a 9.5 m/s

    [Header("Statistiche Reali")]
    public int stepCount = 0;             
    public float totalDistance = 0f;      
    public float currentSpeed = 0f;       

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

        bool isRunning = movementForce > runThreshold;
        
        float activeThreshold = isRunning ? runThreshold : walkThreshold;
        float activeCooldown = isRunning ? runCooldown : walkCooldown;
        float activeStride = isRunning ? runStride : walkStride;

        // 1. RILEVAMENTO DEL PASSO
        if (movementForce > activeThreshold && Time.time - lastStepTime > activeCooldown)
        {
            float timeSinceLastStep = Time.time - lastStepTime;
            lastStepTime = Time.time;
            
            stepCount++;
            totalDistance += activeStride;

            if (timeSinceLastStep > 0)
            {
                targetSpeed = activeStride / timeSinceLastStep;
                
                // Limiti di velocità separati
                float speedLimit = isRunning ? maxRunSpeed : maxWalkSpeed;
                if (targetSpeed > speedLimit) targetSpeed = speedLimit; 
            }
        }

        // 2. GESTIONE DA FERMO
        if (Time.time - lastStepTime > 0.8f)
        {
            targetSpeed = 0f;
        }

        // 3. FLUIDITÀ E REATTIVITÀ
        float accelerationSmooth = isRunning ? 6.0f : 2.5f;
        float decelerationSmooth = 4.5f; 
        
        float currentLerp = (targetSpeed == 0f) ? decelerationSmooth : accelerationSmooth;
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * currentLerp);
    }
}