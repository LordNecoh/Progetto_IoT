using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{
    private SpeedCalculator speedCalculator;
    private readonly float terrainLength = 173f;
    private readonly float speedMultiplier = 1.4f; //Per regolare la velocità del terreno rispetto alla velocità del giocatore

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

        transform.Translate(Vector3.back * currentSpeed * Time.deltaTime * speedMultiplier);

        if (transform.position.z <= -terrainLength)
        {
            transform.position += new Vector3(0, 0, terrainLength * 2);
        }
    }
}