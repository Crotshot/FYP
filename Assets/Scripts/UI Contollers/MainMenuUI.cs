using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    bool showingSettings = false;
    [SerializeField] GameObject mainPanel, settingsPanel;
    [SerializeField] TMP_Text title;
    private _SceneManager sM;

    private void Awake()
    {
        sM = FindObjectOfType<_SceneManager>();
    }

    public void B_Play()
    {
        sM.LoadScene("Lobby");
    }

    public void B_Settings()
    {
        if (!showingSettings)
        {
            mainPanel.SetActive(false);
            settingsPanel.SetActive(true);
            title.text = "Settings";
            showingSettings = true;
        }
        else
        {
            mainPanel.SetActive(true);
            settingsPanel.SetActive(false);
            title.text = "Overlord Arena";
            showingSettings = false;
        }

    }

    public void B_Quit()
    {
        Application.Quit();
    }
}
