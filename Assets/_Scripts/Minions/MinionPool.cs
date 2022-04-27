using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MinionPool : NetworkBehaviour
{
<<<<<<< Updated upstream
=======
#if UNITY_EDITOR
    [SerializeField] public bool disablePool;
#endif
>>>>>>> Stashed changes
    List<GameObject> inactiveMinions = new List<GameObject>();

    private void Start() {
        if (!isServer)
            Destroy(this);
    }

    public GameObject FindMinionOfType(string type) {
<<<<<<< Updated upstream
        ////TEMPORARILY DISABLED, strange excess minion spawning issues;
        return null;
=======
#if UNITY_EDITOR
        if(disablePool)
            return null;
#endif
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream

        //DELETE LATER
        NetworkServer.Destroy(minion);
=======
        
#if UNITY_EDITOR
        if(disablePool)
            NetworkServer.Destroy(minion);
#endif
>>>>>>> Stashed changes
    }
}
