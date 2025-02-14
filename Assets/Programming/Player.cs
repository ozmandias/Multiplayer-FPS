using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : MonoBehaviour {
	[SerializeField]Animator playerAnimator;
	[SerializeField]GameObject playerLook;
	[SerializeField]WeaponEquipment playerWeaponEquipment;
	[SerializeField]Camera playerCamera;
	[SerializeField]Rigidbody playerBody;
	[SerializeField]CombatManager playerCombat;
	[SerializeField]GameObject playerGroundCheck;
	float speed = 8f;
	float jumpForce = 4f;
	float gravity = 8f;
	bool collideWithWall = false;
	bool atGround = false;

	public delegate void OnHitLimit();
	public OnHitLimit OnHitLimitCallback;

	// Use this for initialization
	void Start () {
		if(playerLook==null){
			playerLook = GameObject.Find("PlayerLook");
		}
		// playerCamera = playerLook.transform.GetChild(0).gameObject.GetComponent<Camera>();
		playerCombat = gameObject.GetComponent<CombatManager>();
	}
	
	// Update is called once per frame
	void Update () {
		// Move();
		// Look();
		// Shoot();
	}

	void FixedUpdate() {
		CalculatePhysics();
		CheckGround();
	}

	public void Move() {
		float horizontal = Input.GetAxis("Horizontal");
		float vertical = Input.GetAxis("Vertical");

		// gameObject.transform.forward = playerLook.transform.forward;
		gameObject.transform.forward = playerLook.GetComponent<PlayerLook>().GetMainBoneObject().transform.forward;

		Vector3 direction = new Vector3(horizontal, 0, vertical) * speed * Time.deltaTime;
		direction = gameObject.transform.TransformDirection(direction);
		
		gameObject.transform.localEulerAngles = new Vector3(0, transform.forward.y, 0);

		if(Input.GetKeyDown(KeyCode.Space) && atGround == true){
			// direction = new Vector3(direction.x, direction.y + jumpForce * Time.deltaTime, direction.z);
			direction.y += jumpForce;
			atGround = false;
		}else if(atGround == false){
			direction.y -= gravity * Time.deltaTime;
		}

		// direction.y = direction.y - gravity * Time.deltaTime;
		gameObject.transform.position = gameObject.transform.position + direction;
		
		if((horizontal <= 1 && horizontal > 0) || (horizontal >= -1 && horizontal < 0) || (vertical <= 1 && vertical > 0) || (vertical >= -1 && vertical < 0)){
			PlayRunAnimation();
		}else{
			StopRunAnimation();
		}
	}

	public void Look() {
		float mouseHorizontal = Input.GetAxis("Mouse X");
		
		Vector3 mouseDirection = new Vector3(0, mouseHorizontal, 0);

		gameObject.transform.localEulerAngles = mouseDirection;
	}

	public void Shoot() {
		if(Input.GetKeyDown(KeyCode.Mouse0)){
			Debug.Log("Shoot");
			Vector3 screenCenterPosition = new Vector3(0.5f, 0.5f, 0);
			Ray shootRay = playerCamera.ViewportPointToRay(screenCenterPosition);
			RaycastHit shootRayHit;
			int playerLayerMask = 1 << LayerMask.NameToLayer("LocalPlayer");
			playerLayerMask = ~playerLayerMask;

			WeaponInfo playerWeapon = playerWeaponEquipment.GetCurrentWeapon();
			float rayDistance = playerWeapon.shootDistance;

			playerWeaponEquipment.PlayWeaponShootAnimation();
			// playerWeaponEquipment.PlayShootEffect();
			playerCombat.CmdPlayShootEffect(gameObject);
			// playerCombat.RpcPlayShootEffect(gameObject);

			// Debug.DrawRay(shootRay.origin, playerLook.transform.forward * rayDistance);
			if(Physics.Raycast(shootRay, out shootRayHit, rayDistance, playerLayerMask)){
				GameObject hitObject = shootRayHit.transform.gameObject;
				Debug.Log("Hit: " + hitObject.name);

				// playerWeaponEquipment.PlayHitEffect(shootRayHit.point, shootRayHit.normal, hitObject.transform);
				playerCombat.CmdPlayHitEffect(gameObject, shootRayHit.point, shootRayHit.normal, hitObject);
				
				if(hitObject.CompareTag("NetworkPlayer")){
					int weaponDamage = playerWeapon.damage;
					playerCombat.CmdCombat(gameObject, hitObject, weaponDamage);
				}
			}
		}
	}

	public void PlayRunAnimation() {
		playerAnimator.SetBool("PlayerRun", true);
	}

	public void StopRunAnimation() {
		playerAnimator.SetBool("PlayerRun", false);
	}

	public void PlayDeathAnimation() {
		playerAnimator.SetTrigger("PlayerDeath");
	}

	public WeaponEquipment GetWeaponEquipment() {
		return playerWeaponEquipment;
	}

	public GameObject GetPlayerLook() {
		return playerLook;
	}

	public Camera GetPlayerCamera() {
		return playerCamera;
	}

	void CalculatePhysics() {
		if(collideWithWall) {
			playerBody.velocity = Vector3.zero;
			// collideWithWall = false;
		}
		// Debug.Log("atGround: " + atGround);
		// Debug.Log("collideWithWall: " + collideWithWall);
	}

	void CheckGround() {
		// if(atGround == false) {
			Ray groundRay = new Ray(playerGroundCheck.transform.position, Vector3.down);
			RaycastHit groundRayHit;
			float groundCheckDistance = 0.5f;
			int PlayerLayerMask = 1 << 10;
			PlayerLayerMask = ~PlayerLayerMask;

			Debug.DrawRay(groundRay.origin, groundRay.direction * groundCheckDistance, Color.white);
			if(Physics.Raycast(groundRay, out groundRayHit, groundCheckDistance, PlayerLayerMask)){
				if(groundRayHit.collider.gameObject.CompareTag("Ground") || groundRayHit.collider.gameObject.CompareTag("Wall") || collideWithWall){
					atGround = true;
				}
			}
		// }
	}

	void OnCollisionEnter(Collision objectCollision) {
		if(objectCollision.collider.tag == "Wall") {
			collideWithWall = true;
			atGround = true;
		}
		// Debug.Log("Collide with Collider: " + objectCollision.collider.gameObject);
	}

	void OnCollisionExit(Collision objectCollision) {
		if(objectCollision.collider.tag == "Wall") {
			collideWithWall = false;
		}
		if(objectCollision.collider.tag == "Ground" && collideWithWall == false) {
			atGround = false;
		}
	}

	void OnTriggerEnter(Collider objectCollider) {
		if(objectCollider.gameObject.CompareTag("FallCollider")) {
			OnHitLimitCallback.Invoke();
		}
		// Debug.Log("Collide with Trigger: " + objectCollider.gameObject);
	}
}