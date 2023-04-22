using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider))]
public class Holdable : MonoBehaviour {
    public float holdWidth;
    public float pickUpTime = 0.5f;

    private float heldState = 0.0f; // 0 if not held, 1 if held
    private Transform playerHoldTransform;
    Collider physicsCollider;
    Rigidbody myRigidbody;
    Vector3 oldPosition;

    public bool IsHeld {
        get => this.transform.parent == playerHoldTransform;
        private set {
            if (value) {
                this.transform.SetParent(playerHoldTransform);
            } else {
                this.transform.SetParent(null);
            }
        }
    }

    public bool IsFree {
        get {
            return this.transform.parent != playerHoldTransform;
        }
    }

    void Start(){
        playerHoldTransform = GameObject.FindWithTag("Player").transform.Find("HoldLocation");
        physicsCollider = GetComponent<Collider>();
        myRigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update(){
        if (IsHeld && heldState < 1f) {
            heldState += Time.deltaTime / pickUpTime;
            heldState = Mathf.Min(heldState, 1f);

            Vector3 newPosition = playerHoldTransform.position;
            this.transform.position =
                Vector3.Lerp(oldPosition, newPosition, QuadInterpolate(heldState));
        } else if (!IsHeld && heldState > 0f) {
            heldState -= Time.deltaTime / pickUpTime;
            heldState = Mathf.Max(heldState, 0f);
        }
    }

    public void Drop() {
        IsHeld = false;
    }

    public void Hold() {
        IsHeld = true;
        oldPosition = this.transform.position;
        this.transform.rotation = playerHoldTransform.rotation;
        playerHoldTransform.parent.GetComponent<HoldObject>().OnHoldObject(gameObject);
    }

    public float GetColliderWidth() {
        return holdWidth;
    }

    private float QuadInterpolate(float x) {
        return -x * (x - 2);
    }

}

