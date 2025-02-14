using UnityEngine;
using UnityEngine.Networking;

public class FPS_Network_Discovery : NetworkDiscovery {
    public override void OnReceivedBroadcast(string fromAddress, string data) {
        Debug.Log("OnReceivedBroadcast from: " + fromAddress + " with data: " + data);
        // Debug.Log("Time.time: " + Time.time);
        MatchInfo gameMatch = new MatchInfo();
        gameMatch.address = fromAddress;
        gameMatch.data = data;
        gameMatch.connectTime = Time.time + MatchManager.instance.timeout;
        MatchManager.instance.AddToMatchList(gameMatch);
    }
}