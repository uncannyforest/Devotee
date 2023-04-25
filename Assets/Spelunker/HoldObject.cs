using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HoldObject : MonoBehaviour
{
    public float transitionTime = 1f;

    private Transform playerHoldTransform;

    EnvironmentInteractor environmentInteractor;
    HoldAnimationControl holdAnimationControl;

    public bool IsHolding {
        get => this.playerHoldTransform.childCount > 0;
    }

    // Start is called before the first frame update
    void Start() {
        playerHoldTransform = gameObject.transform.Find("HoldLocation");

        environmentInteractor = new EnvironmentInteractor(this, playerHoldTransform);
        holdAnimationControl = new HoldAnimationControl(this, playerHoldTransform);
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetButtonDown("Interact")) {
            Interact();
        }
    }

    void OnTriggerEnter(Collider other) {
        if(other.tag == "CanPickUp") {
            environmentInteractor.AddInteractableObject(other.gameObject);
        } else if(other.tag == "DropZone") {
            environmentInteractor.readyToDrop = true;
        }
    }

	void OnTriggerExit(Collider other) {
        if(other.tag == "CanPickUp") {
            environmentInteractor.RemoveInteractableObject(other.gameObject);
		} else if(other.tag == "DropZone") {
            environmentInteractor.readyToDrop = false;
        }
    }

    /// <summary> Called by Holdable script once hold initiated </summary>
    public void OnHoldObject(GameObject heldObject) {
        holdAnimationControl.heldObjectWidth = heldObject.GetComponent<Holdable>().GetColliderWidth();
    }

    void Interact() {
        if (IsHolding) {
            environmentInteractor.DropHeldObject();
        } else {
            environmentInteractor.HoldClosestObject();
        }
    }

    void OnAnimatorIK() {
        holdAnimationControl.AnimatorIK();
    }
    
}
