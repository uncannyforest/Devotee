using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Altar : MonoBehaviour {
    public float collectTime = 1f;
    public float acceleration = 3f;
    public float yAcceleration = 1f;

    public IEnumerator Collect(Transform child) {
        float ddy = 0;
        float dy = 0;
        float stopTime = Time.time + collectTime;
        child.SetParent(null);
        while (Time.time < stopTime) {
            ddy += yAcceleration * (Time.deltaTime * Time.deltaTime * Time.deltaTime);
            dy += ddy;
            child.position = 
                new Vector3(Mathf.Lerp(child.position.x, transform.position.x, acceleration * Time.deltaTime),
                    child.position.y + dy,
                    Mathf.Lerp(child.position.z, transform.position.z, acceleration * Time.deltaTime));
            yield return null;
        }
        CollectibleSpawnManager.I.Collect(child.GetComponent<Collectible>());
    }
}
