﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] GameObject mainPanel, lobbyPanel, joinPanel;
    [SerializeField] TMP_Text title, joinText, lobbyText;
    [SerializeField] TMP_InputField inputIP, inputName;
    [SerializeField] Button startButton, hostButton, joinButton;

    private _SceneManager sM;
    private string playerName = "_";

    private void Awake()
    {
        sM = FindObjectOfType<_SceneManager>();
        //startButton.interactable = false;//----------------------------------------->Important Re-enable later
        hostButton.interactable = false;
        joinButton.interactable = false;
    }

    private void Start()
    {
        GameNetworkManager.ClientOnConnected += HandleClientConnected;
        GameNetworkManager.ClientOnDisconnected += HandleClientDisconnected;
        GameNetworkManager.ServerOnConnected += HandleServerConnected;
        GameNetworkManager.ServerOnDisconnected += HandleServerDisconnected;
    }

    private void OnDestroy()
    {
        GameNetworkManager.ClientOnConnected -= HandleClientConnected;
        GameNetworkManager.ClientOnDisconnected -= HandleClientDisconnected;
        GameNetworkManager.ServerOnConnected -= HandleServerConnected;
        GameNetworkManager.ServerOnDisconnected -= HandleServerDisconnected;
    }

    public string GetPlayerName()
    {
        return playerName;
    }

    #region EventListeners
    private void HandleClientConnected() //Runs for the joining players client, runs once when the host makes the lobby and again when player 2 joins
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            lobbyText.text = "Waiting for opponent . . .";
            //startButton.interactable = false;//----------------------------------------->Important Re-enable later
        }
        else
        {
            lobbyText.text = "Connected to Host, waiting for them to start the match!";
            lobbyPanel.SetActive(true);
            joinPanel.SetActive(false);
        }
    }

    private void HandleClientDisconnected() //If Host Leaves-> Close lobby for client
    {
        lobbyText.text = "Host has left"; //Ideally instead this would change the start button to a Re-Host button
        NetworkManager.singleton.StopClient();
    }

    private void HandleServerConnected() //Runs on host when player 2 joins, does not run on player 2's client
    {
        if (NetworkServer.connections.Count == 2 && NetworkServer.active && NetworkClient.isConnected) //Local connections -> Might be an issue later
        {
            lobbyText.text = "Opponent Connected, start when ready!";
            startButton.interactable = true;
        }
    }

    private void HandleServerDisconnected() //Runs on host when P2 disconnects
    {
        if (NetworkServer.active && NetworkClient.isConnected) //If P2 leaves -> Update Host to say waiting for opponent
        {
            lobbyText.text = "Waiting for opponent . . .";
            //startButton.interactable = false;//----------------------------------------->Important Re-enable later
        }
    }
    #endregion

    #region UI Buttons
    public void B_Host()
    {
        mainPanel.SetActive(false);
        lobbyPanel.SetActive(true);

        lobbyText.text = "Waiting for opponent . . .";
        NetworkManager.singleton.StartHost();
    }

    public void B_Join()
    {
        joinPanel.SetActive(true);
        mainPanel.SetActive(false);
        joinText.text = "Enter IP to join a lobby";
    }

    public void B_JoinLobby()
    {
        string address = inputIP.text;
        if (address == "")
            address = "localhost";
        NetworkManager.singleton.networkAddress = address;
        NetworkManager.singleton.StartClient();
    }

    public void B_Back()
    {
        mainPanel.SetActive(true);
        joinPanel.SetActive(false);
    }

    public void B_LeaveLobby()
    {
        mainPanel.SetActive(true);
        lobbyPanel.SetActive(false);

        if (NetworkServer.active && NetworkClient.isConnected)
            NetworkManager.singleton.StopHost();
        else
            NetworkManager.singleton.StopClient();
        sM.LoadScene(sM.GetSceneName());
    }

    public void B_BackToMenu()
    {
        sM.LoadScene("MainMenu");
    }

    public void B_StartGame()
    {
        //Change scene in Network manager
        ((GameNetworkManager)NetworkManager.singleton).StartGame();
    }

    public void B_ConfirmName()
    {
        playerName = inputName.text;

        if(playerName.Length > 0)
        {
            hostButton.interactable = true;
            joinButton.interactable = true;
        }
        else
        {
            hostButton.interactable = false;
            joinButton.interactable = false;
        }
    }
    #endregion
}