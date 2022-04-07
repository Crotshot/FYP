using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasCloud : NetworkBehaviour
{
    [SerializeField] float duration, poisonDamage, scalePerSec, slowStrength;
    [SerializeField] int poisonTicks, slownessTicks;
    [SerializeField] ParticleSystem shockEmitter, poisonEmitter;
    [SerializeField] Transform gasEm;
    float tickInterval, ticktimer, durationTimer;

    List<Status> trackedStatus = new List<Status>();
    ParticleSystem.EmissionModule poisonEmission;
    SphereCollider coll;
    public enum GasCloudState {Active, Inactive}
    GasCloudState gcs = GasCloudState.Inactive;

    private void Awake() {
        transform.parent = null;
        transform.position = Vector3.down * 50f;
        poisonEmission = poisonEmitter.emission;
    }

    private void Start() {
        if (isServer) {
            coll = GetComponent<SphereCollider>();
            tickInterval = 1 / (float) FindObjectOfType<StatusEffectManager>().GetTickRate();
        }
    }

    private void FixedUpdate() {
        if (gcs == GasCloudState.Inactive)
            return;

        gasEm.localScale += Vector3.one * scalePerSec * Time.deltaTime;
        transform.localScale += Vector3.one * scalePerSec * Time.deltaTime;

        #region Server
        if (isServer) {
            if (ticktimer > 0) {
                ticktimer -= Time.deltaTime;
            }
            else {
                for (int i = trackedStatus.Count - 1; i > -1; i--) {
                    if (trackedStatus[i] == null) {
                        trackedStatus.Remove(trackedStatus[i]);
                        continue;
                    }
                    trackedStatus[i].AddEffect(Status.StatusEffect.Poison, poisonTicks, poisonDamage);
                    trackedStatus[i].AddEffect(Status.StatusEffect.Slow, slownessTicks, slowStrength);
                }
                ticktimer += tickInterval;
            }
            
        }
        #endregion
        durationTimer -= Time.deltaTime;
        if (durationTimer <= 0) {
            DeActivate();
        }
    }

    public void Activate(Vector3 pos) {
        transform.position = pos;
        gcs = GasCloudState.Active;
        poisonEmission.enabled = true;
        if (isServer) {
            Debug.Log("Collider on");
            coll.enabled = true;
            tickInterval = ticktimer;
        }
        durationTimer = duration;
    }

    public void DeActivate() {
        transform.position = Vector3.down * 50f;
        gcs = GasCloudState.Inactive;
        poisonEmission.enabled = false;
        if (isServer) {
            coll.enabled = false;
            trackedStatus.Clear();
        }
        gasEm.localScale = Vector3.one * 0.7f;
        transform.localScale = Vector3.one;
    }

    #region Triggers
    private void OnTriggerEnter(Collider other) {//Triggers only enabled on server and when cloud is active
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
    #endregion

    #region Taser Combo
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
    #endregion
}