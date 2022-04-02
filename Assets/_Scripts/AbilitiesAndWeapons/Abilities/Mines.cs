using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Helpers = Crotty.Helpers.StaticHelpers;

public class Mines : Ability {
    [SerializeField] float mineChargeTime, minDist, damage, blastRadius, triggerRadius, armTime, decayTime;
    [SerializeField] int maxMines, particles;
    /// <summary>
    /// The amount of mines that can be active at once is equal to the initial List of mines
    /// </summary>
    [SerializeField] List<Transform> mineBodies;
    [SerializeField] LayerMask unitLayer;
    [SerializeField] ParticleSystem explosiveEmitter;
    ParticleSystem.EmitParams emp;//SUed to override partical emmision location

    List<Mine> mines;
    int currentMines, index;
    float mineTimer;

    private void Start() {
        mines = new List<Mine>();
        explosiveEmitter.transform.parent = null;
        emp = new ParticleSystem.EmitParams {
            applyShapeToPosition = true
        };

        foreach (Transform m in mineBodies) {
            Mine mine = new Mine(m);
            m.parent = null;
            mines.Add(mine);
            mine.Detonated();
            //if (hasAuthority)
            //    if (isServer) {
            //        RpcMinePos(mine.GetMinePos(), index);
            //    }
            //    else {
            //        CmdMinePos(mine.GetMinePos(), index);
            //    }
        }

        if (!hasAuthority)
            return;

        SetUp(Cast);
    }

    private void Update() {
        if (!hasAuthority)
            return;
        CoolDown(Time.deltaTime);

        if (currentMines < maxMines) {
            mineTimer -= Time.deltaTime;
            if (mineTimer <= 0) {
                mineTimer += mineChargeTime;
                currentMines++;
            }
        }
        foreach(Mine mine in mines) {
            if(mine.state == Mine.MineState.Active) {
                mine.Update(Time.deltaTime);
                if(mine.state == Mine.MineState.Inactive) { // Mine was moved inside mine class and now needs reflection on client
                    index = mines.IndexOf(mine);
                    if (isServer) {
                        RpcMinePos(mine.GetMinePos(), index);
                    }
                    else {
                        CmdMinePos(mine.GetMinePos(), index);
                    }
                }
            }
            else {
                mine.Update(Time.deltaTime);
            }
            if(mine.state == Mine.MineState.Active) {
                bool steppedOn = false;
                Collider[] cols = Physics.OverlapSphere(mine.GetMinePos(), triggerRadius, unitLayer, QueryTriggerInteraction.Ignore);
                foreach (Collider col in cols) {
                    if(col.TryGetComponent(out Team team)) {
                        if (team.GetTeam() != GetComponent<Team>().GetTeam()) {
                            steppedOn = true;
                            break;
                        }
                    }
                }
                if (steppedOn) {
                    cols = Physics.OverlapSphere(mine.GetMinePos(), blastRadius, unitLayer, QueryTriggerInteraction.Ignore);
                    foreach (Collider col in cols) {
                        if (col.TryGetComponent(out Team team)) {
                            if (team.GetTeam() != GetComponent<Team>().GetTeam()) {
                                team.GetComponent<Health>().Damage(damage);
                            }
                        }
                    }
                    Vector3 minePos = mine.GetMinePos();
                    mine.Detonated();
                    index = mines.IndexOf(mine);
                    if (isServer) {
                        RpcEffect(particles, minePos);
                        RpcMinePos(mine.GetMinePos(), index);
                        
                    }
                    else {
                        CmdEffect(particles, minePos);
                        CmdMinePos(mine.GetMinePos(), index);
                    }
                }
            }
        }    
    }

    private void Cast() {
        if (currentMines < 1)
            return;
        foreach (Mine mine in mines) {
            if (Vector3.Distance(transform.position, mine.GetMinePos()) <= minDist) { //Distance check so mines cannot be stacked
                return;
            }
        }
        if (AbilityUsed()) {
            Mine mine = PickMine();
            mine.SetMine(armTime, decayTime);
            index = mines.IndexOf(mine);
            currentMines--;
            if (isServer) {
                RpcMinePos(transform.position, index);
            }
            else {
                CmdMinePos(transform.position, index);
            }
        }
    }

    [ClientRpc]
    public void RpcMinePos(Vector3 pos, int index) {
        mines[index].SetMinePos(pos);
    }

    [Command]
    public void CmdMinePos(Vector3 pos, int index) {
        RpcMinePos(pos, index);
    }

    [ClientRpc]
    public void RpcEffect(int count, Vector3 pos) {
        emp.position = pos;
        explosiveEmitter.Emit(emp, count);
    }

    [Command]
    public void CmdEffect(int count, Vector3 pos) {
        RpcEffect(count, pos);
    }

    private Mine PickMine() {
        int lowestIndex = 0, currentIndex = 0;
        float lowestTime = 999f;
        foreach(Mine mine in mines) {
            if (mine.state == Mine.MineState.Inactive) {
                currentIndex++;
                return mine;
            }

            if (mine.decayTimer < lowestTime) {
                lowestTime = mine.decayTimer;
                lowestIndex = currentIndex;
            }

            currentIndex++;
        }
        return mines[lowestIndex];//If all mines are active return the oldest;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos() {
        foreach (Mine mine in mines) {
            Gizmos.DrawSphere(mine.GetMinePos(), blastRadius);
        }
    }
#endif 
}

internal class Mine {
    public Mine(Transform body) {
        mineBody = body;
    }
    private Transform mineBody;
    public enum MineState { Active, Inactive, Arming}
    public MineState state = MineState.Inactive;
    public float armTimer, decayTimer;

    public void SetMine(float armTime, float decayTime) {
        armTimer = armTime;
        decayTimer = decayTime;
        state = MineState.Arming;
    }

    public void SetMinePos(Vector3 pos) {
        mineBody.position = pos;
    }

    public Vector3 GetMinePos() {
        return mineBody.position;
    }

    public void Detonated() {
        state = MineState.Inactive;
        mineBody.position = Vector3.down * 50f;
    }

    public MineState Update(float deltaTime) {
        if(state == MineState.Active) {
            decayTimer -= deltaTime;
            if (decayTimer <= 0) {
                Detonated();
            }
        }
        else if (state == MineState.Arming) {
            armTimer -= deltaTime;
            if(armTimer <= 0) {
                state = MineState.Active;
            }
        }

        return state;
    }
}