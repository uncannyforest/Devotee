using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Land))]
public class Hall : MonoBehaviour {
    public Material oneWayOpen;
    public Material oneWayClosed;

    void Start() {
        Transform inner = transform.Find("Inner");
        Transform outer = transform.Find("Outer");

        Land land = GetComponent<Land>();
        int version = GetComponent<Land>().RandomRange(4);
        switch (version) {
            case 0: // all entrances open
                foreach (Transform child in inner) GameObject.Destroy(child.gameObject);
                foreach (Transform child in outer) GameObject.Destroy(child.gameObject);
                land.ground = new MeshRenderer[] {land.ground[0]};
                break;
            case 1: // 1 entrance open
                GameObject.Destroy(inner.GetChild(0).gameObject);
                GameObject.Destroy(outer.GetChild(0).gameObject);
                RegisterAllChildren();
                break;
            case 2: // 3 entrances open
                GameObject.Destroy(inner.GetChild(0).gameObject);
                GameObject.Destroy(inner.GetChild(2).gameObject);
                GameObject.Destroy(inner.GetChild(4).gameObject);
                GameObject.Destroy(outer.GetChild(0).gameObject);
                GameObject.Destroy(outer.GetChild(2).gameObject);
                GameObject.Destroy(outer.GetChild(4).gameObject);
                RegisterAllChildren();
                break;
            case 3: // magic oneways
                List<MeshRenderer> groundRegistry = new List<MeshRenderer>(land.ground);
                for (int i = 0; i < 6; i++) {
                    if (i % 2 == 0) {
                        GameObject.Destroy(outer.GetChild(i).gameObject);
                        inner.GetChild(i).GetComponent<MeshRenderer>().material = oneWayClosed;
                    } else {
                        inner.GetChild(i).gameObject.layer = LayerMask.NameToLayer("Let Player Through");
                        inner.GetChild(i).GetComponent<MeshRenderer>().material = oneWayOpen;
                        groundRegistry.Add(outer.GetChild(i).GetComponent<MeshRenderer>());
                    }
                }
                land.ground = groundRegistry.ToArray();
                break;
        }
    }

    private void RegisterAllChildren() {
        Land land = GetComponent<Land>();
        List<MeshRenderer> groundRegistry = new List<MeshRenderer> {land.ground[0]};
        foreach (Transform child in transform.Find("Inner"))
            groundRegistry.Add(child.GetComponent<MeshRenderer>());
        foreach (Transform child in transform.Find("Outer"))
            groundRegistry.Add(child.GetComponent<MeshRenderer>());
        land.ground = groundRegistry.ToArray();
    }
}
