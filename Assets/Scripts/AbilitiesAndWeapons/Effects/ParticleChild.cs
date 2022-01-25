using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ParticleChild : NetworkBehaviour
{
    private void Start() {
        ParticleSystem pS = GetComponent<ParticleSystem>();
        var time = pS.main.duration;
        if(isServer)
            Invoke(nameof(Del), time + 0.05f);
        pS.Play();
    }

    private void Del() {
        NetworkServer.Destroy(gameObject);
    }
}