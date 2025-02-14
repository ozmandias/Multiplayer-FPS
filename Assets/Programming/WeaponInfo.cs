using UnityEngine;

[System.Serializable] public class WeaponInfo {
    public string weaponName = "Weapon";
    public int damage = 0;
    public float shootDistance = 0f;
    public float bulletRate = 0f;
    public GameObject weaponObject;
    // public Animator weaponAnimator;
    public Animation shootAnimation;
    public ParticleSystem weaponParticle;
    public ParticleSystem hitParticle;

    public void SetWeaponObject(GameObject newWeaponObject) {
        if(newWeaponObject) {
            weaponObject = newWeaponObject;
        }
    }

    public void SetWeaponAnimation(GameObject weaponSource) {
        if(weaponSource) {
            shootAnimation = weaponSource.GetComponent<Animation>();
        }
    }
    
    public void SetWeaponParticle(GameObject weaponParent) {
        if(weaponObject) {
            weaponParticle = weaponParent.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>();
        }
    }
}