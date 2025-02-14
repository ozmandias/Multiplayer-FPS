using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MatchManager : MonoBehaviour {
    NetworkManager matchNetworkManager;
    NetworkDiscovery matchNetworkDiscovery;
    string matchName = "";
    [SerializeField]List<MatchInfo> matchList = new List<MatchInfo>();
    bool runAsClient = false;
    bool matchManagerRunning = false;
    public float timeout = 1f;
    
    public delegate void OnAddMatchList(MatchInfo newMatchInfo);
    public OnAddMatchList OnAddMatchListCallback;

    public delegate void OnRemoveMatch(MatchInfo removingMatch);
    public OnRemoveMatch OnRemoveMatchCallback;

    public delegate void OnMatchSelect(string _matchName);
    public OnMatchSelect OnMatchSelectCallback;

    #region Singleton
        public static MatchManager instance;

        public void Awake() {
            if(instance != null) {
                return;
            }
            instance = this;
        }
    #endregion

    void Start() {
        matchNetworkManager = NetworkManager.singleton;
        matchNetworkDiscovery = matchNetworkManager.gameObject.GetComponent<NetworkDiscovery>();
        matchManagerRunning = true;

        // matchNetworkManager.StartMatchMaker();
        // SetNetworkDiscovery("client");

        StartCoroutine(FindMatches());
        StartCoroutine(ClearDisconnectedMatches());
    }

    void Update() {

    }

    public void CreateGame() {
        SetNetworkDiscovery("server");
        matchNetworkManager.StartHost();
    }

    public void JoinGame() {
        NetworkManager.singleton.networkAddress = matchName;
        matchNetworkManager.StartClient();
    }

    public void Refresh() {
        SetNetworkDiscovery("client");
    }

    public void SetMatchName(string newMatchName) {
        matchName = newMatchName;
        
        if(OnMatchSelectCallback != null) {
            OnMatchSelectCallback.Invoke(matchName);
        }
    }

    public void SetNetworkDiscovery(string type) {
        // matchNetworkDiscovery.broadcastPort = Random.Range(0, 1001);
        // Debug.Log("port: " + matchNetworkDiscovery.broadcastPort);
        if(matchNetworkDiscovery) {
            if(matchNetworkDiscovery.running) {
                matchNetworkDiscovery.StopBroadcast();
            }
            matchNetworkDiscovery.Initialize();
            if(type=="server") {
                matchNetworkDiscovery.StartAsServer();
            } else if(type=="client") {
                runAsClient = matchNetworkDiscovery.StartAsClient();
                // Debug.Log("runAsClient: " + runAsClient);
            }
        }
    }

    public void AddToMatchList(MatchInfo newMatchInfo) {
        if(matchList.Any(match => match.address == newMatchInfo.address) == false) {
            matchList.Add(newMatchInfo);
            // Debug.Log("matchList: " + matchList);
            // Debug.Log("matchList length: " + matchList.Count);

            if(OnAddMatchListCallback != null) {
                OnAddMatchListCallback.Invoke(newMatchInfo);
            }
        } else {
            // Debug.Log("MatchInfo exists");

            int matchInfoPosition = matchList.FindIndex(match => match.address == newMatchInfo.address);
            // Debug.Log("matchInfoPosition: " + matchInfoPosition);
            // Debug.Log("matchList count: " + matchList.Count);
            matchList[matchInfoPosition].connectTime = Time.time + timeout;
        }
    }

    public void RemoveFromMatchList(MatchInfo matchToRemove) {
        if(matchList.Any(match => match.address == matchToRemove.address) == true) {
            matchList.Remove(matchToRemove);

            if(OnRemoveMatchCallback != null) {
                OnRemoveMatchCallback.Invoke(matchToRemove);
            }
        }
    }

    public void SelectMatch(string matchAddress) {
        // Debug.Log("SelectMatch(" + matchAddress + ")");
        SetMatchName(matchAddress);
    }

    public void ExitGame() {
        SceneManager.LoadScene("login");
        DatabaseManager.instance.ShowLogInUICanvas();
        // StartCoroutine(ExitGameCoroutine());
    }

    IEnumerator FindMatches() {
        while(runAsClient == false) {
            // Debug.Log("FindMatches");
            Refresh();
            yield return new WaitForSeconds(5f);
        }
    }

    IEnumerator ClearDisconnectedMatches() {
        List<int> removePositions = new List<int>();
        
        while(matchManagerRunning) {
            // Debug.Log("ClearDisconnectedMatches");
            
            foreach(MatchInfo match in matchList) {
                if(match.connectTime <= Time.time) {
                    // Debug.Log("Clear Match");
                    removePositions.Add(matchList.IndexOf(match));
                }
            }

            if(removePositions.Count > 0) {
                foreach(int removePosition in removePositions) {
                    RemoveFromMatchList(matchList[removePosition]);   
                }
                removePositions.Clear();
            }
            yield return new WaitForSeconds(timeout);
        }
    }

    IEnumerator ExitGameCoroutine() {
        yield return new WaitForSeconds(1f);
        DatabaseManager.instance.ShowLogInUICanvas();
    }
}