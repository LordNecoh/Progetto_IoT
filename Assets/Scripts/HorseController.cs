using System.Collections;
using UnityEngine;

public class HorseController : MonoBehaviour
{
    public Animator anim;
    public float walkSpeed = 2f;
    private bool isRunning = false;

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
            transform.Translate(Vector3.forward * walkSpeed * Time.deltaTime);
            timer -= Time.deltaTime;
            yield return null;
        }

        // 2. Mangia l'erba
        anim.SetTrigger("doEat");
        yield return new WaitForSeconds(5f); // Aspetta 5 secondi

        // 3. Si gira e se ne va
        anim.SetTrigger("doWalk");
        transform.Rotate(0, 180, 0);
        timer = 3f;
        while (timer > 0)
        {
            transform.Translate(Vector3.forward * walkSpeed * Time.deltaTime);
            timer -= Time.deltaTime;
            yield return null;
        }

        // Scompare alla fine
        gameObject.SetActive(false);
        isRunning = false;
    }
}