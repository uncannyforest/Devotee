using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowEverywhere : MonoBehaviour {
    public float duration = 4;

    private int defaultLayer;

    void Start() {
        defaultLayer = gameObject.layer;
    }

    public void ActivateTemp() {
        CancelInvoke();
        SetLayer(transform, LayerMask.NameToLayer("UI"));
        Invoke("Deactivate", duration);
    }

    public void Deactivate() {
        SetLayer(transform, defaultLayer);
    }

    private void SetLayer(Transform t, int layer) {
        t.gameObject.layer = layer;
        foreach(Transform child in t) SetLayer(child, layer);
    }
}
