using UnityEngine;

public static class UnityExtensions {
    public static void SetLayerForAllChildren(this Transform root, int layer) {
        foreach (Transform child in root.GetComponentsInChildren<Transform>(includeInactive: true)) {
            child.gameObject.layer = layer;
        }
    }
}
