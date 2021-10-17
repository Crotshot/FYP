using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{

}

//[SerializeField] float respawnVariety;
//Health hp;

//private void Awake() {
//    hp = transform.parent.GetComponent<Health>();
//    hp.damaged.AddListener(RespawnPlayer);
//    transform.parent = null;
//}

//void RespawnPlayer(float health) {
//    if(health <= 0) {
//        hp.transform.position = transform.position + new Vector3(Random.Range(-respawnVariety, respawnVariety), 0, Random.Range(-respawnVariety, respawnVariety));
//        hp.ResetHealth();
//    }
//}