using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MinionManager : NetworkBehaviour {

    private void Start() {
        if (!isServer)
            return;
    }
}