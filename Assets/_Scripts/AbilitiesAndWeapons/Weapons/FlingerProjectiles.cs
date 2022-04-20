using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class FlingerProjectiles : ArchingProjectile
{
    [SerializeField] int projFired = 5;
    [SerializeField] float inaccuracy;

    [SerializeField] ParticleSystem expEmitter;
    ParticleSystem.EmitParams exp;

    public override void Start() {
        base.Start();
        exp = new ParticleSystem.EmitParams {
            applyShapeToPosition = true
        };
    }

    [Client]
    public override void RpcFire(Vector3 pos, Vector3 target) {
        for(int i = projFired; i > -1; i--) {
            base.RpcFire(pos, target + new Vector3(Random.Range(-inaccuracy, inaccuracy),0, Random.Range(-inaccuracy, inaccuracy)));
        }
    }
}
