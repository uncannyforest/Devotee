using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointArm : MonoBehaviour {
    public Transform pointTo;
    public float transitionTime = .5f;

    private float handWeightCurrent = 0;

    private Animator animator;
    // Start is called before the first frame update
    void Start() {
        animator = GetComponent<Animator>();
        
    }

    void OnAnimatorIK() {
        if(pointTo == null && handWeightCurrent == 0){
            return;
        }

        if (pointTo != null) {
            if (handWeightCurrent < 1f) {
                handWeightCurrent += Time.deltaTime / transitionTime;
            } else {
                handWeightCurrent = 1f;
            }
        } else {
            if (handWeightCurrent > 0f) {
                handWeightCurrent -= Time.deltaTime / transitionTime;
            } else { 
                handWeightCurrent = 0f;
            }
        }

        animator.SetIKPositionWeight(AvatarIKGoal.RightHand,handWeightCurrent);

        animator.SetIKPosition(AvatarIKGoal.RightHand, pointTo.position);
    }
}
