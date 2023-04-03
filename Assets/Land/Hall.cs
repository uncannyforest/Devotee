using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hall : MonoBehaviour {
    public GameObject inner1;
    public GameObject inner2;
    public GameObject outer1;
    public GameObject outer2;
    public Material oneWayOpen;
    public Material oneWayClosed;

    void Start() {
        int version = Random.Range(0, 4);
        switch (version) {
            case 0: // all entrances open
                GameObject.Destroy(inner1);
                GameObject.Destroy(inner2);
                GameObject.Destroy(outer1);
                GameObject.Destroy(outer2);
                break;
            case 1: // all entrances closed
                GameObject.Destroy(inner1);
                GameObject.Destroy(inner2);
                break;
            case 2: // 3 entrances open
                GameObject.Destroy(inner2);
                GameObject.Destroy(outer2);
                break;
            case 3: // magic oneways
                GameObject.Destroy(outer1);
                inner1.GetComponent<MeshRenderer>().material = oneWayClosed;
                inner2.GetComponent<MeshRenderer>().material = oneWayOpen;
                GameObject.Destroy(inner2.GetComponent<Collider>());
                break;
        }
    }
}
