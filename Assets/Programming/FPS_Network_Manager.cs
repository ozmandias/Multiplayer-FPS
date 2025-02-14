using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class FPS_Network_Manager : NetworkManager {
    [SerializeField] string networkAddress = "";
    [SerializeField] UserInfo user;

    public UserInfo GetUser() {
        return user;
    }
    
    public void SetUser(UserInfo _user) {
        user = _user;
    }
}