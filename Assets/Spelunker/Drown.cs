using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drown : MonoBehaviour {
    public float cycleRate = 2f;
    public Vector3 ikMax = new Vector3(.5f, .5f, 1f);
    public float scarfForce = 20;
    public bool isDrowning = false;

    private bool wasDrowning;

    public bool IsDrowning {
        set { isDrowning = value; }
    }

    private Animator animator;
    private Cloth scarf;
    void Start() {
        animator = GetComponent<Animator>();
        scarf = GetComponentInChildren<Cloth>();
    }

    void OnAnimatorIK() {
        if (!isDrowning && !wasDrowning) return;

        if (isDrowning ^ wasDrowning) {
            wasDrowning = isDrowning;
            scarf.externalAcceleration = Vector3.up * (isDrowning ? scarfForce : 0);
        }

        Vector3 rightPosition = new Vector3(ikMax.x, ikMax.y, ikMax.z * Mathf.Sin(Time.time * cycleRate));
        Vector3 leftPosition = Quaternion.Euler(0, 180, 0) * rightPosition;

        animator.SetIKPosition(AvatarIKGoal.LeftHand, transform.TransformPoint(leftPosition));
        animator.SetIKPosition(AvatarIKGoal.RightHand, transform.TransformPoint(rightPosition));
        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, isDrowning ? 1 : 0);
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, isDrowning ? 1 : 0);
    }
}
