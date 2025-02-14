using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    bool cursorActive = false;
    Dictionary<string, NetworkPlayer> onlinePlayers = new Dictionary<string, NetworkPlayer>();
    NetworkPlayer localNetworkPlayer;

    public delegate void OnRegisterPlayer();
    public OnRegisterPlayer OnRegisterPlayerCallback;

    public delegate void OnSetPlayerName(/*string playerID, string newName*/);
    public OnSetPlayerName OnSetPlayerNameCallback;

    public delegate void OnPlayerNameComplete(string playerID, string playerName, int killsCount, int deathsCount, bool isLocalPlayer);
    public OnPlayerNameComplete OnPlayerNameCompleteCallback;

    #region Singleton
        public static GameManager instance;

        public void Awake() {
            if(instance != null){
                return;
            }
            instance = this;
        }
    #endregion

    void Start() {
        UnlockCursor();

        OnRegisterPlayerCallback = OnRegisterPlayerCallback + SetCameraForNameplate;

        OnSetPlayerNameCallback = OnSetPlayerNameCallback + SetOnlinePlayerName;
    }

    void Update() {
        // ToggleCursor();
    }

    void OnGUI() {
        GUIListPlayers();
    }

    void GUIListPlayers() {
        GUILayout.BeginArea(new Rect(100, 200, 500, 500));
        GUILayout.BeginVertical();

        foreach(KeyValuePair<string, NetworkPlayer> playerID in onlinePlayers) {
            // GUILayout.Label(playerID.Key);
            GUILayout.Label(playerID.Value.gameObject.name);
        }

        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    public void RegisterPlayer(string id, NetworkPlayer player) {
        onlinePlayers.Add(id, player);

        if(OnRegisterPlayerCallback != null) {
            OnRegisterPlayerCallback.Invoke();
        }
    }

    public void UnregisterPlayer(string id) {
        onlinePlayers.Remove(id);
    }
    
    public void LockCursor() {
        // Debug.Log("LockCursor");
        cursorActive = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void UnlockCursor() {
        // Debug.Log("UnlockCursor");
        cursorActive = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ToggleCursor() {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            if(cursorActive == true) {
                LockCursor();
            }else{
                UnlockCursor();
            }
        }
    }

    public void SetLocalNetworkPlayer(NetworkPlayer _localNetworkPlayer) {
        localNetworkPlayer = _localNetworkPlayer;

        SetCameraForNameplate();
    }

    public void SetCameraForNameplate() {
        // Debug.Log("SetCameraForNameplate");
        if(localNetworkPlayer){
            foreach(KeyValuePair<string, NetworkPlayer> onlinePlayer in onlinePlayers) {
                // Debug.Log("onlinePlayer ID: " + onlinePlayer.Key + ", " + "onlinePlayer Value: " + onlinePlayer.Value.gameObject.GetComponent<PlayerSetup>().GetPlayerInfo());
                if(onlinePlayer.Key != localNetworkPlayer.GetPlayerID()) {
                    PlayerNameplate onlinePlayerNameplate = onlinePlayer.Value.gameObject.GetComponent<PlayerSetup>().playerNameplate;
                    onlinePlayerNameplate.SetLocalPlayerCamera(localNetworkPlayer.GetNetworkPlayerCamera());
                }
            }
        }
    }

    public void SetOnlinePlayerName(/*string playerID, string onlinePlayerName*/) {
        // Debug.Log("SetOnlinePlayerName");
        foreach(KeyValuePair<string, NetworkPlayer> onlinePlayer in onlinePlayers) {
            // if(onlinePlayer.Key == playerID) {
                // Debug.Log("Player found.");
                // Debug.Log("Player-ID: " + onlinePlayer.Key + ", " + "username: " + onlinePlayer.Value.gameObject.name);

                string onlinePlayerName = onlinePlayer.Value.gameObject.GetComponent<PlayerSetup>().playerName;
                string onlinePlayerID = onlinePlayer.Value.GetPlayerID();

                if(onlinePlayerName == "") {
                    onlinePlayer.Value.gameObject.name = "Player-" + onlinePlayerID;
                } else {
                    onlinePlayer.Value.gameObject.name = onlinePlayerName;
                }

                onlinePlayer.Value.gameObject.GetComponent<PlayerSetup>().playerNameplate.SetNameplateText(onlinePlayer.Value.gameObject.name);

                if(OnPlayerNameCompleteCallback != null) {
                    Debug.Log("completeName: " + onlinePlayer.Value.gameObject.name);
                    OnPlayerNameCompleteCallback.Invoke(onlinePlayerID, onlinePlayer.Value.gameObject.name, onlinePlayer.Value.kills, onlinePlayer.Value.deaths, onlinePlayer.Value.isLocalPlayer);
                }
            // }
        }
    }
}