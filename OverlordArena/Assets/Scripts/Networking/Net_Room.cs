using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Net_Room : NetworkBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject lobbyUI = null;
    [SerializeField] private TMP_Text[] playerNameTexts = new TMP_Text[2];

    [SyncVar(hook = nameof(HandleDisplayNameChanged))]
    public string DisplayName = "Loading...";

    private GameNetworkManager room;
    private GameNetworkManager Room { //Always null, when used it gets 'room' instead and if room is null it sets room from the Singleton Network Manger
        get {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as GameNetworkManager;
        }
    }

    public override void OnStartAuthority()
    {
        CmdSetDisplayName(FindObjectOfType<LobbyUI>().GetPlayerName()); //Grab the name set in the Lobby UI and Command the server to set it as this players name
        lobbyUI.SetActive(true);
    }

    public override void OnStartClient()
    {
        Room.RoomPlayers.Add(this);
    }

    public override void OnStopClient()
    {
        Room.RoomPlayers.Remove(this);
    }

    public void HandleDisplayNameChanged(string oldValue, string newValue) => UpdateDisplay();

    private void UpdateDisplay() //Update the name displays of each player
    {
        if (!hasAuthority)
        {
            foreach (var player in Room.RoomPlayers)
            {
                if (player.hasAuthority)
                {
                    player.UpdateDisplay();
                    break;
                }
            }

            return;
        }

        for (int i = 0; i < playerNameTexts.Length; i++)
        {
            playerNameTexts[i].text = "Waiting . . .";
        }
        
        for (int i = 0; i < Room.RoomPlayers.Count; i++)
        {
            playerNameTexts[i].text = Room.RoomPlayers[i].DisplayName;
        }

        //Debug.Log("Player Count: " + Room.RoomPlayers.Count);
        //Weird issue where the first player in the lobby does not get counted unitl player 2 joins
        //Setting the player name directly fixes this issue
        if (Room.RoomPlayers.Count == 0)
        {
            playerNameTexts[0].text = DisplayName;
        }
    }

    [Command]
    private void CmdSetDisplayName(string displayName)
    {
        DisplayName = displayName;
    }
}