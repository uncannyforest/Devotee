using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hall : MonoBehaviour {
    public Material oneWayOpen;
    public Material oneWayClosed;

    void Start() {
        Transform inner = transform.Find("Inner");
        Transform outer = transform.Find("Outer");

        int version = GetComponent<Land>().RandomRange(4);
        switch (version) {
            case 0: // all entrances open
                foreach (Transform child in inner) GameObject.Destroy(child.gameObject);
                foreach (Transform child in outer) GameObject.Destroy(child.gameObject);
                break;
            case 1: // 1 entrance open
                GameObject.Destroy(inner.GetChild(0).gameObject);
                GameObject.Destroy(outer.GetChild(0).gameObject);
                break;
            case 2: // 3 entrances open
                GameObject.Destroy(inner.GetChild(0).gameObject);
                GameObject.Destroy(inner.GetChild(2).gameObject);
                GameObject.Destroy(inner.GetChild(4).gameObject);
                GameObject.Destroy(outer.GetChild(0).gameObject);
                GameObject.Destroy(outer.GetChild(2).gameObject);
                GameObject.Destroy(outer.GetChild(4).gameObject);
                break;
            case 3: // magic oneways
                for (int i = 0; i < 6; i++) {
                    if (i % 2 == 0) {
                        GameObject.Destroy(outer.GetChild(i).gameObject);
                        inner.GetChild(i).GetComponent<MeshRenderer>().material = oneWayClosed;
                    } else {
                        inner.GetChild(i).gameObject.layer = LayerMask.NameToLayer("Let Player Through");
                        inner.GetChild(i).GetComponent<MeshRenderer>().material = oneWayOpen;
                    }
                }
                break;
        }
    }
}
