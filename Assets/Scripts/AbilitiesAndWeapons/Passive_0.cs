using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Passive_0 : NetworkBehaviour 
{
    [SerializeField] Transform spawnPoint;
    GameObject projectilePrefab;

    private void Start() {
        if (!hasAuthority && !GetComponent<PlayerController>().getOfflineTest())
            Destroy(this);
        
        if(TryGetComponent(out RangedAttack atk)) {
            if(atk != null) {
                atk.weaponFired.AddListener(Shoot);
                projectilePrefab = GetComponent<RangedAttack>().GetShellPrefab();
            }
        }
    }

    private void Shoot() {
        if (isServer) {
            GameObject newProj = Instantiate(projectilePrefab, spawnPoint);
            NetworkServer.Spawn(newProj);
            newProj.GetComponent<Team>().SetTeam(GetComponent<Team>().GetTeam());
            newProj.transform.parent = null;
        }
        else {
            cmdDoubleTap();
        }
    }

    [Command]
    private void cmdDoubleTap() {
        Shoot();
    }
}




//[SerializeField] float delay = 0.3f;
//[SerializeField] Transform spawnPoint;
//GameObject projectilePrefab;

//private void Start() {
//    if (!isLocalPlayer)
//        return;
//    GetComponent<RangedAttack>().weaponFired.AddListener(cmdDoubleTap);
//    projectilePrefab = GetComponent<RangedAttack>().GetShellPrefab();
//}

//[Command]
//private void cmdDoubleTap() {
//    GameObject newProj = Instantiate(projectilePrefab, spawnPoint);
//    NetworkServer.Spawn(newProj);
//    newProj.GetComponent<BasicShell>().Make(GetComponent<RangedAttack>().GetTeam());
//    newProj.transform.parent = null;
//}