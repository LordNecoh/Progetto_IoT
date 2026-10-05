using UnityEngine;

public class GazeTracker : MonoBehaviour
{
    public Transform xrOriginRoot;

    [Header("Dati in tempo reale")]
    public float currentVisualAngle;
    public string currentDirection = "[FORWARD]";
    public string currentLookedObject = "None";

    void Update()
    {
        float signedAngle = Vector3.SignedAngle(xrOriginRoot.forward, transform.forward, Vector3.up);

        currentVisualAngle = Mathf.Abs(signedAngle);

        // Calcolo della direzione
        if (currentVisualAngle > 110f)
            currentDirection = "[BACK]";
        else if (signedAngle > 45f)
            currentDirection = "[RIGHT]";
        else if (signedAngle < -45f)
            currentDirection = "[LEFT]";
        else
            currentDirection = "[FORWARD]";

        Ray ray = new Ray(transform.position, transform.forward);
        string lookedObject = "None";

        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            lookedObject = hit.collider.gameObject.name;
        }

        currentLookedObject = ObjectNameSimplifier(lookedObject);
    }

    private string ObjectNameSimplifier(string rawName)
    {
        if (rawName == "Nessuno") return rawName;

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