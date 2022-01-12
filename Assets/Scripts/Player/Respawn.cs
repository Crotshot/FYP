//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.AI;
//using Mirror;

//public class Respawn : NetworkBehaviour {
//    [SerializeField] Vector3 spawnPoint;
//    [SerializeField] float respawnTime;
//    [SerializeField] PlayerController pC;
//    [SerializeField] NavMeshAgent aI;

//    public void Setup() {
//        if (isServer) {
//            GameObject[] points = GameObject.FindGameObjectsWithTag("Respawn");
//            foreach (GameObject obj in points) {
//                if (obj.GetComponent<Team>().GetTeam() == GetComponent<Team>().GetTeam()) {
//                    spawnPoint = obj.transform.position;
//                    RpcSpawnSetup(spawnPoint);
//                    break;
//                }
//            }
//        }
//        else {
//            CmdSetup();
//        }
//    }

//    [Command]
//    void CmdSetup() {
//        Setup();
//    }

//    [ClientRpc]
//    void RpcSpawnSetup(Vector3 point) {
//        spawnPoint = point;
//        pC = GetComponent<PlayerController>();
//        aI = GetComponent<NavMeshAgent>();
//    }

//    public void RespawnPlayer() {
//        if (isServer) {
//            RPCRespawnPlayer();
//        }
//        else {
//            if (aI != null && pC != null) {
//                aI.enabled = false;
//                pC.enabled = false;
//                transform.position = spawnPoint + Vector3.down * 20f;
//                StartCoroutine("RespawnDelay");
//            }
//        }
//    }

//    IEnumerator RespawnDelay() {
//        yield return new WaitForSeconds(respawnTime);
//        transform.position = spawnPoint;
//        aI.enabled = true;
//        pC.enabled = true;
//        GetComponent<Health>().ResetHealth();
//    }

//    [ClientRpc]
//    void RPCRespawnPlayer() {
//        RespawnPlayer();
//    }
//}