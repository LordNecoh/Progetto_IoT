using UnityEngine;
using TMPro;

public class BPMDisplay : MonoBehaviour
{
    [Header("Riferimenti")]
    private UDPReceiver udpReceiver;
    private TextMeshProUGUI testoBPM;

    private void Start()
    {
        udpReceiver = GameObject.Find("GameManager").GetComponent<UDPReceiver>();
        if (udpReceiver == null)
        {
            udpReceiver = GetComponent<UDPReceiver>();
            if (udpReceiver == null)
            {
                Debug.LogError("UDPReceiver non trovato! Assicurati che lo script sia presente sullo stesso GameObject.");
            }
        }
        testoBPM = GetComponentInChildren<TextMeshProUGUI>();
        if (testoBPM == null)
        {
            Debug.LogError("TextMeshProUGUI non assegnato! Trascina il riferimento nel campo 'testoBPM' nell'Inspector.");
        }
    }


    void Update()
    {
        if (udpReceiver != null && testoBPM != null)
        {
            testoBPM.text = $"{udpReceiver.currentHeartRate} BPM";
        }
    }
}