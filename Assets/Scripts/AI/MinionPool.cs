using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MinionPool : NetworkBehaviour
{
    List<GameObject> inactiveMinions = new List<GameObject>();

    public GameObject FindMinionOfType(string type) {
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
    }
}
