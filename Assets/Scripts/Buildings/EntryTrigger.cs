using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EntryTrigger : MonoBehaviour
{
    public UnityEvent<GameObject> entryTouched;

    private void Start() {
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
