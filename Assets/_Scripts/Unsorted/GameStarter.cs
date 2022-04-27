using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameStarter : NetworkBehaviour
{
    [SerializeField] [SyncVar] int playersReady, mapMade, matchSeed, currencyPerWave, playersNeededToBeReady;
    [SerializeField] float currencyWave;
    [SerializeField] Transform[] spawnerSpots1, spawnerSpots2; 
    [SyncVar] float matchTimer;
    float currencyTimer;
    bool gameStarted;
   
    UI ui;
    ulong IDcount = 0;
    PlayerCurrency[] pCs;
    private void Start() {
        if (isServer) { //When loaded in this is the first thing called
            playersNeededToBeReady = NetworkServer.connections.Count;
            Debug.Log("GameStarter: Players Connected:" + playersNeededToBeReady);
            playersReady++;//Increment players ready
            matchSeed = ParseSeed(GameObject.Find("Local").GetComponent<PlayerConstructor>().seed);
        }
        else {
            Increment(); //And the second increment when player 2 is loaded in;
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
        while (mapMade < playersNeededToBeReady)
            yield return new WaitForEndOfFrame();

        SetupSpawners();
        yield return new WaitForSeconds(0.1f);
        pCs = FindObjectsOfType<PlayerCurrency>();
        SetupPlayers();
        ReleasePlayers();
        FindObjectOfType<MinionManager>().StartWaveSystem();
        StartMatchTimer();
        
        foreach (Gate gate in FindObjectsOfType<Gate>()) {
            gate.Setup();
        }
        foreach (ControlPoint cP in FindObjectsOfType<ControlPoint>()) {
            cP.Setup();
        }

        foreach (MinionSumoner mS in FindObjectsOfType<MinionSumoner>()) {
            mS.Setup(mS.transform.position, mS.transform.eulerAngles);
        }
    }

    GameManager gM;
    private void SetupSpawners() {
        gM = FindObjectOfType<GameManager>();//Called on the server, sets up team 1 spawners
        int count = 0;
        foreach(CharacterStats minionS in GameObject.Find("Local").GetComponent<PlayerConstructor>().minions){
            if(minionS != null) {
                GameObject spawner = Instantiate(gM.GetMinionSpawner(minionS.name), spawnerSpots1[count]);
                NetworkServer.Spawn(spawner);
            }
            count++;
        }
        RpcPingCLientForSpawners();
    }

    [ClientRpc]
    private void RpcPingCLientForSpawners() { //Need to send minions spawner names to server to spawn
        if (isClient && isServer)
            return;
        string[] names = new string[4];
        int count = 0;
        foreach (CharacterStats minionS in GameObject.Find("Local").GetComponent<PlayerConstructor>().minions) {
            if (minionS != null) {
                names[count] = minionS.name;
            }
            else {
                names[count] = "null"; //Cannot send an array of strings so concatinating all strings to one string 
            }
            count++;
        }
        string sb = "";
        foreach (string s in names) {
            sb += s + "*";
        }
        //sb = sb.Substring(0,sb.Length-2);
        SpawnClientSpawners(sb);
    }

    [Command(requiresAuthority = false)]
    private void SpawnClientSpawners(string s) {
        string[] names = s.Split('*');
        int count = 0;
        foreach (string name in names) {
            Debug.Log(name);
            if (name != "null" && name.Length > 0) { //THIS IS ACTING SUS?????
                GameObject spawner = Instantiate(gM.GetMinionSpawner(name), spawnerSpots2[count]);
                NetworkServer.Spawn(spawner);
            }
            count++;
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
            pC.GetComponent<PlayerMinions>().Setup();
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