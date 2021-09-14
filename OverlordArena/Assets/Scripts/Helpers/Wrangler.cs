using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Wrangler : MonoBehaviour
{
    private void Awake()
    {
        if (!FindObjectOfType<GameManager>())
        {
            Debug.LogWarning("Test Started in wrong scene, wrangled back to _preload");
            SceneManager.LoadScene("_preload");
        }
        else{
            Destroy(gameObject);
        }
    }
}