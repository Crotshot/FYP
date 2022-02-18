using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;
using Helpers = Crotty.Helpers.StaticHelpers;

public class MinionSpawner : MinionSpawner_Base {
    [SerializeField] GameObject minion_ranged; //1 = Melee & also player purchased, 2 = Ranged
    [SerializeField] Transform[] path; //Assigned control point is last path point

    private void Start() {
        if (!isServer)
            Destroy(this);
        else {
            pool = FindObjectOfType<MinionPool>();
            mm = FindObjectOfType<MinionManager>();
        }
    }

    public void SpawnWave(int melee, int ranged) {
        MinionDat dat1 = new MinionDat(melee, 0, minion_1, "Base_Melee"), dat2 = new MinionDat(ranged, (melee + 1) * 0.66f, minion_ranged, "Base_Ranged");
        StartCoroutine("WaveSpawning", dat1);
        StartCoroutine("WaveSpawning", dat2);
    }

    IEnumerator WaveSpawning(MinionDat dat) {
        yield return new WaitForSeconds(dat.delay);
        while (dat.num > 0) {
            GameObject minion = SpawnMinion(dat.obj,false, dat.mType);
            dat.num--;
            float timer = 0.66f;
            while (timer > 0) {
                if (minion == null)
                    break;
                minion.transform.position = Helpers.Vector3Follow(spawnPoint.position, releasePoint.position, (0.66f - timer) / 0.66f);
                timer -= 0.03f;
                yield return new WaitForSeconds(0.03f);
            }
            if(minion != null) {
                minion.GetComponent<NavMeshAgent>().enabled = true;
                MinionController mc = minion.GetComponent<MinionController>();
                mc.enabled = true;
                mc.AddPathPoints(path);
                mc.AssignControlPoint(path[path.Length - 1]);
                mm.AddMinion(mc);
            }
        }
    }

    internal class MinionDat {
        public int num;
        public float delay;
        public GameObject obj;
        public string mType;

        public MinionDat(int num, float delay, GameObject obj, string mType) {
            this.num = num;
            this.delay = delay;
            this.obj = obj;
            this.mType = mType;
        }
    }
}