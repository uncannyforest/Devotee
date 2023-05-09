using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlexibleInputDisplay : MonoBehaviour {
    public GameObject largeNotice;
    public GameObject interactNotice1;

    public void UpdateForHeldObject(GameObject heldObject) {
        SetLayerPerspective().SetInteractionMessage("release");
    }

    public FlexibleInputDisplay SetLayerPerspective() {
        gameObject.layer = LayerMask.NameToLayer("Perspective UI Only");
        return this;
    }
    public FlexibleInputDisplay SetLayerOrthographic() {
        gameObject.layer = LayerMask.NameToLayer("Orthographic UI Only");
        return this;
    }
    public FlexibleInputDisplay SetLayerBoth() {
        gameObject.layer = LayerMask.NameToLayer("UI");
        return this;
    }

    public void SetLargeMessage(string message) {
        if (!string.IsNullOrEmpty(message)) {
            largeNotice.SetActive(true);
            largeNotice.transform.Find("Text").gameObject.GetComponent<Text>().text = message;
        } else {
            largeNotice.SetActive(false);
        }
    }

    public void SetInteractionMessage(string interact1) {
        SetInteractionMessage(interactNotice1, interact1, "x");
    }

    private void SetInteractionMessage(GameObject interactNotice, string message, string desktopKey) {
        if (!string.IsNullOrEmpty(message)) {
            interactNotice.SetActive(true);

        message = "press <color=white>" + desktopKey + "</color> to <color=white>"
            + message + "</color>";


            interactNotice.transform.Find("Text").gameObject.GetComponent<Text>().text = message;
        } else {
            interactNotice.SetActive(false);
        }
    }
}
