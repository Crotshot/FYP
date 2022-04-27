using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MinionManager : NetworkBehaviour {

    [SerializeField] float waveTimer = 20f;
    float timer;

    [SerializeField] MinionSpawner t1_E, t1_W, t1_C, t2_E, t2_W, t2_C;
    private bool bt_G = false, e_G = false, w_G = false, started = false;

    [SerializeField] MinionPath NW_C, NE_C, C_SW, C_S, SW_S, SE_S, NW_SW, NE_SE; //All paths except spawn path for team 1
    [SerializeField] MinionPath SW_C, SE_C, C_NE, C_N, NW_N, NE_N, SW_NW, SE_NE; //All paths except spawn path for team 2

    [SerializeField] ControlPoint BN, BS, C, NW, NE, SW, SE;
    [SerializeField] List<MinionController> minions_1 = new List<MinionController>(), minions_2 = new List<MinionController>();
    [SerializeField] int maxWaveSize = 10;

    private void Start() {
        if (!isServer)
            Destroy(this);
    }

    public void StartWaveSystem() {
        timer = waveTimer;
        started = true;
    }

    private void Update() {
        if (!started)
            return;
        if (timer > 0)
            timer -= Time.deltaTime;
        else {
            timer = waveTimer;
            #region Team One Minion Logic
            if (bt_G) {//TEAM ONE //If middle gates are open
                if (C.GetOwningTeam() == 1) {
                    if ((NW.GetOwningTeam() == 1 || NE.GetOwningTeam() == 1) && (SW.GetOwningTeam() == 1 || SE.GetOwningTeam() == 1)) {
                        foreach (MinionController min in minions_1) {
                            if (!min.isBaseMinion())
                                continue;
                            if (BS.GetCurrentMinions(1) < BS.GetCurrentMaxMinions()) {
                                if (min.GetAssignedControlPoint() == C.transform) {
                                    min.AddPathPoints(C_S.points);
                                    min.AssignControlPoint(BS.transform);
                                    continue;
                                }
                                else if (min.GetAssignedControlPoint() == SW.transform && SW.GetOwningTeam() == 1) {
                                    min.AddPathPoints(SW_S.points);
                                    min.AssignControlPoint(BS.transform);
                                }
                                else if (min.GetAssignedControlPoint() == SE.transform && SE.GetOwningTeam() == 1) {
                                    min.AddPathPoints(SE_S.points);
                                    min.AssignControlPoint(BS.transform);
                                }
                            }
                            else {
                                break;
                            }
                        }
                    }
                    else {
                        foreach (MinionController min in minions_1) {
                            if (!min.isBaseMinion())
                                continue;
                            if (SW.GetCurrentMinions(1) < SW.GetCurrentMaxMinions()) {
                                if (min.GetAssignedControlPoint() == C.transform) {
                                    min.AddPathPoints(C_SW.points);
                                    min.AssignControlPoint(SW.transform);
                                    continue;
                                }
                            }
                            else {
                                break;
                            }
                        }
                    }
                }
                MinionSpawnWave(minions_1, C, t1_C, 1);
            }
            if(NE.GetOwningTeam() == 1) {
                if (e_G) { //If east gate is open
                    foreach (MinionController min in minions_1) {
                        if (!min.isBaseMinion())
                            continue;
                        if (SE.GetCurrentMinions(1) < SE.GetCurrentMaxMinions()) {
                            if (min.GetAssignedControlPoint() == NE.transform) {
                                min.AddPathPoints(NE_SE.points);
                                min.AssignControlPoint(SE.transform);
                                continue;
                            }
                        }
                        else {
                            break;
                        }
                    }
                }
                foreach (MinionController min in minions_1) {
                    if (!min.isBaseMinion())
                        continue;
                    if (C.GetCurrentMinions(1) < C.GetCurrentMaxMinions()) {
                        if (min.GetAssignedControlPoint() == NE.transform) {
                            min.AddPathPoints(NE_C.points);
                            min.AssignControlPoint(C.transform);
                            continue;
                        }
                    }
                    else {
                        break;
                    }
                }
            }
            MinionSpawnWave(minions_1, NE, t1_E, 1);

            if (NW.GetOwningTeam() == 1) {
                if (w_G) { //If east gate is open
                    foreach (MinionController min in minions_1) {
                        if (!min.isBaseMinion())
                            continue;
                        if (SW.GetCurrentMinions(1) < SW.GetCurrentMaxMinions()) {
                            if (min.GetAssignedControlPoint() == NW.transform) {
                                min.AddPathPoints(NW_SW.points);
                                min.AssignControlPoint(SW.transform);
                                continue;
                            }
                        }
                        else {
                            break;
                        }
                    }
                }
                foreach (MinionController min in minions_1) {
                    if (!min.isBaseMinion())
                        continue;
                    if (C.GetCurrentMinions(1) < C.GetCurrentMaxMinions()) {
                        if (min.GetAssignedControlPoint() == NW.transform) {
                            min.AddPathPoints(NW_C.points);
                            min.AssignControlPoint(C.transform);
                            continue;
                        }
                    }
                    else {
                        break;
                    }
                }
            }
            MinionSpawnWave(minions_1, NW, t1_W, 1);
            #endregion
            #region Team Two Minion Logic
            if (bt_G) {
                if (C.GetOwningTeam() == 2) {
                    if ((SW.GetOwningTeam() == 2 || SE.GetOwningTeam() == 2) && (NW.GetOwningTeam() == 2 || NE.GetOwningTeam() == 2)) {
                        foreach (MinionController min in minions_2) {
                            if (!min.isBaseMinion())
                                continue;
                            if (BN.GetCurrentMinions(2) < BN.GetCurrentMaxMinions()) {
                                if (min.GetAssignedControlPoint() == C.transform) {
                                    min.AddPathPoints(C_N.points);
                                    min.AssignControlPoint(BN.transform);
                                    continue;
                                }
                                else if (min.GetAssignedControlPoint() == NW.transform && NW.GetOwningTeam() == 2) {
                                    min.AddPathPoints(NW_N.points);
                                    min.AssignControlPoint(BN.transform);
                                }
                                else if (min.GetAssignedControlPoint() == NE.transform && NE.GetOwningTeam() == 2) {
                                    min.AddPathPoints(NE_N.points);
                                    min.AssignControlPoint(BN.transform);
                                }
                            }
                            else {
                                break;
                            }
                        }
                    }
                    else {
                        foreach (MinionController min in minions_2) {
                            if (!min.isBaseMinion())
                                continue;
                            if (NW.GetCurrentMinions(2) < NW.GetCurrentMaxMinions()) {
                                if (min.GetAssignedControlPoint() == C.transform) {
                                    min.AddPathPoints(C_NE.points);
                                    min.AssignControlPoint(NE.transform);
                                    continue;
                                }
                            }
                            else {
                                break;
                            }
                        }
                    }
                }
                MinionSpawnWave(minions_2, C, t2_C,2);
            }

            if (SW.GetOwningTeam() == 2) {
                if (w_G) { //If east gate is open
                    foreach (MinionController min in minions_2) {
                        if (!min.isBaseMinion())
                            continue;
                        if (NW.GetCurrentMinions(2) < NW.GetCurrentMaxMinions()) {
                            if (min.GetAssignedControlPoint() == SW.transform) {
                                min.AddPathPoints(SW_NW.points);
                                min.AssignControlPoint(NW.transform);
                                continue;
                            }
                        }
                        else {
                            break;
                        }
                    }
                }
                foreach (MinionController min in minions_2) {
                    if (!min.isBaseMinion())
                        continue;
                    if (C.GetCurrentMinions(2) < C.GetCurrentMaxMinions()) {
                        if (min.GetAssignedControlPoint() == SW.transform) {
                            min.AddPathPoints(SW_C.points);
                            min.AssignControlPoint(C.transform);
                            continue;
                        }
                    }
                    else {
                        break;
                    }
                }
            }
            MinionSpawnWave(minions_2, SW, t2_W,2);

            if (SE.GetOwningTeam() == 2) {
                if (e_G) { //If east gate is open
                    foreach (MinionController min in minions_2) {
                        if (!min.isBaseMinion())
                            continue;
                        if (NE.GetCurrentMinions(2) < NE.GetCurrentMaxMinions()) {
                            if (min.GetAssignedControlPoint() == SE.transform) {
                                min.AddPathPoints(SE_NE.points);
                                min.AssignControlPoint(NE.transform);
                                continue;
                            }
                        }
                        else {
                            break;
                        }
                    }
                }
                foreach (MinionController min in minions_2) {
                    if (!min.isBaseMinion())
                        continue;
                    if (C.GetCurrentMinions(2) < C.GetCurrentMaxMinions()) {
                        if (min.GetAssignedControlPoint() == SE.transform) {
                            min.AddPathPoints(SE_C.points);
                            min.AssignControlPoint(C.transform);
                            continue;
                        }
                    }
                    else {
                        break;
                    }
                }
            }
            MinionSpawnWave(minions_2, SE, t2_E,2);
            #endregion
        }
    }

    private void MinionSpawnWave(List<MinionController> minions, ControlPoint p, MinionSpawner mS, int team) {
        int minionsNeeded = p.GetCurrentMaxMinions() - p.GetCurrentMinions(team), meleeC = 0, rangedC = 0, meleeN = 0, rangedN = 0;
        foreach (MinionController min in minions) {
            if (min.GetAssignedControlPoint() == p.transform) {
                if (min.GetMinionType().Equals("Base_Melee")) {
                    meleeC++;
                }
                else if (min.GetMinionType().Equals("Base_Ranged")) {
                    rangedC++;
                }
            }
        }
        if (minionsNeeded > maxWaveSize)
            minionsNeeded = maxWaveSize;
        while (minionsNeeded > 0) {
            if (meleeC <= rangedC * 1.5f) {
                meleeN++;
                meleeC++;
            }
            else {
                rangedN++;
                rangedC++;
            }
            minionsNeeded--;
        }
        if (meleeN > 0 || rangedN > 0) {
            mS.SpawnWave(meleeN, rangedN);
        }
    }

    public void OpenGate(string gateName) {
        if (gateName.Equals("bt_G")) {
            bt_G = true;
        }
        else if (gateName.Equals("e_G")) {
            e_G = true;
        }
        else if (gateName.Equals("w_G")) {
            w_G = true;
        }
    }

    public void AddMinion(MinionController min) {
        if(min.GetComponent<Team>().GetTeam() == 1) {
            minions_1.Add(min);
        }
        else if (min.GetComponent<Team>().GetTeam() == 2) {
            minions_2.Add(min);
        }
    }

    public void RemoveMinion(MinionController min) {
        if (min.GetComponent<Team>().GetTeam() == 1) {
            minions_1.Remove(min);
        }
        else if (min.GetComponent<Team>().GetTeam() == 2) {
            minions_2.Remove(min);
        }
    }
}

[System.Serializable]
public class MinionPath {
    [SerializeField] public Transform[] points;
}

/*
 * 
 * Minion control movement logic
 * 
foreach team
    //Middle point
    if(middle gates are open)
        if(we own 1 control point on both sides of the map && middle)
            //Advance mid minions towards enemey base
            //Advance forward minions
        else if(we own middle point)
            //advance up 10 minions from middle point to long side point (NE,SW)

        //Spawn up to 10 minions from mid spawner for mid point
    
    //Side Points, starting with short side  (NW, SE)
    if(side gates are open)
       if(forward point needs minions)
          //Advance to point minions from first point to second
       if(middle point needs minions)
          //Advance minions to middle point
       if(first point needs minions)
         //Spawn up to 10 minions for first point
    else if(both side gates are closed)
        if(middlePoint has less than 20 minions assigned)
            //Advance on middle point
        //(reinforce side points with up to 10 minions)

Assigning minion 
    Base minions are set to path state, they are given a set of points to go through,
    they stop on the last point and change state to defence.
    When minions recieve an order before reaching their last point, the new points will
    be appended to the end.
 */
