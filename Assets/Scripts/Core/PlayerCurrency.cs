using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerCurrency : NetworkBehaviour{
    [SerializeField] [SyncVar] int shinies;

    public void AddShinies(int more) {
        shinies += more;
    }

    public int GetShinies() {
        return shinies;
    }

    public bool SpendShinies(int attempt) {
        if (attempt > shinies) {
            return false;
        }
        shinies -= attempt;
        return true;
    }
}