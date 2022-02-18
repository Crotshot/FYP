using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;
using Helpers = Crotty.Helpers.StaticHelpers;

public class MinionSpawner_Base : NetworkBehaviour {
    [SerializeField] protected GameObject minion_1;
    [SerializeField] protected Transform spawnPoint, releasePoint;
    protected MinionPool pool;
    protected MinionManager mm;

    public GameObject SpawnMinion(GameObject min1, bool individualSpawnRoutine, string mType) {
        GameObject freshMinion = pool.FindMinionOfType(mType);
        if (freshMinion == null) {
            freshMinion = Instantiate(min1, spawnPoint);
            NetworkServer.Spawn(freshMinion);
        }
        else
            Debug.Log("Reusing minion from pool instead of making a new one !!!!!");
        freshMinion.transform.parent = null;
        freshMinion.transform.position = spawnPoint.position;
        freshMinion.transform.rotation = spawnPoint.rotation;
        freshMinion.GetComponent<Team>().SetTeam(GetComponent<Team>().GetTeam());
        Color c = GetComponent<Team>().GetTeamColor();
        freshMinion.GetComponent<Team>().SetTeamColor(c.r, c.g, c.b, c.a);
        freshMinion.GetComponent<MinionController>().Setup();
        freshMinion.GetComponent<MinionHealth>().ResetHealth();
        if(individualSpawnRoutine)
            StartCoroutine("MinionSpawn", freshMinion);
        return freshMinion;
    }

    IEnumerator MinionSpawn(GameObject minion) { //Called when the player spawns minions manually
        float timer = 0.21f;
        while (timer > 0) {
            if (minion == null)
                break;
            minion.transform.position = Helpers.Vector3Follow(spawnPoint.position, releasePoint.position, (0.66f - timer) / 0.66f);
            timer -= 0.03f;
            yield return new WaitForSeconds(0.03f);
        }
        if (minion != null) {
            minion.GetComponent<NavMeshAgent>().enabled = true;
            MinionController mc = minion.GetComponent<MinionController>();
            mc.enabled = true;
            //mc.AddPathPoints(path);
            //mc.AssignControlPoint(path[path.Length - 1]);
            mm.AddMinion(mc);
        }
    }
}
