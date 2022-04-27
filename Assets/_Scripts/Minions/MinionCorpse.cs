using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionCorpse : MonoBehaviour {
    float halfTime, decayTime, particleTime;
    Transform body;

    private void Start() {
        body = transform.GetChild(0);

        ParticleSystem pS = GetComponent<ParticleSystem>();
        decayTime = pS.main.duration;
        particleTime = pS.main.startLifetime.constantMax;

        halfTime = decayTime / 2;
    }

    private void Update() {
        if(decayTime > 0) {
            decayTime -= Time.deltaTime;
            if(decayTime <= halfTime) {
                body.transform.localScale = Vector3.one * decayTime / halfTime;
            }
        }
        else if(particleTime > 0) {
            particleTime -= Time.deltaTime;

            if(particleTime <= 0) {
                Destroy(gameObject);
            }
        }
    }
}