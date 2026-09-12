using UnityEngine;

public class CompanionGuide : MonoBehaviour
{
    [Header("Impostazioni")]
    private int safeThresholdBpm = 80; // Sotto questo valore, il compagno appare
    private float floatSpeed = 2f;
    private float floatAmplitude = 0.15f;
    private float fadeSpeed = 3f;
    private float maxScale = 1f; // Dimensione finale dell'oggetto

    private Vector3 startLocalPos;
    private Vector3 targetScale;

    public GameObject heartRateSimulator; // Riferimento allo script HeartRateSimulator

    void Start()
    {
        startLocalPos = transform.localPosition;
        targetScale = transform.localScale;
        transform.localScale = Vector3.zero;

    }

    void Update()
    {
        // Movimento di galleggiamento sull'asse Y locale
        float newY = startLocalPos.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.localPosition = new Vector3(startLocalPos.x, newY, startLocalPos.z);

        // Apparizione e scomparsa fluida
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * fadeSpeed);
        UpdateHeartRate(heartRateSimulator.GetComponent<HeartRateSimulator>().currentBPM);
    }

    public void UpdateHeartRate(int currentBpm)
    {
        if (currentBpm < safeThresholdBpm)
            targetScale = Vector3.one * maxScale;
        else
            targetScale = Vector3.zero;
    }
}