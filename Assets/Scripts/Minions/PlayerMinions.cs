using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
//A class controlled by the player for commanding minions
public class PlayerMinions : NetworkBehaviour {
    [SyncVar] [SerializeField] int maxFollowers;
    [SyncVar] int followerCount; //Rpc from server for local UI
    [SerializeField] string[] minionTypes;
    [SerializeField] List<MinionController> followerMinions;
    Vector2[] minionPositions;
    float xMod =  1.25f, yMod = 1.25f;
    Inputs inputs;
    Interactor inter;
    bool canRecall, canAttack, canDefend, canRetreat;
    [SerializeField] int minionTypeSelected;

    [SerializeField] bool isCollossus;

    private void Start() {
        if (isCollossus) {
            minionPositions = new Vector2[30];
            minionPositions[25] = new Vector2(6 * xMod, -10.5f * yMod);
            minionPositions[26] = new Vector2(1.5f * xMod, -12 * yMod);
            minionPositions[27] = new Vector2(-1.5f * xMod, -12 * yMod);
            minionPositions[28] = new Vector2(-4.5f * xMod, -12 * yMod);
            minionPositions[29] = new Vector2(4.5f * xMod, -12 * yMod);
        }
        else
            minionPositions = new Vector2[25];

        maxFollowers = minionPositions.Length;
        minionPositions[0] = new Vector2(1.5f * xMod, -6 * yMod);
        minionPositions[1] = new Vector2(-1.5f * xMod, -6 * yMod);
        minionPositions[2] = new Vector2(-4.5f * xMod, -6 * yMod);
        minionPositions[3] = new Vector2(4.5f * xMod, -6 * yMod);
        minionPositions[4] = new Vector2(0, -7.5f * yMod);
        minionPositions[5] = new Vector2(-3 * xMod, -7.5f * yMod);
        minionPositions[6] = new Vector2(3 * xMod, -7.5f * yMod);
        minionPositions[7] = new Vector2(1.5f * xMod, -9 * yMod);
        minionPositions[8] = new Vector2(-1.5f * xMod, -9 * yMod);
        minionPositions[9] = new Vector2(-7.5f * xMod, -6 * yMod);
        minionPositions[10] = new Vector2(7.5f * xMod, -6 * yMod);
        minionPositions[11] = new Vector2(-6 * xMod, -7.5f * yMod);
        minionPositions[12] = new Vector2(6 * xMod, -7.5f * yMod);
        minionPositions[13] = new Vector2(-4.5f * xMod, -9 * yMod);
        minionPositions[14] = new Vector2(4.5f * xMod, -9 * yMod);
        minionPositions[15] = new Vector2(-3 * xMod, -10.5f * yMod);
        minionPositions[16] = new Vector2(0, -10.5f * yMod);
        minionPositions[17] = new Vector2(3 * xMod, -10.5f * yMod);
        minionPositions[18] = new Vector2(-10.5f * xMod, -6 * yMod);
        minionPositions[19] = new Vector2(10.5f * xMod, -6 * yMod);
        minionPositions[20] = new Vector2(-9 * xMod,-7.5f * yMod);
        minionPositions[21] = new Vector2(9 * xMod,-7.5f * yMod);
        minionPositions[22] = new Vector2(-7.5f * xMod,-9 * yMod);
        minionPositions[23] = new Vector2(7.5f * xMod, -9 * yMod);
        minionPositions[24] = new Vector2(-6 * xMod,-10.5f * yMod);

    }

    public void Setup() {
        inputs = FindObjectOfType<Inputs>();
        if (isServer) {
            inter = GetComponent<Interactor>();
        }
    }

    private void FixedUpdate() {
        #region OrderInputs
        if (inputs == null)
            return;

        //degrees += inputs.GetScrollWheelInput();
        //minionTypeSelected = (int)(degrees / 360f) * (minionTypes.Length - 1);

        minionTypeSelected += inputs.GetScrollWheelSpaced();
        if(minionTypeSelected > minionTypes.Length - 1)
            minionTypeSelected = 0;
        else if( minionTypeSelected < 0)
            minionTypeSelected = minionTypes.Length - 1;

        if (inputs.GetCommandRecallHeld()) {
            if (isServer) Recall_Interaction(minionTypeSelected, false); else CmdRecall_Interaction(minionTypeSelected, false);
        }
        else if(inputs.GetCommandRetreatHeld()) {
            if (isServer) OrderRetreat(minionTypeSelected, false); else CmdOrderRetreat(minionTypeSelected, false);
        }

        if (inputs.GetCommandRecallLong()) {
            if (isServer) Recall_Interaction(minionTypeSelected, true); else CmdRecall_Interaction(minionTypeSelected, true);
        }
        else if (inputs.GetCommandRetreatLong()) {
            if (isServer) OrderRetreat(minionTypeSelected, true); else CmdOrderRetreat(minionTypeSelected, true);
        }

        if (inputs.GetCommandRecallInput() > 0) {
            if (canRecall) {
                if (isServer) {
                    Recall_Interaction(minionTypeSelected, false);
                }
                else {
                    CmdRecall_Interaction(minionTypeSelected, false);
                }
            }
            canRecall = false;
        }
        else if (inputs.GetCommandRecallInput() < 0) {
            if (canRetreat) {
                if (isServer) {
                    OrderRetreat(minionTypeSelected, false);
                }
                else {
                    CmdOrderRetreat(minionTypeSelected, false);
                }
            }
            canRetreat = false;
        }
        else {
            canRetreat = true;
            canRecall = true;
        }

        if (inputs.GetCommandAttackHeld()) {
            if (isServer) OrderAttack(minionTypeSelected); else CmdOrderAttack(minionTypeSelected);
        }
        else if (inputs.GetCommandAttackInput() > 0) {
            if (canAttack) {
                if (isServer) {
                    OrderAttack(minionTypeSelected);
                }
                else {
                    CmdOrderAttack(minionTypeSelected);
                }
            }
            canAttack = false;
        }
        else {
            canAttack = true;
        }

        if (inputs.GetCommandDefendHeld()) {
            if (isServer) OrderDefend(minionTypeSelected); else CmdOrderDefend(minionTypeSelected);
        }
        else if(inputs.GetCommandDefendInput() > 0) {
            if (canDefend) {
                if (isServer) {
                    OrderDefend(minionTypeSelected);
                }
                else {
                    CmdOrderDefend(minionTypeSelected);
                }
            }
            canDefend = false;
        }
        else {
            canDefend = true;
        }
        #endregion

        int counter = 0, c2 = 0;
        for (int i = followerMinions.Count - 1; i >= 0; i--) { //LATER will need to be changed 
            if (followerMinions[i] == null) {
                followerMinions.RemoveAt(i);
                continue;
            }
            else if (followerMinions[i].minionState == MinionController.MinionState.Follower
                || followerMinions[i].minionState == MinionController.MinionState.Retreating
                || followerMinions[i].minionState == MinionController.MinionState.Recalling) {
                followerMinions[i].SetDestination(transform.TransformPoint(new Vector3(minionPositions[c2].x, 0, minionPositions[counter].y)));
                c2++;
            }
            counter++;
        }
        followerCount = counter;
    }

