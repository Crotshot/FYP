using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public class StatusEffectManager : NetworkBehaviour {
    /// <summary>
    /// When a character is created they have a a StatusComponent, the status component alters the character based on status effects
    /// The status effect Components are also responsible for health regen on characters
    /// 
    /// One thing to note if the same is applied multiple times the for example Speed 30% 2secs & Speed 10% for 5secs
    /// Whichever effect lasts longest will remain.
    /// 
    /// Regular Effects
    /// Stun        -> Character is completely disabled for duration
    /// Speed Boost -> Character is +(Base* %) faster for duration
    /// Slowness    -> Character is -(Base* %) slower for duration
    /// Burning     -> Character is on fire and taking X burn damage per tick
    /// Weakened    -> Character is taking % bonus damage
    /// Abduction   -> Character is disabled and is being moved against their will
    /// Rooted      -> Character cannot move but can still attack
    /// Magnetised  -> Character is being pulled toward location slowly
    /// </summary>

    /// <summary>
    /// Similar again characters have a a BuildingEffectComponent, there is two BEC's one for Conquerors and one for minions
    /// 
    /// Minion
    /// Wheat field 	10% Minion HP
    /// Pumpkin Patch 	15% Minion bonus ability damage
    /// Iron Mine 		10% Minion melee damage
    /// Fletcher 		10% Minion ranged damage
    /// Charged		    15% Minion ms and 15% damage
    /// Drained		    -15% Minion ms and -5% damage
    /// 
    /// Brewery		    1% Both Health regen
    /// 
    /// Sacred Crystal	10% Conqueror cdr - diminishing
    /// Horde increase	+3 Conqueror minions

    /// </summary>
    [SerializeField] float tickRate;//Ticks per second
    float tickTime, tickTimer;
    List<Status> statusList;

    private void Awake() {
        if (!isServer)
            Destroy(this);

        statusList = new List<Status>();
        tickTime = 1 / tickRate;
        tickTimer = tickTime;
    }


    private void Update() {
        if(tickTimer > 0) {
            tickTimer -= Time.deltaTime;
            return;
        }
        tickTimer = tickTime;

        for(int i = statusList.Count -1; i > -1; i--) {
            if (!statusList[i].GetInit()) {
                statusList.Remove(statusList[i]);
                continue;
            }

            statusList[i].StatusUpdate();
        }
    }
}
