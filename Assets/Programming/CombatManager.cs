using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CombatManager: NetworkBehaviour {

    void Start() {

    }

    [Command] public void CmdCombat(GameObject shooter, GameObject target, int damage) {
        Debug.Log("CmdCombat");
        Debug.Log("target: " + target);
        if(target){
            NetworkPlayer shooterPlayer = shooter.GetComponent<NetworkPlayer>();
            NetworkPlayer targetPlayer = target.GetComponent<NetworkPlayer>();
            if(targetPlayer != null){
                ClientTakeDamage(shooterPlayer, targetPlayer, damage);
            }
        }
        // RpcPlayShootEffect();
    }

    [Client] public void ClientTakeDamage(NetworkPlayer shootPlayer, NetworkPlayer hitPlayer, int damage) {
        Debug.Log("ClientTakeDamage");
        hitPlayer.RpcTakeDamage(shootPlayer.gameObject, damage);
    }

    [Command] public void CmdPlayShootEffect(GameObject shooter) {
        // Debug.Log("CmdPlayShootEffect");
        RpcPlayShootEffect(shooter);
    }

    [ClientRpc] public void RpcPlayShootEffect(GameObject shooter) {
        // Debug.Log("RpcPlayShootEffect");
        // Instantiate(gameObject, new Vector3(0,0,0),Quaternion.identity);
        Player shooterProgramming = shooter.GetComponent<Player>();
        WeaponEquipment shooterEquipment = shooterProgramming.GetWeaponEquipment();
        shooterEquipment.PlayShootEffect();
    }

    [Command] public void CmdPlayHitEffect(GameObject shooter, Vector3 position, Vector3 rotation, GameObject parent) {
        RpcPlayHitEffect(shooter, position, rotation, parent);
    }

    [ClientRpc] public void RpcPlayHitEffect(GameObject shooter, Vector3 position, Vector3 rotation, GameObject parent) {
        // Debug.Log("shooter: " + shooter);
        // Debug.Log("position: " + position + ", " + "rotation: " + rotation);
        // Debug.Log("parent: " + parent);

        Player shooterProgramming = shooter.GetComponent<Player>();
        // Debug.Log("shooterProgramming: " + shooterProgramming);
        
        WeaponEquipment shooterEquipment = shooterProgramming.GetWeaponEquipment();
        // Debug.Log("shooterEquipment: " + shooterEquipment);
        
        if(parent){
            shooterEquipment.PlayHitEffect(position, rotation, parent.transform);
        }else{
            shooterEquipment.PlayHitEffect(position, rotation, null);
        }
    }
}