using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CollectibleSpawn : MonoBehaviour {
    public Collectible collectible;
    public Transform[] possibilities; // must match triggers
    public Transform[] triggers;
    public LayerMask triggerLayerMask;
    public float radiusSpaceRequired = 0;
    public enum Orientation {
        Any,
        Default,
        Flipped
    }
    public Orientation orientation = Orientation.Any;
    public bool collected = false;

    private Collectible item;

    void Start() {
        if (orientation == Orientation.Default && Vector3.Dot(transform.up, Vector3.down) > 0)
            gameObject.SetActive(false);
        if (orientation == Orientation.Flipped && Vector3.Dot(transform.up, Vector3.up) > 0)
            gameObject.SetActive(false);
        if (triggers.Length == 0) Spawn(transform);
        if (triggers.Length == 1) possibilities = new Transform[] { transform };
    }

    public void UpdateTrigger() {
        if (triggers.Length == 0 || collected) return;
        Transform[] triggered = possibilities.Where((p, i) => Physics.Linecast(p.position, triggers[i].position, triggerLayerMask)
            && !Physics.CheckSphere(p.position, radiusSpaceRequired, triggerLayerMask)).ToArray();
        if (triggered.Length == 0) {
            if (item != null) GameObject.Destroy(item.gameObject);
        } else if (item == null) {
            SpawnRandom(triggered);
        } else if (!triggered.Contains(item.transform.parent)) {
            GameObject.Destroy(item.gameObject);
            SpawnRandom(triggered);
        }
    }

    public void SpawnRandom(Transform[] parentList) {
        Spawn(parentList[Random.Range(0, parentList.Length)]);
    }

    public void Spawn(Transform parent) {
        item = GameObject.Instantiate<Collectible>(collectible,
            parent.position, Quaternion.identity);
        item.transform.parent = parent;
        item.collectibleSpawn = this;
    }

    public static IEnumerator UpdateAllTriggers() {
        yield return new WaitForFixedUpdate();
        CollectibleSpawn[] spawns = GameObject.FindObjectsOfType<CollectibleSpawn>();
        foreach (CollectibleSpawn spawn in spawns) spawn.UpdateTrigger();
    }
}
