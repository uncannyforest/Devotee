using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpeningAt10 : MonoBehaviour {
    public GameObject door;
    public GameObject passage;


    void Start() {
        if (transform.localScale.y == 10)
            GameObject.Destroy(door);
        else
            GameObject.Destroy(passage);
    }
}