    #region Recall / Building interaction
    private void Recall_Interaction(int mT, bool b) {
        Interactable focus = inter.GetFocus();
        if (focus != null) {
            if (focus.TryGetComponent(out MinionSumoner summ) && followerCount < maxFollowers) {
                summ.Summon();
            }
            else if (focus.TryGetComponent(out EntryTrigger trigger)) {
                OrderEnter(mT);//Send the cheapest (or lowest health minion if all are same cost, incases where one minion type is selected) into the trigger
            }
        }
        else {//Recall a minion
            OrderRecall(mT, b);
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdRecall_Interaction(int i, bool b) {
        Recall_Interaction(i, b);
    }
    #endregion

    public void OrderAttack(int mT) {//Go to mouse and attack anything and everything
        foreach (MinionController min in followerMinions) {//When runs out of fight goes back to player
            if ((minionTypes[minionTypeSelected].Equals("All") || minionTypes[minionTypeSelected].Equals(min.GetMinionType())) && min.minionState == MinionController.MinionState.Follower) {
                min.minionState = MinionController.MinionState.Forward;
                min.SetDestination(inter.GetMouseWorldPos());
                break;
            }
        }
    }

    public void OrderDefend(int mT) {//Order minion to stay in spot, will attack anything and everything and will return to spot after fighting is done
        foreach (MinionController min in followerMinions) {
            if ((minionTypes[minionTypeSelected].Equals("All") || minionTypes[minionTypeSelected].Equals(min.GetMinionType())) && min.minionState == MinionController.MinionState.Follower) {
                min.minionState = MinionController.MinionState.Defender;
                min.SetDestination(inter.GetMouseWorldPos());
                break;
            }
        }
    }

    public void OrderRecall(int mT, bool all) {
        foreach (MinionController min in followerMinions) {
            if ((minionTypes[minionTypeSelected].Equals("All") || minionTypes[minionTypeSelected].Equals(min.GetMinionType())) && min.minionState != MinionController.MinionState.Follower && min.minionState != MinionController.MinionState.Retreating && min.minionState != MinionController.MinionState.Recalling) {
                min.minionState = MinionController.MinionState.Recalling;
                if(!all)
                    break;
            }
        }
    }

    public void OrderEnter(int mT) {
        foreach (MinionController min in followerMinions) {
            if ((minionTypes[minionTypeSelected].Equals("All") || minionTypes[minionTypeSelected].Equals(min.GetMinionType())) && min.minionState != MinionController.MinionState.Recalling && min.minionState != MinionController.MinionState.Retreating ) {
                min.minionState = MinionController.MinionState.Entering;
                min.SetDestination(inter.GetFocus().transform.position);
                break;
            }
        }
    }

    public void OrderRetreat(int mT, bool all) {//Recalls minion nearest to worldspacemousepos, minion will ingore all
        foreach (MinionController min in followerMinions) {//Hold to make all retreat
            if ((minionTypes[minionTypeSelected].Equals("All") || minionTypes[minionTypeSelected].Equals(min.GetMinionType())) && min.minionState != MinionController.MinionState.Follower && min.minionState != MinionController.MinionState.Retreating) {
                min.minionState = MinionController.MinionState.Retreating;
                if (!all)
                    break;
            }
        }
    }

    [Command (requiresAuthority = false)]
    public void CmdOrderAttack(int i) {
        OrderAttack(i);
    }

    [Command(requiresAuthority = false)]
    public void CmdOrderDefend(int i) {
        OrderDefend(i);
    }

    [Command(requiresAuthority = false)]
    public void CmdOrderRetreat(int i, bool b) {
        OrderRetreat(i,b);
    }

    public void AddFollower(MinionController cont) {
        if (!followerMinions.Contains(cont)) {
            followerMinions.Add(cont);
            followerCount++;
        }
    }

    public void RemoveFollower(MinionController cont) {
        if (followerMinions.Contains(cont)) {
            followerMinions.Remove(cont);
            followerCount--;
        }
    }

    public string GetMinionCount() {
        return followerCount + " / " + maxFollowers;
    }

    public float GetMinionSelectedAngle() {
        return minionTypeSelected * (360f / minionTypes.Length);
    }
}