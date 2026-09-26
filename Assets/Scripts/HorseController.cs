using System.Collections;
using UnityEngine;

public class HorseController : MonoBehaviour
{
    [Header("Condizioni Battito")]
    private UDPReceiver udpReceiver; 
    private readonly int sogliaBattito = 80; 

    [Header("Impostazioni Cavallo")]
    public Animator anim;
    public float walkSpeed = 2f;
    private bool isRunning = false;

    private void Start()
    {
        udpReceiver = GameObject.Find("GameManager").GetComponent<UDPReceiver>();
    }

    public void StartSequence()
    {
        if (!isRunning) StartCoroutine(Routine());
    }

    private IEnumerator Routine()
    {
        isRunning = true;

        // 1. Cammina in avanti
        anim.SetTrigger("doWalk");
        float timer = 2f;
        while (timer > 0)
        {
            yield return StartCoroutine(WaitIfStressed()); 

            transform.Translate(Vector3.forward * walkSpeed * Time.deltaTime);
            timer -= Time.deltaTime;
            yield return null;
        }

        // 2. Mangia l'erba
        anim.SetTrigger("doEat");
        float eatTimer = 5f;
        while (eatTimer > 0)
        {
            yield return StartCoroutine(WaitIfStressed());

            eatTimer -= Time.deltaTime;
            yield return null;
        }

        // 3. Si gira e se ne va
        anim.SetTrigger("doWalk");
        transform.Rotate(0, 180, 0);
        timer = 3f;
        while (timer > 0)
        {
            yield return StartCoroutine(WaitIfStressed());

            transform.Translate(Vector3.forward * walkSpeed * Time.deltaTime);
            timer -= Time.deltaTime;
            yield return null;
        }

        gameObject.SetActive(false);
        isRunning = false;
    }

    // Ferma il tempo e l'animazione se il battito è troppo alto
    private IEnumerator WaitIfStressed()
    {
        // Se il battito supera la soglia (e ignoriamo lo 0 di quando il sensore è disconnesso)
        if (udpReceiver != null && udpReceiver.currentHeartRate > sogliaBattito && udpReceiver.currentHeartRate > 0)
        {
            anim.speed = 0; // Mette in pausa l'animazione del cavallo

            // Aspetta finché il battito non scende di nuovo
            yield return new WaitUntil(() => udpReceiver.currentHeartRate <= sogliaBattito);

            anim.speed = 1; // Fa ripartire l'animazione
        }
    }
}