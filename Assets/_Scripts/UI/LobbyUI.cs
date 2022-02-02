using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Steamworks;
using Mirror;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] GameObject mainPanel, lobbyPanel, joinPanel, hostPanel;
    [SerializeField] TMP_Text title, joinText, lobbyText, hostText;
    [SerializeField] TMP_InputField inputIP, inputName, hostIP;
    [SerializeField] Button startButton, hostButton, joinButton;
    [SerializeField] bool useSteam = false;

    private _SceneManager sM;
    private string playerName = "_";

    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> lobbyEntered;

    private void Awake()
    {
        sM = FindObjectOfType<_SceneManager>();
        //startButton.interactable = false;//----------------------------------------->Important Re-enable later
        hostButton.interactable = false;
        joinButton.interactable = false;
    }

    private void Start()
    {
        GameManager mang = FindObjectOfType<GameManager>();
        if (mang != null) {
            if(mang.TryGetComponent(out SteamManager man)) {
                if (man.enabled) {
                    useSteam = true;
                }
                else {
                    useSteam = false;
                }
            }
        }

        if (useSteam) {
            lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
            gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
            lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        }


        GameNetworkManager.ClientOnConnected += HandleClientConnected;
        GameNetworkManager.ClientOnDisconnected += HandleClientDisconnected;
        GameNetworkManager.ServerOnConnected += HandleServerConnected;
        GameNetworkManager.ServerOnDisconnected += HandleServerDisconnected;
    }

    private void OnLobbyCreated(LobbyCreated_t callback) {
        if (callback.m_eResult != EResult.k_EResultOK) {
            mainPanel.SetActive(true);
            return;
        }

        NetworkManager.singleton.StartHost();

        SteamMatchmaking.SetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby),
            "HostAddress",
            SteamUser.GetSteamID().ToString());

        lobbyPanel.SetActive(true);
        mainPanel.SetActive(false);
    }

    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback) {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback) {
        if (NetworkServer.active) { return; }

        string hostAddress = SteamMatchmaking.GetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby),
            "HostAddress");

        NetworkManager.singleton.networkAddress = hostAddress;
        NetworkManager.singleton.StartClient();

        mainPanel.SetActive(false);
        lobbyPanel.SetActive(true);
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
        if (useSteam) {
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, 2);
            return;
        }
        hostPanel.SetActive(true);
        mainPanel.SetActive(false);
        hostText.text = "Enter IP to host a lobby";
    }

    public void B_HostLobby() {
        hostPanel.SetActive(false);
        lobbyPanel.SetActive(true);

        lobbyText.text = "Waiting for opponent . . .";

        string address = hostIP.text;
        if (address == "")
            address = "localhost";

        NetworkManager.singleton.networkAddress = address;
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
        hostPanel.SetActive(false);
    }

    public void B_LeaveLobby()
    {
        mainPanel.SetActive(true);
        lobbyPanel.SetActive(false);
        hostPanel.SetActive(false);

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