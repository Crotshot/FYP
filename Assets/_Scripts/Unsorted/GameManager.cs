using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    _SceneManager sceneManager;
    [SerializeField] GameObject[] conquerorPrefabs;
    [SerializeField] GameObject[] minionSpawnerPrefabs;

    private void Awake() {
        sceneManager = GetComponent<_SceneManager>();
    }

    private void Start() {
        sceneManager.LoadScene("MainMenu");
    }

    public GameObject GetConqueror(string conqName) {
        foreach (GameObject obj in conquerorPrefabs) {
            if (obj.name.Contains(conqName) || obj.name.Equals(conqName)) {
                return obj;
            }
        }
        return null;
    }

    public GameObject GetMinionSpawner(string name) {
        foreach (GameObject obj in minionSpawnerPrefabs) {
            if (obj.name.Contains(name)) {
                return obj;
            }
        }
        return null;
    }
}