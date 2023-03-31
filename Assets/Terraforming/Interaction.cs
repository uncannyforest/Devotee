using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction : MonoBehaviour {
    public Terraformer terraformer;

    private int currentTool = 0;
    public int CurrentTool {
        get => currentTool;
        private set {
            transform.GetChild(currentTool).GetComponent<Tool>().Unload();
            currentTool = value;
            transform.GetChild(value).GetComponent<Tool>().Load();
        }
    }

    void Update() {
        if (Input.GetKeyDown("1")) CurrentTool = 0;
        if (Input.GetKeyDown("2")) CurrentTool = 1;
        if (Input.GetKeyDown("3")) CurrentTool = 2;
        if (Input.GetKeyDown("4")) CurrentTool = 3;
    }

    public void WillUpdatePos(HexPos pos) {
        transform.GetChild(currentTool).GetComponent<Tool>().WillUpdatePos(pos);
    }

    public bool Use() {
        return transform.GetChild(currentTool).GetComponent<Tool>().Use();
    }
}
