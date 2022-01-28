using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameStarter : NetworkBehaviour
{
    [SerializeField] [SyncVar] int playersReady, mapMade, matchSeed, currencyPerWave, playersNeededToBeReady;
    [SerializeField] float currencyWave;
    [SyncVar] float matchTimer;
    float currencyTimer;
    bool gameStarted;
   
    UI ui;
    PlayerCurrency[] pCs;
    ulong IDcount = 0;

    private void Start() {
        if (isServer) {
            playersNeededToBeReady = NetworkServer.connections.Count;
            Debug.Log("GameStarter: Players Connected:" + playersNeededToBeReady);
            playersReady++;//Once when the server starts;
            matchSeed = ParseSeed(GameObject.Find("Local").GetComponent<PlayerConstructor>().seed);
        }
        else {
            Increment(); //And the second when player 2 is ready;
        }
        ReadyUp();
    }

    [Command(requiresAuthority =false)]
    private void Increment() {
        playersReady++;
    }

    private void FixedUpdate() {
        if (!gameStarted)
            return;
        ui.UpdateMatchTimer(matchTimer);

        if (!isServer)
            return;
        currencyTimer -= Time.deltaTime;
        if(currencyTimer <= 0) {
            currencyTimer = currencyWave;
            foreach(PlayerCurrency pC in pCs) {
                pC.AddShinies(currencyPerWave);
            }
        }
        matchTimer += Time.deltaTime;
    }

    private void Ready() {
        if (playersNeededToBeReady > playersReady)
            return;
        RpcGenerateMap(matchSeed);
        Debug.Log("GameStarter: Players Connected:" + playersNeededToBeReady);
        StartCoroutine("StartGame");
    }

    private void ReadyUp() {
        if (isServer) {
            Ready();
        }
        else {
            CmdReady();
        }
    }

    public void Made() {
        if (isServer) {
            mapMade++;
        }
        else {
            CmdMapMade();
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdMapMade() {
        Made();
    }
    
    IEnumerator StartGame() {
        //CmdGenerateMap(matchSeed);
        while (mapMade < playersNeededToBeReady)
            yield return new WaitForEndOfFrame();

        SetupPlayers();
        ReleasePlayers();
        FindObjectOfType<MinionManager>().StartWaveSystem();
        pCs = FindObjectsOfType<PlayerCurrency>();
        StartMatchTimer();
        foreach (MinionSumoner mS in FindObjectsOfType<MinionSumoner>()) {
            mS.Setup(pCs);
        }
        foreach (GateScript gate in FindObjectsOfType<GateScript>()) {
            gate.Setup();
        }
        foreach (ControlPoint cP in FindObjectsOfType<ControlPoint>()) {
            cP.Setup();
        }
    }



    [Command(requiresAuthority = false)]
    private void CmdReady() {
        Ready();
    }

    [ClientRpc]
    private void RpcGenerateMap(int matchSeed) {
        this.matchSeed = matchSeed;
        FindObjectOfType<MapBuilder>().GenerateMap(matchSeed);
    }

    [ClientRpc]
    private void SetupPlayers() {
        foreach (PlayerController pC in FindObjectsOfType<PlayerController>()) {
            pC.Setup();
        }
    }

    [ClientRpc]
    private void ReleasePlayers() {
        foreach (PlayerController pC in FindObjectsOfType<PlayerController>()) {
            pC.Release();
        }
    }

    [ClientRpc]
    private void StartMatchTimer() {
        ui = FindObjectOfType<UI>();
        gameStarted = true;
    }

    private int ParseSeed(int seedS) {
        if(seedS == 0) { //No seed is random seed
            return Random.Range(int.MinValue, int.MaxValue);
        }
        return seedS;
    }

    public ulong NextID() {
        IDcount++;
        return IDcount;
    }
}