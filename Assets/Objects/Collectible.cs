using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour {
    public float yMovement;
    public float movementDuration;

    void Update() {
        transform.localPosition = new Vector3(0,
            Mathf.Sin((2 * Mathf.PI) * Time.time / movementDuration) * yMovement, 0);
    }

    void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.tag == "Player") {
            GameObject.Destroy(gameObject);
        }
    }
}
