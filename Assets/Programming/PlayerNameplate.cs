using UnityEngine;
using UnityEngine.UI;

public class PlayerNameplate : MonoBehaviour {
    [SerializeField]Camera localPlayerCamera;
    [SerializeField]Text nameplateText;

    void Start() {
        
    }
    
    void LateUpdate() {
        FaceCamera();
    }

    void FaceCamera() {
        if(localPlayerCamera) {
            transform.LookAt(localPlayerCamera.transform);
            gameObject.transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
            // transform.Rotate(0, 180f, 0);
        }
    }

    public void SetLocalPlayerCamera(Camera _localPlayerCamera) {
        localPlayerCamera = _localPlayerCamera;
    }

    public void SetNameplateText(string _text) {
        nameplateText.text = _text;
    }
}