using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour {
    public Camera lookCamera;

    void LateUpdate() {
        transform.LookAt(lookCamera == null ? Camera.main.transform : lookCamera.transform);
        transform.Rotate(0, 180, 0);
    }
}
