using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Land : MonoBehaviour {
    public int id;
    public float randomSeed = -1;
    public MeshRenderer[] ground;

    private float randomNext;

    void Awake() {
        if (randomSeed < 0) randomSeed = Random.value;
        randomNext = randomSeed;
    }

    public int RandomRange(int n) {
        float result = randomNext * n;
        randomNext = result % 1;
        return Mathf.FloorToInt(result);
    }
}
