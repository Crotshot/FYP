using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    Transform player;
    Health playerHealth;
    List<GameObject> spawnPoints = new List<GameObject>();

    private void Awake() {
        player = transform.parent;
        playerHealth = player.GetComponent<Health>();
        playerHealth.damaged.AddListener(RespawnPlayer);
        transform.parent = null;

        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Respawn"))
            spawnPoints.Add(obj);
        RespawnPlayer(-1);
    }

    void RespawnPlayer(float health) {
        if (health <= 0) {
            player.position = spawnPoints[Random.Range(0, spawnPoints.Count)].transform.position;
            playerHealth.ResetHealth();
        }
    }
}

//Current temporary code is using multiple spawn points placed in the arena and then moving players to these locations for sapwning
//In later versions this will be rmeoved from the players and added to the Game Manager