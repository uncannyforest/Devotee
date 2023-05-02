using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class Trigger : MonoBehaviour {
    public string tagToCheck;
    public UnityEvent onCollisionEnter;
    public UnityEvent onCollisionExit;
    
    void OnCollisionEnter(Collision other) {
        Debug.Log("Collided! " + other.collider.tag);
        if (other.collider.CompareTag(tagToCheck)) {
            onCollisionEnter.Invoke();
        }
    }
    void OnCollisionExit(Collision other) {
        Debug.Log("Uncollided! " + other.collider.tag);
        if (other.collider.CompareTag(tagToCheck)) {
            onCollisionExit.Invoke();
        }
    }
}
