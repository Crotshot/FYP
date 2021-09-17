using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    _SceneManager sceneManager;

    private void Awake()
    {
        sceneManager = GetComponent<_SceneManager>();
    }

    private void Start()
    {
        sceneManager.LoadScene("MainMenu");
    }
}
