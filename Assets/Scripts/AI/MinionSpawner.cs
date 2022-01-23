using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;
using Helpers = Crotty.Helpers.StaticHelpers;

public class MinionSpawner : NetworkBehaviour
{
    [SerializeField] GameObject minion_melee, minion_ranged; //1 = Melee & also player purchased, 2 = Ranged
    [SerializeField] Transform spawnPoint, releasePoint;
    [SerializeField] Transform[] path; //Assigned control point is last path point

    private MinionPool pool;
    private MinionManager mm;
    int melee, ranged;

    private void Start() {
        if (!isServer)
            //Destroy(this); 
            Debug.Log("Nuffin");
        else {
            pool = FindObjectOfType<MinionPool>();
            mm = FindObjectOfType<MinionManager>();
        }
    }

    //TEST CODE
    public bool spawnWave;
    private void Update() {
        if (spawnWave) {
            if (isServer) {
                spawnWave = false;
                SpawnWave(6, 4);
            }
            else {
                spawnWave = false;
                CmdSpawnWave();
            }
        }
    }

    [Command (requiresAuthority = false)]
    public void CmdSpawnWave() {
        SpawnWave(10, 0);
    }
    //
    public void SpawnWave(int melee, int ranged) {
        this.melee = melee;
        this.ranged = ranged;
        StartCoroutine("WaveSpawning");
    }

    IEnumerator WaveSpawning() {
        while (melee > 0) {
            GameObject minion = SpawnMinion("Base_Melee");
            melee--;
            float timer = 0.66f;
            while (timer > 0) {
                minion.transform.position = Helpers.Vector3Follow(spawnPoint.position, releasePoint.position, (0.66f - timer) / 0.66f);
                timer -= 0.03f;
                yield return new WaitForSeconds(0.03f);
            }
            minion.GetComponent<NavMeshAgent>().enabled = true;
            MinionController mc = minion.GetComponent<MinionController>();
            mc.enabled = true;
            mc.AddPathPoints(path);
            mc.AssignControlPoint(path[path.Length - 1]);
            mm.AddMinion(mc);
        }
        while (ranged > 0) {
            GameObject minion = SpawnMinion("Base_Ranged");
            ranged--;
            float timer = 0.66f;
            while (timer > 0) {
                minion.transform.position = Helpers.Vector3Follow(spawnPoint.position, releasePoint.position, (0.66f - timer) / 0.66f);
                timer -= 0.03f;
                yield return new WaitForSeconds(0.03f);
            }
            minion.GetComponent<NavMeshAgent>().enabled = true;
            MinionController mc = minion.GetComponent<MinionController>();
            mc.enabled = true;
            mc.AddPathPoints(path);
            mc.AssignControlPoint(path[path.Length -1]);
            mm.AddMinion(mc);
        }
    }

    public GameObject SpawnMinion(string type) {
        GameObject freshMinion = pool.FindMinionOfType(type);
        if (freshMinion == null) {
            if (type.Equals("Base_Melee"))
                freshMinion = Instantiate(minion_melee, spawnPoint);
            else
                freshMinion = Instantiate(minion_ranged, spawnPoint);
            NetworkServer.Spawn(freshMinion);
        }
        freshMinion.transform.parent = null;
        freshMinion.transform.position = spawnPoint.position;
        freshMinion.transform.rotation = spawnPoint.rotation;
        freshMinion.GetComponent<Team>().SetTeam(GetComponent<Team>().GetTeam());
        Color c = GetComponent<Team>().GetTeamColor();
        freshMinion.GetComponent<Team>().SetTeamColor(c.r, c.g, c.b, c.a);
        freshMinion.GetComponent<MinionController>().Setup();
        return freshMinion;
    }
}