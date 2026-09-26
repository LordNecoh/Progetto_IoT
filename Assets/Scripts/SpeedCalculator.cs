using UnityEngine;

public class SpeedCalculator : MonoBehaviour
{
    [Header("Riferimenti")]
    private UDPReceiver udpReceiver;

    [Header("Impostazioni di Calcolo")]
    private readonly float gravitaBase = 9.81f;         // Accelerazione di gravità standard
    private readonly float sogliaMovimento = 1.5f;      // Sotto questo valore l'utente è considerato fermo
    private readonly float moltiplicatoreVelocita = 1.2f; // Regola questo valore per tarare la velocità virtuale

    private readonly float fluidita = 2f;               // Quanto velocemente la velocità si adatta (smussamento)

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

        float magnitudo = udpReceiver.currentAcceleration.magnitude;
        float forzaMovimento = Mathf.Abs(magnitudo - gravitaBase);

        if (forzaMovimento < sogliaMovimento)
        {
            forzaMovimento = 0f;
        }

        intensitaSmussata = Mathf.Lerp(intensitaSmussata, forzaMovimento, Time.deltaTime * fluidita);

        currentSpeed = intensitaSmussata * moltiplicatoreVelocita;
    }
}