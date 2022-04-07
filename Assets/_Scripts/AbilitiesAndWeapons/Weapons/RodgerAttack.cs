using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

public class RodgerAttack : PlayerAttack {
    [SerializeField] float roundsPerMinute = 60f, secondShotDelay = 0.02f, bulletSpeed = 65f, bulletRadius = 0.25f, bulletDamage = 35f, bulletDecayTime = 10f;
    [SerializeField] int muzzleParticles, impactParticles;
    [SerializeField] Transform launchPoint1, launchPoint2;
    [SerializeField] List<Transform> bulletBodies;
    [SerializeField] ParticleSystem muzzleFlash, impactSparks, shellEmitter;
    ParticleSystem.EmitParams mFlash, iSparks, eShell;
    [SerializeField] LayerMask layers, defLayer;//Unit & Default

    int bulletCount, bulletIndex = 0;
    List<Bullet> bullets;
    float fireRatetimer;

    override protected void Start() {
        base.Start();

        bullets = new List<Bullet>();

        impactSparks.transform.parent = null;
        iSparks = new ParticleSystem.EmitParams {
            applyShapeToPosition = true
        };

        foreach (Transform b in bulletBodies) {
            Bullet bullet = new Bullet(b);
            b.parent = null;
            bullets.Add(bullet);
            bullet.Impact();
        }

        bulletCount = bullets.Count;
    }

    void FixedUpdate() {
        if (hasAuthority && fireRatetimer > 0) {
            fireRatetimer -= Time.deltaTime;
        }

        foreach (Bullet b in bullets) {
            if(b.Update(Time.deltaTime) == Bullet.BulletState.Active) {
                b.bulletBody.position += b.bulletBody.forward * Time.deltaTime * bulletSpeed;
                if (hasAuthority) {
                    Collider[] hits = Physics.OverlapSphere(b.bulletBody.position, bulletRadius, layers, QueryTriggerInteraction.Ignore);
                    foreach(Collider hit in hits) {
                        if (hit.tag.Equals("minion") || hit.tag.Equals("Player")) {
                            if (hit.GetComponent<Team>().GetTeam() != GetComponent<Team>().GetTeam()) {
                                Debug.Log("Hit minion");
                                hit.GetComponent<Health>().Damage(bulletDamage);
                                BulletImpact(b);
                                break;
                            }
                        }
                        else if (hit.gameObject.layer == defLayer) {
                            Debug.Log("Hit terrain");
                            BulletImpact(b);
                            break;
                        }
                    }
                }
            }
        }
    }

    void BulletImpact(Bullet b) {
        int i = bullets.IndexOf(b);
        if (isServer) {
            RpcBulletImpact(i);
        }
        else {
            CmdBulletImpact(i);
        }
    }

    [Command]
    void CmdBulletImpact(int i) {
        RpcBulletImpact(i);
    }

    [ClientRpc]
    void RpcBulletImpact(int i) {//Play effect at bullet position and reset bullet
        iSparks.position = bullets[i].bulletBody.position;
        impactSparks.Emit(iSparks, impactParticles);
        bullets[i].Impact();
    }

    override public void Attack() {
        if (fireRatetimer <= 0) {
            if (isServer) {
                RpcShoot(bulletIndex);
            }
            else {
                CmdShoot(bulletIndex);
            }
            fireRatetimer = 60f / roundsPerMinute;
            weaponFired?.Invoke();
        }
    }

    private void Shoot(bool second) {
        if(bulletIndex == bulletCount)
            bulletIndex = 0;
        if (!second) {
            bullets[bulletIndex].bulletBody.position = launchPoint1.position;
            bullets[bulletIndex].bulletBody.rotation = launchPoint1.rotation;


            muzzleFlash.Emit(mFlash, muzzleParticles);

            shellEmitter.Emit(eShell, 1);
        }
        else {
            bullets[bulletIndex].bulletBody.position = launchPoint2.position;
            bullets[bulletIndex].bulletBody.rotation = launchPoint2.rotation;

            muzzleFlash.Emit(mFlash, muzzleParticles);
            shellEmitter.Emit(eShell, 1);
        }
        bullets[bulletIndex].ShootBullet(bulletDecayTime);

        bulletIndex++;
        if (!second)
            StartCoroutine("SecondShot");
    }

    [ClientRpc]
    private void RpcShoot(int i) {
        bulletIndex = i;
        Shoot(false);
    }

    [Command]
    private void CmdShoot(int i) {
        RpcShoot(i);
    }

    IEnumerator SecondShot() {
        yield return new WaitForSeconds(secondShotDelay);
        Shoot(true);
        yield break;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        foreach (Bullet b in bullets) {
            Gizmos.DrawSphere(b.bulletBody.position, bulletRadius);
        }
    }
#endif
}

internal class Bullet {
    public Bullet(Transform body) {
        bulletBody = body;
    }
    public Transform bulletBody;
    public enum BulletState { Active, Inactive }
    public BulletState state = BulletState.Inactive;
    public float decayTimer;

    public void ShootBullet(float decayTime) {
        decayTimer = decayTime;
        state = BulletState.Active;
    }

    public void Impact() {
        state = BulletState.Inactive;
        bulletBody.position = Vector3.down * 55f;
    }

    public BulletState Update(float deltaTime) {
        if (state == BulletState.Active) {
            decayTimer -= deltaTime;
            if (decayTimer <= 0) {
                Impact();
            }
        }
        return state;
    }
}
