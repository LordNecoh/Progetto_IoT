using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

public class MainMenuManager : MonoBehaviour
{
    private readonly string SimSceneName = "SimulationScene";

    public void IniziaSimulazione()
    {
        SceneManager.LoadScene(SimSceneName, LoadSceneMode.Single);
    }

    public void Esci()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}