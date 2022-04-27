using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Projectile : NetworkBehaviour {
    [SerializeField] protected float projDamage, projSpeed, projRadius, projDecayTime;
    [SerializeField] protected List<Transform> projectileBodies;
    [SerializeField] protected LayerMask layers, defLayer;//Unit & Default
    int projCount, projIndex = 0;
    protected List<Proj> projectiles;

    public virtual void Start() {
        projectiles = new List<Proj>();

        foreach (Transform p in projectileBodies) {
            Proj proj = new Proj(p);
            p.parent = null;
            projectiles.Add(proj);
            proj.Impact();
        }

        projCount = projectiles.Count;
    }

    protected virtual void Update() {
        foreach (Proj p in projectiles) {
            p.projBody.position += p.projBody.forward * projSpeed * Time.deltaTime;
        }
        CollisionCheck();
    }

    [ClientRpc]
    public virtual void RpcFire(Vector3 pos, Vector3 target) {
        if (projIndex == projCount)
            projIndex = 0;
        projectiles[projIndex].ShootProj(projDecayTime);
        projectiles[projIndex].start = pos;
        projectiles[projIndex].end = target;
        projIndex++;
    }

    protected virtual void CollisionCheck() {
        if (!isServer)
            return;
        foreach (Proj p in projectiles) {
            if (p.Update(Time.deltaTime) == Proj.ProjState.Active) {
                Collider[] hits = Physics.OverlapSphere(p.projBody.position, projRadius, layers, QueryTriggerInteraction.Ignore);
                foreach (Collider hit in hits) {
                    if (hit.tag.Equals("minion") || hit.tag.Equals("Player")) {
                        if (hit.GetComponent<Team>().GetTeam() != GetComponent<Team>().GetTeam()) {
                            hit.GetComponent<Health>().Damage(projDamage);
                            RpcProjImpact(projectiles.IndexOf(p));
                            return;
                        }
                    }
                    else if (hit.gameObject.layer == defLayer) {
                        RpcProjImpact(projectiles.IndexOf(p));
                        return;
                    }
                }
            }
        }
    }

    [ClientRpc]
    void RpcProjImpact(int i) {
        projectiles[i].Impact();
    }

    public List<Proj> GetProjectiles() {
        return projectiles;
    }

    private void OnDestroy() {
        foreach (Proj p in projectiles) {
            if (p.projBody != null) {
                Destroy(p.projBody.gameObject);
            }
        }
    }
}

public class Proj {
    public Proj(Transform body) {
        projBody = body;
    }
    public Vector3 start, end;
    public Transform projBody;
    public enum ProjState { Active, Inactive }
    public ProjState state = ProjState.Inactive;
    public float decayTimer, maxTime;//Max time is used when calculating arc of projectile later

    public void ShootProj(float decayTime) {
        decayTimer = decayTime;
        maxTime = 0;
        state = ProjState.Active;
    }

    public void Impact() {
        state = ProjState.Inactive;
        projBody.position = Vector3.down * 60f;
    }

    virtual public ProjState Update(float deltaTime) {
        if (state == ProjState.Active) {
            decayTimer -= deltaTime;
            if (decayTimer <= 0) {
                Impact();
            }
        }
        return state;
    }
}
