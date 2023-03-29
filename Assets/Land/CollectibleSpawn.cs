using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleSpawn : MonoBehaviour {
    public Collectible collectible;
    public float rate = 1;
    public float yMovement;

    void Start() {
        if (Random.value > rate) return;
        Collectible go = GameObject.Instantiate<Collectible>(collectible,
            transform.position, Quaternion.identity);
        go.transform.parent = transform;
        go.yMovement = yMovement;
    }
}
