using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class GameNetworkManager : NetworkManager
{
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

    public override void Awake()
    {
        base.Awake();
        sM = GetComponent<_SceneManager>();
    }

    #region Server
    public override void OnServerConnect(NetworkConnection conn) //Can be rejumbled later when game scene is implemented
    {
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
        if (sM.GetSceneName() == "Lobby")
        {
            //Debug.Log("Player: " + conn.address.ToString() + " has left");
            ServerOnDisconnected?.Invoke();
        }

        //Can this be moved to only work when we are in the lobby
        var player = conn.identity.GetComponent<Net_Room>();
        RoomPlayers.Remove(player);

        base.OnServerDisconnect(conn);
    }

    public override void ServerChangeScene(string newSceneName)
    {
        if (sM.GetSceneName() == "Lobby" && newSceneName.StartsWith("Arena")) //Change for selection scene
        {
            for (int i = RoomPlayers.Count - 1; i >= 0; i--)
            {
                var conn = RoomPlayers[i].connectionToClient;
                var gameplayerInstance = Instantiate(gamePlayerPrefab);
                gameplayerInstance.SetDisplayName(RoomPlayers[i].DisplayName);

                NetworkServer.Destroy(conn.identity.gameObject);
                NetworkServer.ReplacePlayerForConnection(conn, gameplayerInstance.gameObject);
            }
        }
        base.ServerChangeScene(newSceneName);
    }

    public override void OnServerChangeScene(string newSceneName)
    {
        base.OnServerChangeScene(newSceneName);
        //throw new NotImplementedException();
    }

    //Only called if in the lobby due to OnServerConnect() removing players who attempt to connect when outside the Lobby scene
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        Net_Room roomInstance = Instantiate(playerRoomPrefab);
        NetworkServer.AddPlayerForConnection(conn, roomInstance.gameObject);
    }

    public void StartGame() //Change later to go to selection scene
    {
        ServerChangeScene("Arena");
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
}