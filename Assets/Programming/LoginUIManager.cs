using UnityEngine;

public class LoginUIManager : MonoBehaviour {
    public static LoginUIManager instance;

    void Awake() {
        if(instance != null) {
            Destroy(this.gameObject);
        } else {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
}