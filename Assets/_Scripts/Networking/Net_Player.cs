using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Net_Player : NetworkBehaviour
{
    [SyncVar] private string displayName = "Loading...";

    private GameNetworkManager room;

    private GameNetworkManager Room {
        get {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as GameNetworkManager;
        }
    }

    public override void OnStartClient() //Adds Net_Player to a list when joining the server
    {
        DontDestroyOnLoad(gameObject);

        Room.GamePlayers.Add(this);
    }

    public override void OnStopClient() //Removes Net_Player from list when leaving the server
    {
        Room.GamePlayers.Remove(this);
    }

    [Server]
    public void SetDisplayName(string displayName)
    {
        this.displayName = displayName;
    }
}