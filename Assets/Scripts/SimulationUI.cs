using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

public class SimulationUI : MonoBehaviour
{
    private string MenuSceneName = "MainMenu";

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(MenuSceneName, LoadSceneMode.Single);
    }
}