using Mirror;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SteamLobby : MonoBehaviour {
    [SerializeField] TMP_Text username;

    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> lobbyEntered;

    private const string HostAddressKey = "HostAddress";

    private NetworkManager networkManager;

    public static CSteamID LobbyId { get; private set; }

    private void Start() {
        if (!GetComponent<GameNetworkManager>().isUsingSteam())
            return;

        networkManager = GetComponent<NetworkManager>();

        if (!SteamManager.Initialized) {
            return;
        }
        else {
            username.SetText(SteamFriends.GetPersonaName());
        }

        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
    }

    public void HostLobby() {
        LobbyUI lobbyUI = FindObjectOfType<LobbyUI>();
        lobbyUI.DisableMain();
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, networkManager.maxConnections);
    }

    private void OnLobbyCreated(LobbyCreated_t callback) {
        LobbyUI lobbyUI = FindObjectOfType<LobbyUI>();
        if (callback.m_eResult != EResult.k_EResultOK) {
            lobbyUI.SteamFail();
            return;
        }

        LobbyId = new CSteamID(callback.m_ulSteamIDLobby);
        networkManager.StartHost();
        SteamMatchmaking.SetLobbyData(
            LobbyId,
            HostAddressKey,
            SteamUser.GetSteamID().ToString());

        lobbyUI.SteamHost();
    }

    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback) {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback) {
        if (NetworkServer.active) { return; }

        string hostAddress = SteamMatchmaking.GetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby),
            HostAddressKey);

        networkManager.networkAddress = hostAddress;
        networkManager.StartClient();
        LobbyUI lobbyUI = FindObjectOfType<LobbyUI>();
        lobbyUI.SteamClient();
    }
}