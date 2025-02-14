using UnityEngine;

public class PlayerLook : MonoBehaviour {
    [SerializeField] GameObject playerMainBone;
    [SerializeField] GameObject playerRotateBone;
    [SerializeField] Camera weaponCamera;
    float rotateSpeed = 8f;
    float mouseHorizontal = 0f;
    float mouseVertical = 0f;
    float minMouseVertical = -30f;
    float maxMouseVertical = 30f;
    void Start() {
        weaponCamera.nearClipPlane = 0.01f;
    }

    void Update() {
        Rotate();
    }

    void Rotate() {
        mouseHorizontal += Input.GetAxis("Mouse X") * rotateSpeed;
        mouseVertical -= Input.GetAxis("Mouse Y") * rotateSpeed;
        mouseVertical = Mathf.Clamp(mouseVertical, minMouseVertical, maxMouseVertical);

        Vector3 mouseDirection = new Vector3(mouseVertical, mouseHorizontal, 0f);
        
        // playerMainBone.transform.forward = gameObject.transform.forward;
        playerMainBone.transform.forward = playerRotateBone.transform.forward;
        playerMainBone.transform.localEulerAngles = new Vector3(0, playerMainBone.transform.localEulerAngles.y, 0);
        
        // gameObject.transform.rotation = Quaternion.Euler(mouseDirection);
        playerRotateBone.transform.rotation = Quaternion.Euler(mouseDirection);
    }

    public GameObject GetMainBoneObject() {
        return playerMainBone;
    }

    public GameObject GetRotationObject() {
        return playerRotateBone;
    }
}