using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

public class EntryTrigger : NetworkBehaviour
{
    public UnityEvent<GameObject> entryTouched;

    private void Start() {
        if (!isServer)
            Destroy(this);
        if (entryTouched == null)
            entryTouched = new UnityEvent<GameObject>();
    }

    private void OnTriggerEnter(Collider other) {
        if(other.tag == "minion") {
            if (!other.GetComponent<MinionController>().isBaseMinion()) {
                entryTouched?.Invoke(other.gameObject);
                Debug.Log("Entry Triggered");
            }
        }
    }
}
