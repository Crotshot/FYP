using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarTarget : MonoBehaviour
{
    Transform cam;

    private void Start() {
        Invoke("Setup", 5f);
    }

    void Setup(){
        foreach (PlayerController pC in FindObjectsOfType<PlayerController>()) {
            if (pC.AuthCheck()) {
                cam = pC.transform.GetChild(0).GetChild(0).GetChild(0);
                break;
            }
        }
    }

    public Transform GetCamera() {
        return cam;
    }
}
