using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasCloud : NetworkBehaviour
{
    [SerializeField] float duration, poisonDamage, scalePerSec, slowStrength;
    [SerializeField] int poisonTicks, slownessTicks;
    [SerializeField] ParticleSystem shockEmitter;
    [SerializeField] Transform gasEm;
    float tickInterval = 0.25f, ticktimer = 0.25f;

    List<Status> trackedStatus = new List<Status>();

    private void FixedUpdate() {
        gasEm.localScale += Vector3.one * scalePerSec * Time.deltaTime;

        if (!isServer)
            return;

        if (ticktimer > 0) {
            ticktimer -= Time.deltaTime;
        }
        else {
            for(int i = trackedStatus.Count - 1; i > -1; i--) {
                if (trackedStatus[i] == null) {
                    trackedStatus.Remove(trackedStatus[i]);
                    continue;
                }
                trackedStatus[i].AddEffect(Status.StatusEffect.Poison, poisonTicks, poisonDamage);
                trackedStatus[i].AddEffect(Status.StatusEffect.Slow, slownessTicks, slowStrength);
            }
            ticktimer = tickInterval;
        }

        transform.localScale += Vector3.one * scalePerSec * Time.deltaTime;
        duration -= Time.deltaTime;

        if(duration <= 0) {
            NetworkServer.Destroy(gameObject);
            Destroy(gameObject);
        }
    }

    //Player, minion
    private void OnTriggerEnter(Collider other) {
        if (other.tag.Equals("minion") || other.tag.Equals("Player")) {
            if (other.GetComponent<Team>().GetTeam() != GetComponent<Team>().GetTeam()) {
                trackedStatus.Add(other.GetComponent<Status>());
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.tag.Equals("minion") || other.tag.Equals("Player")) {
            if (other.TryGetComponent(out Status s)) {
                if (trackedStatus.Contains(s)) {
                    trackedStatus.Remove(s);
                }
            }
        }
    }

    public void TaserStun(int ticks) {
        if (isServer) {
            for (int i = trackedStatus.Count - 1; i > -1; i--) {
                if (trackedStatus[i] == null) {
                    trackedStatus.Remove(trackedStatus[i]);
                    continue;
                }
                trackedStatus[i].AddEffect(Status.StatusEffect.Stun, ticks, 0);
            }
            RpcEffect();
        }
        else {
            CmdTaserStun(ticks);
        }
    }

    [ClientRpc]
    private void RpcEffect() {
        Debug.Log("Shocking Combo");
        var es = shockEmitter.shape;
        es.radius = transform.localScale.x;
        shockEmitter.Play();
    }

    [Command (requiresAuthority = false)]
    private void CmdTaserStun(int ticks) {
        TaserStun(ticks);
    }
}
