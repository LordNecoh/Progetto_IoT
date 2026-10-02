using UnityEngine;

public class SpeedCalculator : MonoBehaviour
{
    [Header("Riferimenti")]
    private UDPReceiver udpReceiver;

    [Header("Impostazioni di Calcolo")]
    private readonly float baseGravity = 9.81f;         // Standard base gravity value in m/s²
    private readonly float movementThreshold = 1.5f;      // Under this threshold, the movement is considered negligible
    private readonly float speedMultiplier = 1.2f; 

    private readonly float smoothing = 2f;               // How quickly the speed adapts (smoothing)

    [Header("Risultati")]
    public float currentSpeed = 0f;           // La velocità calcolata (es. in m/s o km/h)

    private float intensitaSmussata = 0f;

    private void Start()
    {
        udpReceiver = GetComponent<UDPReceiver>();
    }

    void Update()
    {
        if (udpReceiver == null) return;

        float magnitude = udpReceiver.currentAcceleration.magnitude;
        float movementForce = Mathf.Abs(magnitude - baseGravity);

        if (movementForce < movementThreshold)
        {
            movementForce  = 0f;
        }

        intensitaSmussata = Mathf.Lerp(intensitaSmussata, movementForce, Time.deltaTime * smoothing);

        currentSpeed = intensitaSmussata * speedMultiplier;
    }
}