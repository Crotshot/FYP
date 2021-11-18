using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Respawn : NetworkBehaviour
{
    Transform player;
    List<GameObject> spawnPoints = new List<GameObject>();
    Health health;

    public void Setup() {
        player = transform.parent;
        health = player.GetComponent<Health>();
        health.Dead += RespawnPlayer;
        transform.parent = null;

        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Respawn"))
            spawnPoints.Add(obj);
        RespawnPlayer();
    }

    public void OnDestroy() {
        health.Dead -= RespawnPlayer;
    }

    void RespawnPlayer() {
        if (isServer) {
            health.ResetHealth();
            player.transform.position = spawnPoints[Random.Range(0, spawnPoints.Count)].transform.position;
        }
        else {
            CmdRespawnPlayer();
        }
    }

    [Command]
    void CmdRespawnPlayer() {
        RespawnPlayer();
    }
}

//public void Damage(float damage) {
//    if (isServer) {
//        currentHealth -= damage;
//        //regenTimer = regenDelay;
//        if (currentHealth != 0) {
//            damaged?.Invoke(currentHealth);
//            return;
//        }
//        dead?.Invoke();
//    }
//    else {
//        CmdDamage(damage);
//    }
//}

//[Command]
//void CmdDamage(float damage) {
//    Damage(damage);
//}

//Current temporary code is using multiple spawn points placed in the arena and then moving players to these locations for sapwning
//In later versions this will be rmeoved from the players and added to the Game Manager