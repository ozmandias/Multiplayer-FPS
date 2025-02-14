using UnityEngine;

public class Utility: MonoBehaviour {
    
    #region Singleton
        public static Utility instance;

        public void Awake() {
            if(instance != null) {
                return;
            }
            instance = this;
        }
    #endregion

    public void SetLayerToGameObject(GameObject _object, string layerName) {
        int layer = LayerMask.NameToLayer(layerName);
        _object.layer = layer;
    }
    
    public void SetLayerToGameObjects(GameObject[] objects, string layerName) {
        int layer = LayerMask.NameToLayer(layerName);
        if(layer != -1) {
            foreach(GameObject _object in objects){
                _object.layer = layer;
            }
        }
    }

    public void SetLayerRecursion(GameObject parentObject, string layerName) {
        int layer = LayerMask.NameToLayer(layerName);
        parentObject.layer = layer;

        foreach(Transform child in parentObject.transform){
            GameObject _object = child.gameObject;
            SetLayerRecursion(_object, layerName);
        }
    }

    public void EnableComponents(Behaviour[] components, bool status) {
        if(components.Length > 0) {
            foreach(Behaviour component in components) {
                component.enabled = status;
            }
        }
    }
}