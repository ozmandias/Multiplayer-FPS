using UnityEngine;
using UnityEngine.UI;

public class ScoreboardInfo : MonoBehaviour {
    [SerializeField] string onlinePlayerID;
    [SerializeField] Text playerNameText;
    [SerializeField] Text killsText;
    [SerializeField] Text deathsText;
    [SerializeField] Image scoreboardInfoImage;
    Color playerColor = new Color32(144, 190, 255, 255);
    Color enemyColor = new Color32(255, 144, 144, 255);
    

    public void SetupScoreboardInfo(string playerID, string playerName, int killsCount, int deathsCount, bool isLocalPlayer) {
        onlinePlayerID = playerID;
        playerNameText.text = playerName;
        killsText.text = "Kills - " + killsCount;
        deathsText.text = "Deaths - " + deathsCount;

        if(isLocalPlayer) {
            scoreboardInfoImage.color = playerColor;
        } else {
            scoreboardInfoImage.color = enemyColor;
        }
    }

    public string GetOnlinePlayerID() {
        return onlinePlayerID;
    }
}