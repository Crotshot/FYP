using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Mirror.FizzySteam;
using System;
using UnityEngine.AI;
using kcp2k;

public class GameNetworkManager : NetworkManager
{
    [SerializeField] bool useSteam;

    public static event Action ClientOnConnected;
    public static event Action ClientOnDisconnected;
    public static event Action ServerOnConnected;
    public static event Action ServerOnDisconnected;
    //public static event Action OnServerStopped;
    
    //Prefabs to be spawned for the players
    [Header("Room")] [SerializeField] private Net_Room playerRoomPrefab = null;
    [Header("Game")] [SerializeField] private Net_Player gamePlayerPrefab = null;

    //List of all connected players
    public List<Net_Room> RoomPlayers { get; } = new List<Net_Room>();
    public List<Net_Player> GamePlayers { get; } = new List<Net_Player>();

    private _SceneManager sM;

    public int readyPlayers = 0;

    public override void Awake()
    {
        if (useSteam) {
            transport = GetComponent<FizzySteamworks>();
            GetComponent<KcpTransport>().enabled = false;
        }
        else {
            transport = GetComponent<KcpTransport>();
            GetComponent<FizzySteamworks>().enabled = false;
        }

        base.Awake();
        sM = GetComponent<_SceneManager>();
    }

    #region Server
    public override void OnServerConnect(NetworkConnection conn) //Can be rejumbled later when game scene is implemented
    {
        readyPlayers = 0;
        if (sM.GetSceneName() == "Lobby")
        {
            ServerOnConnected?.Invoke();
            //Debug.Log("Player: " + conn.address.ToString() + " joined");
        }
        else
        {
            conn.Disconnect();
        }
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        readyPlayers = 0;
        if (sM.GetSceneName() == "Lobby")
        {
            //Debug.Log("Player: " + conn.address.ToString() + " has left");
            ServerOnDisconnected?.Invoke();
        }

        //Can this be moved to only work when in the lobby
        var player = conn.identity.GetComponent<Net_Room>();
        RoomPlayers.Remove(player);

        base.OnServerDisconnect(conn);
    }

    public override void ServerChangeScene(string newSceneName)
    {
<<<<<<< Updated upstream
        Debug.Log( $">Scene Change to {newSceneName}");
=======
        //Debug.Log( $">Scene Change to {newSceneName}");
>>>>>>> Stashed changes
        readyPlayers = 0;
        if (sM.GetSceneName() == "Lobby" && newSceneName.StartsWith("Selection")) {
            for (int i = RoomPlayers.Count - 1; i >= 0; i--) {
                var conn = RoomPlayers[i].connectionToClient;
                var gameplayerInstance = Instantiate(gamePlayerPrefab);
                gameplayerInstance.SetDisplayName(RoomPlayers[i].DisplayName);

                NetworkServer.Destroy(conn.identity.gameObject);
                NetworkServer.ReplacePlayerForConnection(conn, gameplayerInstance.gameObject);
            }
        }
        else if (sM.GetSceneName() == "Selection") {

        }
        base.ServerChangeScene(newSceneName);
    }

    public override void OnServerChangeScene(string newSceneName)
    {
        readyPlayers = 0;
        base.OnServerChangeScene(newSceneName);
        //throw new NotImplementedException();
    }

    //Only called if in the lobby due to OnServerConnect() removing players who attempt to connect when outside the Lobby scene
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        readyPlayers = 0;
        Net_Room roomInstance = Instantiate(playerRoomPrefab);
        NetworkServer.AddPlayerForConnection(conn, roomInstance.gameObject);
    }

    public void StartGame()
    {
        ServerChangeScene("Selection");
    }

    public override void OnStopServer()
    {
        //OnServerStopped?.Invoke();
        RoomPlayers.Clear();
        GamePlayers.Clear();
    }
    #endregion

    #region Client
    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        //When client joins to server in lobby scene
        if (sM.GetSceneName() == "Lobby")
            ClientOnConnected?.Invoke();
    }
    
    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        //When client leaves to server in lobby scene
        if (sM.GetSceneName() == "Lobby")
            ClientOnDisconnected?.Invoke();
    }
    #endregion

    public void StartMatch() {
        readyPlayers++;
        if (readyPlayers == NetworkServer.connections.Count) {
            ServerChangeScene("Arena");
        }
    }

    public void Leave() {
        NetworkServer.DisconnectAll();
    }

    public void LeaveDelay() {
        Invoke("Leave", 4.85f);
    }

    public bool isUsingSteam() {
        return useSteam;
    }
}