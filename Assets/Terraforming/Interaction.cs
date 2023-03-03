using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction : MonoBehaviour {
    private int currentTool = 0;
    public int CurrentTool {
        get => currentTool;
        private set => currentTool = value;
    }

    void Update() {
        if (Input.GetKeyDown("1")) CurrentTool = 0;
        if (Input.GetKeyDown("2")) CurrentTool = 1;
    }

    public bool Use(HexPos pos) {
        return transform.GetChild(currentTool).GetComponent<Tool>().Use(pos);
    }
}
