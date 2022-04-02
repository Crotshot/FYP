using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class _SceneManager : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public string GetSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }

    public void MainMenu() {
        SceneManager.LoadScene("MainMenu");
    }

    public void MainMenuDelay() {
        Invoke("MainMenu", 5f);
    }
}
