using UnityEngine;

public class CompanionGuide : MonoBehaviour
{
    [Header("Impostazioni")]
    [SerializeField] private int safeThresholdBpm = 85; // Sotto questo valore, il compagno appare
    [SerializeField] private float floatSpeed = 2f;
    [SerializeField] private float floatAmplitude = 0.1f;
    [SerializeField] private float fadeSpeed = 3f;
    [SerializeField] private float maxScale = 0.3f; // Dimensione finale dell'oggetto

    private Vector3 startLocalPos;
    private Vector3 targetScale;

    void Start()
    {
        startLocalPos = transform.localPosition;
        transform.localScale = Vector3.zero;
        targetScale = transform.localScale;
    }

    void Update()
    {
        // Movimento di galleggiamento sull'asse Y locale
        float newY = startLocalPos.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.localPosition = new Vector3(startLocalPos.x, newY, startLocalPos.z);

        // Apparizione e scomparsa fluida
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * fadeSpeed);
    }

    public void UpdateHeartRate(int currentBpm)
    {
        if (currentBpm < safeThresholdBpm)
            targetScale = Vector3.one * maxScale;
        else
            targetScale = Vector3.zero;
    }
}