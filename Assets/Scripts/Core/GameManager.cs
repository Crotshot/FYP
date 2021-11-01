using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    _SceneManager sceneManager;
    [SerializeField] GameObject[] conquerorPrefabs;
    [SerializeField] GameObject[] minionPrefabs;

    private void Awake()
    {
        sceneManager = GetComponent<_SceneManager>();
    }

    private void Start()
    {
        sceneManager.LoadScene("MainMenu");
    }

    public GameObject GetConqueror(string conqName) { // Can pass 10_ for an id or Bert for a name
        foreach (GameObject obj in conquerorPrefabs) {
            if (obj.name.Contains(conqName) || obj.name.Equals(conqName)) { //Equals might no tbe necessary
                return obj;
            }
        }
        return null;
    }

    public GameObject GetMinion(string minionName) { // Can pass 10_ for an id or Bert for a name
        foreach (GameObject obj in minionPrefabs) {
            if (obj.name.Contains(minionName)) {
                return obj;
            }
        }
        return null;
    }
}
