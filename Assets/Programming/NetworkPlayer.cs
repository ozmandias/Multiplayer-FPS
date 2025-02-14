using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkPlayer : NetworkBehaviour {
    Player playerProgramming;
    PlayerSetup playerSetup;
    [SerializeField]NetworkIdentity playerIdentity;
    [SerializeField] Camera networkPlayerCamera;
    int max_health = 100;
    [SyncVar]public int current_health = 0;
    [SyncVar]public int kills = 0;
    [SyncVar]public int deaths = 0;
    Utility networkUtility;
    [SerializeField]NetworkTransformChild weaponNetworkTransform;
    [SerializeField]GameObject[] toLayerChangeOnDeath;
    [SerializeField]Behaviour[] toDisabeOnDeath;
    Ragdoll playerRagdoll;
    [SyncVar]bool isDead = false;

    void Start() {
        playerProgramming = gameObject.GetComponent<Player>();
        playerSetup = gameObject.GetComponent<PlayerSetup>();
        if(playerIdentity == null) {
            playerIdentity = GetComponent<NetworkIdentity>();
        }
        networkUtility = Utility.instance;
        playerRagdoll = GetComponent<Ragdoll>();

        SetDefaults();

        playerProgramming.OnHitLimitCallback = playerProgramming.OnHitLimitCallback + CmdKillPlayer;
    }

    void Update() {
        if(isLocalPlayer){
            NetworkMove();
            NetworkShoot();
            NetworkKillPlayer();
        }
    }

    void SetDefaults() {
        current_health = max_health;
        if(isLocalPlayer){
            networkUtility.SetLayerToGameObject(gameObject, "LocalPlayer");
            networkUtility.SetLayerToGameObjects(toLayerChangeOnDeath, "Gun");
            networkUtility.EnableComponents(toDisabeOnDeath, true);
        }else{
            networkUtility.SetLayerToGameObject(gameObject, "NetworkPlayer");
        }
    }

    [Client] void NetworkMove() {
        playerProgramming.Move();
    }

    [Client] void NetworkShoot() {
        playerProgramming.Shoot();
    }

    [Client] void NetworkKillPlayer() {
        if(Input.GetKeyDown(KeyCode.K)) {
            CmdKillPlayer();
        }
    }

    [Client] void PlayAllAnimations() {
        playerRagdoll.CmdRagdoll(false);
    }

    [Client] void StopAllAnimations() {
        playerProgramming.StopRunAnimation();
        playerRagdoll.CmdRagdoll(true);
        // playerProgramming.PlayDeathAnimation();
    }

    [ClientRpc]
    public void RpcTakeDamage(GameObject shooter, int incomingDamage) {
        Debug.Log("RpcTakeDamage");
        if(isDead == false) {
            current_health = current_health - incomingDamage;

            if(current_health <= 0) { 
                Death();

                string shooterName = "";
                if(shooter) {
                    shooterName = shooter.name;
                    NetworkPlayer shooterPlayer = shooter.GetComponent<NetworkPlayer>();
                    shooterPlayer.kills += 1; 
                    playerSetup.GetUI().UpdateScoreboard(shooterPlayer);
                }
                playerSetup.GetUI().AddKillFeedNotification(shooterName, gameObject.name);
            } else if(isLocalPlayer) {
                playerSetup.GetUI().ToggleHitPanel();
                playerSetup.GetUI().UpdateHealthBar(incomingDamage);
            }
        }
    }

    [Command] public void CmdKillPlayer() {
        RpcTakeDamage(null, max_health);
    }

    void Death() {
        Debug.Log("Death");
        isDead = true;
        this.deaths += 1;
        playerSetup.GetUI().UpdateScoreboard(this);
        StopAllAnimations();
        playerProgramming.GetWeaponEquipment().UnequipWeapon();
        if(isLocalPlayer){
            playerSetup.GetUI().ToggleHitPanel();
            // playerSetup.GetUI().ShowHitPanel();
            playerSetup.GetUI().UpdateHealthBar(max_health);
            networkUtility.SetLayerToGameObjects(toLayerChangeOnDeath, "Default");
            networkUtility.EnableComponents(toDisabeOnDeath, false);
        }
        Bleed();
        StartCoroutine(DeathCoroutine());
    }

    void Respawn() {
        Debug.Log("Respawn");
        isDead = false;
        PlayAllAnimations();
        playerProgramming.GetWeaponEquipment().EquipWeapon();
        if(isLocalPlayer){
            playerSetup.GetUI().HideHitPanel();
            playerSetup.GetUI().ResetHealthBar();
            Transform networkSpawnPosition = NetworkManager.singleton.GetStartPosition();
            gameObject.transform.position = networkSpawnPosition.position;
            gameObject.transform.rotation = networkSpawnPosition.rotation;
        }
        SetDefaults();
    }

    public void Bleed() {
        GameObject playerBloodObject = Instantiate(playerSetup.GetBlood(), transform);
        ParticleSystem playerBlood = playerBloodObject.GetComponent<ParticleSystem>();
        playerBlood.Play();
        StartCoroutine(BleedCoroutine(playerBlood));
    }

    public string GetPlayerID() {
        return "" + playerIdentity.netId;
    }

    public PlayerSetup GetPlayerSetup() {
        return playerSetup;
    }

    public Camera GetNetworkPlayerCamera() {
        return networkPlayerCamera;
    }
    
    [Client]public void SetWeaponNetworkTransform() {
        CmdSetWeaponNetworkTransform();
    }

    [Command]public void CmdSetWeaponNetworkTransform () {
        RpcSetWeaponNetworkTransform();
    }
    [ClientRpc]public void RpcSetWeaponNetworkTransform() {
        GameObject currentWeapon = playerProgramming.GetWeaponEquipment().GetHoldingWeapon();
        // Debug.Log("currentWeapon: " + currentWeapon);
        weaponNetworkTransform.target = currentWeapon.transform;
    }

    IEnumerator DeathCoroutine() {
        yield return new WaitForSeconds(5f);
        Respawn();
    }

    IEnumerator BleedCoroutine(ParticleSystem bloodParticle) {
        yield return new WaitForSeconds(3f);
        bloodParticle.Stop();
        Destroy(bloodParticle.gameObject);
    }
}