using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Minion_Ranged : Minion_Attack {
    bool projectTileLaunched, projCreated;
    [SerializeField] GameObject archingProjectilePrefab, projectileSpawnPoint;

    public void Setup() {
        if (!isServer)
            Destroy(this);

        animatedWeapon.localPosition = animatedTimings[0].pos;
        animatedWeapon.localScale = animatedTimings[0].scale;
        animatedWeapon.localEulerAngles = animatedTimings[0].localEuler;
    }
    private void FixedUpdate() {
        AnimatedAttack(LaunchProjectile);
    }

    private void LaunchProjectile() {
        if (!projectTileLaunched) {
            projectTileLaunched = true;
            GameObject obj = Instantiate(archingProjectilePrefab, projectileSpawnPoint.transform.position, projectileSpawnPoint.transform.rotation);
            NetworkServer.Spawn(obj);
            obj.transform.position = projectileSpawnPoint.transform.position;
            obj.transform.position = projectileSpawnPoint.transform.position;
            obj.transform.parent = null;
            obj.GetComponent<Team>().SetTeam(GetComponent<Team>().GetTeam());
            Color c = GetComponent<Team>().GetTeamColor();
            obj.GetComponent<Team>().SetTeamColor(c.r, c.g, c.b, c.a);
            if(GetComponent<MinionController>().GetMinionTarget() != null)
                obj.GetComponent<ArchingProjectile>().Setup(GetComponent<MinionController>().GetMinionTarget().position);
        }
    }

    public override void Attack() {
        if (attacking)
            return;
        attackTimer = 0;
        projectTileLaunched = false;
        projCreated = false;
        attacking = true;
        index = 1;
    }
}
