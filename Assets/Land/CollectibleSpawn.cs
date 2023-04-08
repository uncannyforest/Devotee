using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CollectibleSpawn : MonoBehaviour {
    public Collectible collectible;
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

    private Transform[] Possibilities {
        get {
            List<Transform> possibilites = new List<Transform>();
            foreach (Transform child in transform)
                if (child.gameObject.name == "Possibility")
                    possibilites.Add(child);
            return possibilites.ToArray();
        }
    }

    private Transform[] GetTriggers(Transform possibility) {
        List<Transform> triggers = new List<Transform>();
        foreach (Transform child in possibility)
            if (child.gameObject.name == "Trigger")
                triggers.Add(child);
        return triggers.ToArray();
    }

    void Start() {
        if (orientation == Orientation.Default && Vector3.Dot(transform.up, Vector3.down) > 0)
            gameObject.SetActive(false);
        if (orientation == Orientation.Flipped && Vector3.Dot(transform.up, Vector3.up) > 0)
            gameObject.SetActive(false);
        if (Possibilities.Length == 0) Spawn(transform);
    }

    public void UpdateTrigger() {
        if (Possibilities.Length == 0 || collected) return;
        Transform[] triggered = Possibilities.Where((p) =>
            !Physics.CheckSphere(p.position, radiusSpaceRequired, triggerLayerMask) &&
            GetTriggers(p).All((t) => Physics.Linecast(p.position, t.position, triggerLayerMask))).ToArray();
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
