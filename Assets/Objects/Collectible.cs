using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour {
    public CollectibleSpawn collectibleSpawn;

    void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.tag == "Player") {
            collectibleSpawn.collected = true;
            GameObject.Destroy(gameObject);
        }
    }
}
