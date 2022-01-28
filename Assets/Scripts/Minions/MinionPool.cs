using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MinionPool : NetworkBehaviour
{
    List<GameObject> inactiveMinions = new List<GameObject>();

    private void Start() {
        if (!isServer)
            Destroy(this);
    }

    public GameObject FindMinionOfType(string type) {
        ////TEMPORARILY DISABLED, strange excess minion spawning issues;
        //return null;
        GameObject returnedMinion = null;
        foreach (GameObject minion in inactiveMinions) {
            if (minion.GetComponent<MinionController>().GetMinionType().Equals(type)) {
                returnedMinion = minion;
                inactiveMinions.Remove(minion);
                break;
            }
        }
        return returnedMinion;
    }

    public void AddMinionToPool(GameObject minion) {
        inactiveMinions.Add(minion);

        //DELETE LATER
        NetworkServer.Destroy(minion);
    }
}
