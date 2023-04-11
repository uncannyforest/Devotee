using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleSpawnManager : MonoBehaviour {
    private static CollectibleSpawnManager instance;
    public static CollectibleSpawnManager I { get => instance; }
    CollectibleSpawnManager(): base() {
        instance = this;
    }

    public Collectible[] collectibles;
    private bool[] collected;

    private List<HashSet<Collectible>> collectiblesInWorld;
    private Interaction interaction;

    void Start() {
        interaction = GameObject.FindObjectOfType<Interaction>();
        collected = new bool[collectibles.Length];
        collected[0] = true;
        collectiblesInWorld = new List<HashSet<Collectible>>();
        for (int i = 0; i < collectibles.Length; i++)
            collectiblesInWorld.Add(new HashSet<Collectible>());
    }

    public void Collect(Collectible item) {
        collected[item.id] = true;
        collectiblesInWorld[0].Remove(item);
        interaction.AddTool(item);
        foreach(Collectible collectible in collectiblesInWorld[item.id]) {
            CollectibleSpawn spawn = collectible.collectibleSpawn;
            Transform parent = item.transform.parent;
            Despawn(item);
            spawn.Spawn(parent);
        }
    }

    public void Uncollect(int id) {
        collected[id] = false;
    }

    public Collectible Spawn(Transform parent) {
        int spawnId = 0;
        for (int i = 0; i < collectibles.Length; i++) {
            if (!collected[i]) {
                spawnId = i;
                if (Random.value < .5f) break;
            }
        }

        Collectible item = GameObject.Instantiate<Collectible>(collectibles[spawnId],
            parent.position, Quaternion.identity);
        item.transform.parent = parent;
        collectiblesInWorld[0].Add(item);
        return item;
    }

    public void Despawn(Collectible item) {
        collectiblesInWorld[0].Remove(item);
        GameObject.Destroy(item.gameObject);
    }
}
