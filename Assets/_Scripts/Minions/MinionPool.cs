using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MinionPool : NetworkBehaviour
{
#if UNITY_EDITOR
    [SerializeField] public bool disablePool;
#endif
    List<GameObject> inactiveMinions = new List<GameObject>();

    private void Start() {
        if (!isServer)
            Destroy(this);
    }

    public GameObject FindMinionOfType(string type) {
#if UNITY_EDITOR
        if(disablePool)
            return null;
#endif
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
        
#if UNITY_EDITOR
        if(disablePool)
            NetworkServer.Destroy(minion);
#endif
    }
}
