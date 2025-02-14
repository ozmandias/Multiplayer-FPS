using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuUIManager : MonoBehaviour {
    [SerializeField]Button joinGameButton;
    MatchManager uiMatchManager;
    [SerializeField] GameObject matchInfo;
    [SerializeField] GameObject matchScrollViewContent;
    Dictionary<MatchInfo, GameObject> matchDictionary = new Dictionary<MatchInfo, GameObject>();

    void Start() {
        uiMatchManager = MatchManager.instance;

        uiMatchManager.OnAddMatchListCallback = uiMatchManager.OnAddMatchListCallback + UIOnAddMatchListCallback;
        uiMatchManager.OnRemoveMatchCallback = uiMatchManager.OnRemoveMatchCallback + UIOnRemoveMatchCallback;
        uiMatchManager.OnMatchSelectCallback = uiMatchManager.OnMatchSelectCallback + UIOnMatchSelectCallback;
    }

    public void UIOnAddMatchListCallback(MatchInfo newMatchInfo) {
        string matchInfoButtonText = newMatchInfo.address.Split(':')[3];
        // Debug.Log("matchInfoButtonText: " + matchInfoButtonText);

        GameObject newMatchInfoObject = Instantiate(matchInfo, matchScrollViewContent.transform);

        Button matchInfoButton = newMatchInfoObject.GetComponent<Button>();
        matchInfoButton.gameObject.transform.GetChild(0).gameObject.GetComponent<Text>().text += " " + matchInfoButtonText; 
        matchInfoButton.onClick.AddListener(()=>uiMatchManager.SelectMatch(newMatchInfo.address));
        
        matchDictionary.Add(newMatchInfo, newMatchInfoObject);
    }

    public void UIOnRemoveMatchCallback(MatchInfo removingMatch) {
        if(matchDictionary.ContainsKey(removingMatch)) {
            // Debug.Log("removingMatch: " + removingMatch + " found in Dictionary.");

            // Debug.Log("removeMatchObject: " + matchDictionary[removingMatch]);
            Destroy(matchDictionary[removingMatch]);
        }
        matchDictionary.Remove(removingMatch);
    }

    public void UIOnMatchSelectCallback(string _matchName) {
        if(_matchName != "") {
            joinGameButton.interactable = true;
        } else {
            joinGameButton.interactable = false;
        }
    }
}