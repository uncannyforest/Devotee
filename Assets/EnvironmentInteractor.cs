using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary> holds code governing interaction with other game objects </summary>
public class EnvironmentInteractor {

    private HoldObject script;
    private Transform playerHoldTransform;
    private HashSet<GameObject> nearObjects = new HashSet<GameObject>();
    private FlexibleInputDisplay input;
    private bool readyToDrop = false;

    public bool ReadyToDrop {
        set {
            readyToDrop = value;
            if (value) input.SetInteractionMessage("give item");
            else UpdateMessageForHolding();
        }
    }

    public HashSet<GameObject> NearObjects {
        get => nearObjects;
    }

    public EnvironmentInteractor(HoldObject script, Transform playerHoldTransform) {
        this.script = script;
        this.playerHoldTransform = playerHoldTransform;
        input = GameObject.FindObjectOfType<FlexibleInputDisplay>();
    }

    public void AddInteractableObject(GameObject trigger) {
        nearObjects.Add(GetInteractableObject(trigger));
        input.SetInteractionMessage("pick up item");
    }

    public void RemoveInteractableObject(GameObject trigger) {
        nearObjects.Remove(GetInteractableObject(trigger));
        if (nearObjects.Count == 0) input.SetInteractionMessage(null);
    }

    private GameObject GetInteractableObject(GameObject trigger) {
        if(trigger.GetComponent<Holdable>() != null) {
            return trigger;
        } else if (trigger.transform.parent.GetComponent<Holdable>() != null) {
            return trigger.transform.parent.gameObject;
        } else {
            Debug.LogError("Object tagged CanPickUp has no Holdable script on it or parent");
            return null;
        }
    }

    public void HoldClosestObject() {
        if (nearObjects.Count == 0) {
            return;
        }

        GameObject closestObject = nearObjects.OrderBy(
                o => Vector3.Distance(o.transform.position, script.transform.position)
            ).First();

        input.SetInteractionMessage("release item");
        closestObject.GetComponent<Holdable>().Hold();
    }

    public void DropHeldObject() {
        foreach (Transform child in playerHoldTransform) {
            if (readyToDrop) {
                script.StartCoroutine(GameObject.FindObjectOfType<Altar>().Collect(child));
            } else {
                child.GetComponent<Holdable>().Drop();
            }
        }
        UpdateViewForNearbyObjects();
    }

    public void UpdateMessageForHolding() {
        if (script.IsHolding) {
            input.SetInteractionMessage("release item");
        } else {
            UpdateViewForNearbyObjects();
        }
    }
    public void UpdateViewForNearbyObjects() {
        if (nearObjects.Count > 0) {
            input.SetInteractionMessage("pick up item");
        } else {
            input.SetInteractionMessage(null);
        }
    }

}
