using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour {
    public int id;
    public CollectibleSpawn collectibleSpawn;
    public float speed = 1;

    void Update() {
        transform.Rotate(Vector3.up, Time.deltaTime * speed);
    }

    void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.tag == "Player") {
            collectibleSpawn.collected = true;
            CollectibleSpawnManager.I.Collect(this);
        }
    }
}
