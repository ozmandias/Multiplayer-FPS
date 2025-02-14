using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    [Header("Panels")]
    [SerializeField]GameObject aimCircle;
    [SerializeField]GameObject hitPanel;
    [SerializeField]GameObject healthPanel;
    [SerializeField]GameObject pausePanel;
    [SerializeField]GameObject confirmExitPanel;
    [SerializeField]GameObject scoreboardPanel;
    [SerializeField]GameObject scoreboardContent;
    [SerializeField]GameObject killfeedContent;

    [Header("UI Objects")]
    [SerializeField]Image healthBar;
    [SerializeField]Text healthBarText;
    public Button exitMatchButton;

    [Header("Prefabs")]
    [SerializeField]GameObject scoreboardInfo;
    [SerializeField]GameObject killfeedNotification;

    float maxHealthBarWidth = 500f;
    float maxHealthBarLimit = 100f;
    float currentHealthBarLimit = 0f;
    GameManager uiGameManager;
    NetworkManager uiNetworkManager;

    #region Singleton
        public static UIManager instance;

        public void Awake() {
            if(instance != null) {
                return;
            }
            instance = this;
        }
    #endregion

    void Start() {
        uiGameManager = GameManager.instance;
        uiNetworkManager = NetworkManager.singleton;

        HideAimCircle();
        HideHealthPanel();

        GameManager.instance.OnPlayerNameCompleteCallback = GameManager.instance.OnPlayerNameCompleteCallback + AddScoreboardInfo;
    }

    void Update() {
        ToggleMenu();
        ToggleScoreboard();
    }

    public void ToggleMenu() {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            // pausePanel.SetActive(!pausePanel.activeInHierarchy);
            // if(pausePanel.activeInHierarchy == false) {
                // HideConfirmExitPanel();
            // }

            if(pausePanel.activeInHierarchy == false && confirmExitPanel.activeInHierarchy == false) {
                ShowMenu();
            } else if(pausePanel.activeInHierarchy == true && confirmExitPanel.activeInHierarchy == false) {
                HideMenu();
            } else if(pausePanel.activeInHierarchy == false && confirmExitPanel.activeInHierarchy == true) {
                HideConfirmExitPanel();
            }
        }
    }

    public void ToggleScoreboard() {
        if(Input.GetKeyDown(KeyCode.Tab)) {
            ShowScoreboard();
        }
        if(Input.GetKeyUp(KeyCode.Tab)) {
            HideScoreboard();
        }
    }

    public void ShowMenu() {
        uiGameManager.UnlockCursor();
        pausePanel.SetActive(true);
    }

    public void HideMenu() {
        if(confirmExitPanel.activeInHierarchy == false) {
            uiGameManager.LockCursor();
        }
        pausePanel.SetActive(false);
    }

    public void ShowConfirmExitPanel() {
        confirmExitPanel.SetActive(true);
        HideMenu();
    }

    public void HideConfirmExitPanel() {
        confirmExitPanel.SetActive(false);
        if(pausePanel.activeInHierarchy == false) {
            ShowMenu();
        }
    }

    public void ExitMatch(string userType) {
        if(userType == "host") {
            uiNetworkManager.StopHost();
        } else if(userType == "client") {
            uiNetworkManager.StopClient();
        }
    }

    public void ShowAimCircle() {
        if(aimCircle) {
            aimCircle.SetActive(true);
        }
    }

    public void HideAimCircle() {
        if(aimCircle) {
            aimCircle.SetActive(false);
        }
    }

    public void ShowScoreboard() {
        scoreboardPanel.SetActive(true);
    }

    public void HideScoreboard() {
        scoreboardPanel.SetActive(false);
    }

    public void ToggleHitPanel() {
        StartCoroutine(ToggleHitPanelCoroutine());
    }

    public void ShowHitPanel() {
        hitPanel.SetActive(true);
    }

    public void HideHitPanel() {
        hitPanel.SetActive(false);
    }

    public void ShowHealthPanel() {
        healthPanel.SetActive(true);
    }

    public void HideHealthPanel() {
        healthPanel.SetActive(false);
    }

    public void UpdateHealthBar(int incomingDamage) {
        float updateWidth = (healthBar.rectTransform.sizeDelta.x / 100) * incomingDamage;
        healthBar.rectTransform.sizeDelta = new Vector2(healthBar.rectTransform.sizeDelta.x - updateWidth, healthBar.rectTransform.sizeDelta.y);
        ReduceHealthBarTextLimit((float)incomingDamage);
    }

    public void ResetHealthBar() {
        healthBar.rectTransform.sizeDelta = new Vector2(maxHealthBarWidth, healthBar.rectTransform.sizeDelta.y);
        ResetHealthBarTextLimit();
    }

    public void SetHealthBarTextLimit(float limit) {
        currentHealthBarLimit = limit;
        if(currentHealthBarLimit < 0) {
            currentHealthBarLimit = 0;
        }
        healthBarText.text = "" + currentHealthBarLimit;
    }

    public void ReduceHealthBarTextLimit(float reduceLimit) {
        currentHealthBarLimit = currentHealthBarLimit - reduceLimit;
        if(currentHealthBarLimit < 0) {
            currentHealthBarLimit = 0;
        }
        healthBarText.text = "" + currentHealthBarLimit;
    }

    public void ResetHealthBarTextLimit() {
        currentHealthBarLimit = maxHealthBarLimit;
        healthBarText.text = "" + currentHealthBarLimit;
    }

    public void AddScoreboardInfo(string playerID, string playerName, int killsCount, int deathsCount, bool isLocalPlayer) {
        bool recordExists = false;
        Component[] scoreboardRecords = scoreboardContent.GetComponentsInChildren(typeof(ScoreboardInfo));
        // Debug.Log("scoreboardRecords: " + scoreboardRecords);

        if(scoreboardRecords.Length > 0) {
            foreach(ScoreboardInfo scoreboardRecord in scoreboardRecords) {
                // Debug.Log("scoreboardRecord: " + scoreboardRecord);
                if(scoreboardRecord.GetOnlinePlayerID() == playerID) {
                    recordExists = true;
                    // Debug.Log("recordExists: " + recordExists);
                    scoreboardRecord.SetupScoreboardInfo(playerID, playerName, killsCount, deathsCount, isLocalPlayer);
                }
            }
        }

        if(recordExists == false) {
            GameObject newScoreboardInfoObject = Instantiate(scoreboardInfo, scoreboardContent.transform);
            ScoreboardInfo newScoreboardInfo = newScoreboardInfoObject.GetComponent<ScoreboardInfo>();
            newScoreboardInfo.SetupScoreboardInfo(playerID, playerName, killsCount, deathsCount, isLocalPlayer);
        }
    }

    public void AddKillFeedNotification(string killerName, string victimName) {
        GameObject newKillfeedNotificationObject = Instantiate(killfeedNotification, killfeedContent.transform);
        KillfeedNotification newKillfeedNotification = newKillfeedNotificationObject.GetComponent<KillfeedNotification>();
        newKillfeedNotification.SetupKillfeedNotification(killerName, victimName);
    }

    public void UpdateScoreboard(NetworkPlayer playerOfScoreboard) {
        Component[] scoreboardRecords = scoreboardContent.GetComponentsInChildren(typeof(ScoreboardInfo));

        if(scoreboardRecords.Length > 0) {
            foreach(ScoreboardInfo scoreboardRecord in scoreboardRecords) {
                if(scoreboardRecord.GetOnlinePlayerID() == playerOfScoreboard.GetPlayerID()) {
                    // Debug.Log("playerName: " + playerOfScoreboard.gameObject.name);
                    scoreboardRecord.SetupScoreboardInfo(scoreboardRecord.GetOnlinePlayerID(), playerOfScoreboard.gameObject.name, playerOfScoreboard.kills, playerOfScoreboard.deaths, playerOfScoreboard.isLocalPlayer);
                }
            }
        }
    }

    public void DeleteScoreboard(NetworkPlayer playerOfScoreboard) {
        Component[] scoreboardRecords = scoreboardContent.GetComponentsInChildren(typeof(ScoreboardInfo));

        if(scoreboardRecords.Length > 0) {
            foreach(ScoreboardInfo scoreboardRecord in scoreboardRecords) {
                if(scoreboardRecord.GetOnlinePlayerID() == playerOfScoreboard.GetPlayerID()) {
                    Destroy(scoreboardRecord.gameObject);
                }
            }
        }
    }

    public IEnumerator ToggleHitPanelCoroutine() {
        // yield return new WaitForSeconds(0.5f);
        ShowHitPanel();
        yield return new WaitForSeconds(0.5f);
        HideHitPanel();
    }
}