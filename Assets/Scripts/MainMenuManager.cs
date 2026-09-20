using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

public class MainMenuManager : MonoBehaviour
{
    private string simSceneName = "SimulationScene";

    public void IniziaSimulazione()
    {
        SceneManager.LoadScene(simSceneName, LoadSceneMode.Single);
    }

    public void Esci()
    {
        Application.Quit();

        // Debug only: Stop play mode in the Unity Editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}