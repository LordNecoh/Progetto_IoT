using System.Collections;
using UnityEngine;

public class HorseController : MonoBehaviour
{
    [Header("Condizioni Battito")]
    private UDPReceiver udpReceiver;
    private readonly int sogliaBattito = 100;

    [Header("Impostazioni Cavallo")]
    public Animator anim;
    public float walkSpeed = 2f;
    private bool isRunning = false;

    private void Start()
    {
        udpReceiver = GameObject.Find("GameManager").GetComponent<UDPReceiver>();
        StartSequence();
    }

    public void StartSequence()
    {
        if (!isRunning) StartCoroutine(Routine());
    }

    private IEnumerator Routine()
    {
        isRunning = true;

        // Loop infinito
        while (true)
        {
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

            // 3. Si gira di 180 gradi (al prossimo giro camminerà per tornare indietro)
            transform.Rotate(0, 180, 0);
        }
    }

    private IEnumerator WaitIfStressed()
    {
        if (udpReceiver != null && udpReceiver.currentHeartRate > sogliaBattito)
        {
            anim.speed = 0;

            yield return new WaitUntil(() => udpReceiver.currentHeartRate <= sogliaBattito);

            anim.speed = 1;
        }
    }
}