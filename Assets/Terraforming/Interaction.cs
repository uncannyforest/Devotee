using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ShowEverywhere))]
public class Interaction : MonoBehaviour {
    public Terraformer terraformer;
    public Camera uiCamera;
    public float scale = 48;
    public Color activeColor = Color.white;
    public Color inactiveColor = Color.white;

    private int numTools = 1;

    private int currentTool = 0;
    public int CurrentTool {
        get => currentTool;
        private set {
            if (value >= numTools) return;
            SetInactiveTool(transform.GetChild(currentTool).GetChild(0).GetComponent<Collectible>());
            transform.GetChild(currentTool).GetChild(0).GetComponent<Tool>().Unload();
            currentTool = value;
            transform.GetChild(value).GetChild(0).GetComponent<Collectible>().enabled = true;
            transform.GetChild(value).GetChild(0).GetComponent<Tool>().Load();
            transform.GetChild(value).GetChild(0).GetComponentInChildren<SpriteRenderer>().color = activeColor;
        }
    }

    private void SetInactiveTool(Collectible tool) {
        tool.enabled = false;
        tool.transform.localRotation = Quaternion.identity;
        tool.GetComponentInChildren<SpriteRenderer>().color = inactiveColor;
    }

    private void SetLayerForAllChildren(Transform root, int layer) {
        foreach (Transform child in root.GetComponentsInChildren<Transform>(includeInactive: true)) {
            child.gameObject.layer = layer;
        }
    }

    public void AddTool(Collectible tool) {
        tool.transform.parent = transform.GetChild(numTools);
        tool.transform.localPosition = Vector3.zero;
        tool.transform.localRotation = Quaternion.identity;
        tool.transform.localScale = Vector3.one * scale;
        SetLayerForAllChildren(tool.transform, LayerMask.NameToLayer("UI"));
        tool.GetComponentInChildren<Billboard>().lookCamera = uiCamera;
        tool.GetComponentInChildren<ParticleSystem>().gameObject.SetActive(false);
        SetInactiveTool(tool);
        numTools++;
    }

    void Update() {
        if (Input.GetKeyDown("1")) CurrentTool = 0;
        if (Input.GetKeyDown("2")) CurrentTool = 1;
        if (Input.GetKeyDown("3")) CurrentTool = 2;
        if (Input.GetKeyDown("4")) CurrentTool = 3;
        if (Input.GetKeyDown("5")) CurrentTool = 4;
        if (Input.GetKeyDown("6")) CurrentTool = 5;
        if (Input.GetKeyDown("7")) CurrentTool = 6;
        if (Input.GetKeyDown("8")) CurrentTool = 7;
        if (Input.GetKeyDown("9")) CurrentTool = 8;
        if (Input.GetKeyDown("0")) CurrentTool = 9;
    }

    public void UpdatePos(HexPos pos) {
        transform.GetChild(currentTool).GetChild(0).GetComponent<Tool>().UpdatePos(pos);
    }

    public bool Use() {
        bool used = transform.GetChild(currentTool).GetChild(0).GetComponent<Tool>().Use();
        if (used) StartCoroutine(CollectibleSpawn.UpdateAllTriggers());
        return used;
    }
}
