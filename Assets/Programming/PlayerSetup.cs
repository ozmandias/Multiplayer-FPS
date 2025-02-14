using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[RequireComponent(typeof(NetworkPlayer))] public class PlayerSetup : NetworkBehaviour {
    UserInfo playerInfo;
    [SyncVar] public string playerName = "";
    [SerializeField]Camera sceneCamera;
    [SerializeField]NetworkPlayer networkPlayer;
    DatabaseManager playerDatabaseManager;
    [SerializeField]UIManager playerUI;
    Utility playerUtility;
    FPS_Network_Manager playerNetworkManager;
    [SerializeField]Behaviour[] componentsToDisable;
    [SerializeField]GameObject[] layerChangeObjects;
    [SerializeField]GameObject playerBlood;
    public PlayerNameplate playerNameplate;

    void Start() {
        if(networkPlayer == null){
            networkPlayer = gameObject.GetComponent<NetworkPlayer>();
        }

        playerDatabaseManager = DatabaseManager.instance;
        playerUI = UIManager.instance;
        playerUtility = Utility.instance;
        playerNetworkManager = (FPS_Network_Manager) NetworkManager.singleton;

        Setup();
    }

    void Update() {
        
    }

    public void Setup() {
        SetPlayerName();

        if(isLocalPlayer){
            sceneCamera = Camera.main;
            // sceneCamera = GameObject.Find("SceneCamera").GetComponent<Camera>();
            if(sceneCamera != null) {
                sceneCamera.gameObject.SetActive(false);
                playerNameplate.gameObject.SetActive(false);
            }

            if(playerUI) {
                playerUI.ShowAimCircle();
                playerUI.ShowHealthPanel();
                playerUI.ResetHealthBarTextLimit();
                playerUI.exitMatchButton.onClick.AddListener(PlayerSetupExit);
            }
            
            GameManager.instance.LockCursor();

            gameObject.tag = "LocalPlayer";

            GameManager.instance.SetLocalNetworkPlayer(networkPlayer);
        }else{
            playerUtility.EnableComponents(componentsToDisable, false);

            playerUtility.SetLayerToGameObjects(layerChangeObjects, "Default");

            gameObject.tag = "NetworkPlayer";
        }
    }

    public void PlayerSetupExit() {
        if(isServer) {
            playerUI.ExitMatch("host");
        } else {
            playerUI.ExitMatch("client");
        }
    }

    public UserInfo GetPlayerInfo() {
        return playerInfo;
    }

    [Client] void SetPlayerName() {
        // Debug.Log("networkManager: " + playerNetworkManager);
        // Debug.Log("playerInfo: " + playerNetworkManager.GetUser());

        playerInfo = playerNetworkManager.GetUser();

        CmdSetPlayerName(playerInfo.username);
    }

    [Command] void CmdSetPlayerName(string _playerName) {
        RpcSetPlayerName(_playerName);
    }

    [ClientRpc] void RpcSetPlayerName(string _playerName) {
        playerName = _playerName;
        // Debug.Log("playerName: " + playerName);

        if(GameManager.instance.OnSetPlayerNameCallback != null) {
            GameManager.instance.OnSetPlayerNameCallback.Invoke(/*networkPlayer.GetPlayerID(), playerName*/);
        }
    }

    public override void OnStartClient() {
        base.OnStartClient();

        GameManager.instance.RegisterPlayer(networkPlayer.GetPlayerID(), networkPlayer);
    }

    public override void OnStartLocalPlayer() {
        base.OnStartLocalPlayer();
    }

    void OnDisable() {
        if(sceneCamera != null) {
            sceneCamera.gameObject.SetActive(true);
        }

        if(isLocalPlayer) {
            playerUI.HideAimCircle();
            playerUI.HideHealthPanel();
            playerUI.ResetHealthBar();
            playerUI.HideMenu();

            string dataText = playerDatabaseManager.databaseDataTranslator.DataToText(networkPlayer.kills, networkPlayer.deaths);
            playerDatabaseManager.SetDataText(dataText);
        }
        
        playerUI.DeleteScoreboard(networkPlayer);

        GameManager.instance.UnregisterPlayer(networkPlayer.GetPlayerID());
    }

    public UIManager GetUI() {
        return playerUI;
    }

    public GameObject GetBlood() {
        return playerBlood;
    }
}