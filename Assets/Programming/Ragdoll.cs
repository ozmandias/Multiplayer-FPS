using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Ragdoll : NetworkBehaviour {
    [Header("Main")]
    [SerializeField] Animator mainAnimator;
    [SerializeField] Rigidbody mainRigidbody;
    [SerializeField] Collider[] mainColliders;

    [Header("Ragdoll Parts")]
    [SerializeField] Rigidbody[] ragdollBodies;
    [SerializeField] Collider[] ragdollColliders;

    [Header("Transform Reset")]
    [SerializeField] GameObject[] transformChangeObjects;
    [SerializeField] List<Vector3> defaultPositions = new List<Vector3>();
    [SerializeField] List<Quaternion> defaultRotations = new List<Quaternion>();
    [SerializeField] List<Vector3> defaultScales = new List<Vector3>();

    void Start() {
        SetDefaultTransforms();
        // CollectRagdollBodies();
        // CollectRagdollColliders();
    }

    void CollectRagdollBodies() {
        ragdollBodies = GetComponentsInChildren<Rigidbody>();
    }

    void CollectRagdollColliders() {
        ragdollColliders = GetComponentsInChildren<Collider>();
    }

    void SetDefaultTransforms() {
        if(transformChangeObjects.Length > 0) {
            foreach(GameObject transformChangeObject in transformChangeObjects) {
                defaultPositions.Add(transformChangeObject.transform.localPosition);
                defaultRotations.Add(transformChangeObject.transform.localRotation);
                defaultScales.Add(transformChangeObject.transform.localScale);
            }
        }
    }

    void ApplyDefaultTransforms() {
        if(transformChangeObjects.Length > 0) {
            for(int i = 0; i < transformChangeObjects.Length; i = i + 1) {
                transformChangeObjects[i].transform.localPosition = defaultPositions[i];
                transformChangeObjects[i].transform.localRotation = defaultRotations[i];
                transformChangeObjects[i].transform.localScale = defaultScales[i];

            }
        }
    }

    public void EnableRagdoll() {
        if(mainAnimator != null) {
            mainAnimator.enabled = false;
        }

        if(mainRigidbody != null) {
            mainRigidbody.isKinematic = true;
        }

        if(mainColliders.Length > 0) {
            foreach(Collider mainCollider in mainColliders) {
                mainCollider.enabled = false;
            }
        }

        if(ragdollBodies.Length > 0) {
            foreach(Rigidbody ragdollBody in ragdollBodies) {
                ragdollBody.isKinematic = false;
            }
        }

        if(ragdollColliders.Length > 0) {
            foreach(Collider ragdollCollider in ragdollColliders) {
                ragdollCollider.enabled = true;
            }
        }
    }

    public void DisableRagdoll() {
        if(ragdollBodies.Length > 0) {
            foreach(Rigidbody ragdollBody in ragdollBodies) {
                ragdollBody.isKinematic = true;
            }
        }

        if(ragdollColliders.Length > 0) {
            foreach(Collider ragdollCollider in ragdollColliders) {
                ragdollCollider.enabled = false;
            }
        }

        ApplyDefaultTransforms();

        if(mainRigidbody != null) {
            mainRigidbody.isKinematic = false;
        }

        if(mainColliders.Length > 0) {
            foreach(Collider mainCollider in mainColliders) {
                mainCollider.enabled = true;
            }
        }

        if(mainAnimator != null) {
            mainAnimator.enabled = true;
        }
    }

    [Command] public void CmdRagdoll(bool ragdollSwitch) {
        RpcRagdoll(ragdollSwitch);
    }

    [ClientRpc] public void RpcRagdoll(bool ragdollSwitch) {
        if(ragdollSwitch == true) {
            EnableRagdoll();
        } else {
            DisableRagdoll();
        }
    }
}