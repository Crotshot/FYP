using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleChild : MonoBehaviour {
    private void Start() {
        ParticleSystem pS = GetComponent<ParticleSystem>();
        var time = pS.main.duration;
        Destroy(gameObject, time);
        pS.Play();
    }
}