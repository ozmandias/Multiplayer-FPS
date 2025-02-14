using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KillfeedNotification : MonoBehaviour {
    [SerializeField] Text killerText;
    [SerializeField] Text victimText;
    [SerializeField] float clearDelay = 3f;

    void Start() {
        StartCoroutine(ClearNotification());
    }

    public void SetupKillfeedNotification(string killerName, string victimName) {
        if(killerName == "") {
            killerName = "Game";
        }
        killerText.text = killerName;
        victimText.text = victimName;
    }

    IEnumerator ClearNotification() {
        yield return new WaitForSeconds(clearDelay);
        Destroy(this.gameObject);
    }
}