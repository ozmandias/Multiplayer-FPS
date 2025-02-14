using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponEquipment : MonoBehaviour {
    [SerializeField]NetworkPlayer owner;
    [SerializeField]WeaponInfo primaryWeapon;
    [SerializeField]WeaponInfo secondaryWeapon;
    [SerializeField]WeaponInfo currentWeapon;
    GameObject holdingWeaponObject;
    Utility weaponEquipmentUtility;

    void Start() {
        weaponEquipmentUtility = Utility.instance;

        SetupWeapon();
        // UnequipWeapon();
        // EquipWeapon();
    }

    public void EquipWeapon() {
        currentWeapon = secondaryWeapon;

        GameObject currentWeaponObject = Instantiate(currentWeapon.weaponObject, gameObject.transform);
        if(owner.isLocalPlayer) {
            weaponEquipmentUtility.SetLayerRecursion(currentWeaponObject, "Gun");
        }else{
            weaponEquipmentUtility.SetLayerRecursion(currentWeaponObject, "Default");
        }
        SetHoldingWeaponObject(currentWeaponObject);
        SetCurrentWeaponAnimation(currentWeaponObject);
        SetCurrentWeaponParticle(currentWeaponObject);
        owner.SetWeaponNetworkTransform();
    }

    public void UnequipWeapon() {
        currentWeapon = null;

        GameObject currentWeaponObject = gameObject.transform.GetChild(0).gameObject;
        weaponEquipmentUtility.SetLayerRecursion(currentWeaponObject, "Default");

        Rigidbody currentWeaponBody = currentWeaponObject.GetComponent<Rigidbody>();
        Collider currentWeaponCollider = currentWeaponObject.GetComponent<Collider>();
        
        currentWeaponObject.transform.SetParent(null);
        currentWeaponBody.isKinematic = false;
        currentWeaponCollider.enabled = true;

        // Vector3 throwAwayDirection = currentWeaponObject.transform.forward  + currentWeaponObject.transform.up;
        // float throwAwayForce = 4f;
        // currentWeaponBody.AddForce(throwAwayDirection * throwAwayForce);
    }

    public void SetupWeapon() {
        currentWeapon = secondaryWeapon;
        
        GameObject currentWeaponObject = gameObject.transform.GetChild(0).gameObject;
        
        if(owner.isLocalPlayer) {
            weaponEquipmentUtility.SetLayerRecursion(currentWeaponObject, "Gun");
        }else{
            weaponEquipmentUtility.SetLayerRecursion(currentWeaponObject, "Default");
        }

        SetHoldingWeaponObject(currentWeaponObject);
        SetCurrentWeaponAnimation(currentWeaponObject);
        SetCurrentWeaponParticle(currentWeaponObject);
        owner.SetWeaponNetworkTransform();
    }

    public WeaponInfo GetCurrentWeapon() {
        return currentWeapon;
    }

    public GameObject GetHoldingWeapon() {
        return holdingWeaponObject;
    }

    public void SetHoldingWeaponObject(GameObject weaponObject) {
        holdingWeaponObject = weaponObject;
    }

    public void SetCurrentWeaponObject(GameObject weaponObject) {
        currentWeapon.SetWeaponObject(weaponObject);
    }

    public void SetCurrentWeaponAnimation(GameObject weaponObject) {
        currentWeapon.SetWeaponAnimation(weaponObject);
    }
    
    public void SetCurrentWeaponParticle(GameObject weaponObject) {
        currentWeapon.SetWeaponParticle(weaponObject);
    }

    public void PlayWeaponShootAnimation() {
        if(currentWeapon.shootAnimation){
            currentWeapon.shootAnimation.Play();
        }
    }

    public void PlayWeaponParticle() {
        currentWeapon.weaponParticle.Play();
    }

    public void StopWeaponParticle() {
        currentWeapon.weaponParticle.Stop();
    }
    
    public void PlayShootEffect() {
        PlayWeaponParticle();
        float effectDuration = currentWeapon.bulletRate * 0.1f;
        StartCoroutine(StopShootEffect(effectDuration));
    }

    public void PlayHitEffect(Vector3 position, Vector3 rotation, Transform parent) {
        if(parent){
            GameObject hitParticleObject = Instantiate(currentWeapon.hitParticle.gameObject, position, Quaternion.FromToRotation(currentWeapon.hitParticle.gameObject.transform.up, rotation * -1f), parent);
            if(parent.gameObject.CompareTag("NetworkPlayer") || parent.gameObject.CompareTag("LocalPlayer")) {
                Player parentProgramming = parent.gameObject.GetComponent<Player>();
                Transform parentMainBone = parentProgramming.GetPlayerLook().GetComponent<PlayerLook>().GetMainBoneObject().transform;
                hitParticleObject.transform.parent = parentMainBone;
            }
        }else{
            Instantiate(currentWeapon.hitParticle.gameObject, position, Quaternion.FromToRotation(-currentWeapon.hitParticle.gameObject.transform.up, rotation));
        }
    }

    IEnumerator StopShootEffect(float duration) {
        yield return new WaitForSeconds(duration);
        StopWeaponParticle();
    }
}