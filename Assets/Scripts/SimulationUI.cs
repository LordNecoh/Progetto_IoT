using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SimulationUI : MonoBehaviour
{
    private string MenuSceneName = "MainMenu";

    public void BackToMainMenu()
    {
        Console.WriteLine("Tornando al menu principale...");
        SceneManager.LoadScene(MenuSceneName, LoadSceneMode.Single);
    }
}