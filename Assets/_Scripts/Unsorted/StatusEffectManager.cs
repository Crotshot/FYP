using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class StatusEffectManager : NetworkBehaviour {
    /// <summary>
    /// When a character is created they have a a StatusComponent, the status component alters the character based on status effects
    /// The status effect Components are also responsible for health regen on characters
    /// 
    /// NOTE: If the same is applied multiple times the for example Speed 30% 2secs & Speed 10% for 5secs 
    /// Which ever effect has the highest value calculated by multiplying effect strength by time remaining will be used => 30% for 2 secs = 0.6
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
    /// Poison      -> Character is toxic and taking X posion adamage per tick
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
    [SerializeField] int tickRate = 4;//Ticks per second
    float tickTime, tickTimer;
    List<Status> statusList;

    public void AddStatus(Status status) {
        statusList.Add(status);
    }

    private void Start() {
        if (!isServer)
            return;

        statusList = new List<Status>();
        tickTime = 1 / ((float)tickRate);
        tickTimer = tickTime;
    }

    private void Update() {
        if (!isServer)
            return;

        if (tickTimer > 0) {
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

    public int GetTickRate() {
        return tickRate;
    }
}
