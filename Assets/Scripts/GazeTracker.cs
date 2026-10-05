using UnityEngine;

public class GazeTracker : MonoBehaviour
{
    public Transform xrOriginRoot;

    [Header("Dati in tempo reale")]
    public float currentVisualAngle;
    public string currentDirection = "[FORWARD]";
    public string currentLookedObject = "None";

    [Header("Ottimizzazione")]
    public float raycastInterval = 0.1f; // 0.1s = 10 Raycast al secondo
    private float timer = 0f;
    private string lastRawObjectName = "None";

    void Update()
    {
        // 1. Calcolo dell'angolo (leggero, lo lasciamo a ogni frame per reattività visiva)
        float signedAngle = Vector3.SignedAngle(xrOriginRoot.forward, transform.forward, Vector3.up);
        currentVisualAngle = Mathf.Abs(signedAngle);

        if (currentVisualAngle > 110f) currentDirection = "[BACK]";
        else if (signedAngle > 45f) currentDirection = "[DESTRA]";
        else if (signedAngle < -45f) currentDirection = "[SINISTRA]";
        else currentDirection = "[FORWARD]";

        // 2. Timer per Raycast e manipolazione stringhe
        timer += Time.deltaTime;
        if (timer >= raycastInterval)
        {
            timer = 0f;
            PerformRaycast();
        }
    }

    private void PerformRaycast()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        string currentHitName = "None";

        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            currentHitName = hit.collider.gameObject.name;
        }

        // 3. Elabora la stringa SOLO se stiamo guardando un oggetto diverso da prima
        if (currentHitName != lastRawObjectName)
        {
            lastRawObjectName = currentHitName;
            currentLookedObject = ObjectNameSimplifier(currentHitName);
        }
    }

    private string ObjectNameSimplifier(string rawName)
    {
        if (rawName == "None" || rawName == "Nessuno") return "None";

        string nameLower = rawName.ToLower();

        if (nameLower.Contains("tree")) return "albero";
        if (nameLower.Contains("floor") || nameLower.Contains("ground") || nameLower.Contains("meadow") || nameLower.Contains("grass")) return "terreno";
        if (nameLower.Contains("bridge")) return "ponte";
        if (nameLower.Contains("rock") || nameLower.Contains("pebble")) return "roccia";
        if (nameLower.Contains("mushroom")) return "fungo";
        if (nameLower.Contains("fence")) return "staccionata";
        if (nameLower.Contains("daffodil") || nameLower.Contains("hyacinth") || nameLower.Contains("sunflower")) return "fiore";
        if (nameLower.Contains("mountain")) return "montagna";

        return rawName.Replace("(Clone)", "").Trim();
    }
}