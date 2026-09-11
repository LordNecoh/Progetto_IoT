using UnityEngine;

public class BreathingGuide : MonoBehaviour
{
    [Header("Soglie Battito")]
    public int thresholdBpm = 90; // La guida si attiva sopra questo valore

    [Header("Parametri Respirazione")]
    public float breathCycleDuration = 8f;
    public float minScale = 0.2f;
    public float maxScale = 0.6f;
    public float fadeSpeed = 2f;

    private Vector3 targetScale;
    private bool isActive = false;
    private Renderer objRenderer;

    public GameObject heartRateSimulator; // Riferimento allo script HeartRateSimulator

    void Start()
    {
        objRenderer = GetComponent<Renderer>();
        transform.localScale = Vector3.zero;
    }
    public void UpdateHeartRate(int currentBpm)
    {
        isActive = (currentBpm >= thresholdBpm);
    }

    void Update()
    {
        UpdateHeartRate(heartRateSimulator.GetComponent<HeartRateSimulator>().currentBPM);
        if (isActive)
        {
            // Calcola l'oscillazione sinusoidale da 0 a 1 nel tempo del ciclo
            float t = (Mathf.Sin(Time.time * (2f * Mathf.PI / breathCycleDuration) - (Mathf.PI / 2f)) + 1f) / 2f;
            float currentScaleValue = Mathf.Lerp(minScale, maxScale, t);
            targetScale = Vector3.one * currentScaleValue;
        }
        else
        {
            // Se inattivo, rimpicciolisce fino a sparire
            targetScale = Vector3.zero;
        }

        // Transizione fluida della dimensione
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * fadeSpeed);
    }

    // Funzione chiamata dallo script che riceve i BPM
    
}