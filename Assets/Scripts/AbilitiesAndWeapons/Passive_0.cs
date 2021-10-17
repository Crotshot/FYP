using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Passive_0 : NetworkBehaviour
{

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