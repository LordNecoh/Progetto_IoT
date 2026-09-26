using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{
    private SpeedCalculator speedCalculator;
    private float terrainLength = 173f;

    private void Start()
    {
        speedCalculator = GameObject.Find("GameManager").GetComponent<SpeedCalculator>();
        if(speedCalculator == null)
        {
            Debug.LogError("SpeedCalculator non trovato! Assicurati che l'oggetto 'GameManager' abbia il componente SpeedCalculator.");
        }
    }
    void Update()
    {
        float currentSpeed = speedCalculator != null ? speedCalculator.currentSpeed : 0f;

        transform.Translate(Vector3.back * currentSpeed * Time.deltaTime);

        if (transform.position.z <= -terrainLength)
        {
            transform.position += new Vector3(0, 0, terrainLength * 2);
        }
    }
}