using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class Drown : MonoBehaviour {
    public float lifeDrainTime = 2;
    public float cycleRate = 2f;
    public Vector3 ikMax = new Vector3(.5f, .5f, 1f);
    public float scarfForce = 20;
    public bool isDrowning = false;

    public bool IsDrowning {
        set {
            isDrowning = value;

            scarf.externalAcceleration = Vector3.up * (isDrowning ? scarfForce : 0);
            if (isDrowning) {
                control.ProhibitMove("drowning");
                StartCoroutine(LifeDrain());
            } else {
                control.AllowMove("drowning");
                StopAllCoroutines();
            }
        }
    }

    private Animator animator;
    private ThirdPersonUserControl control;
    private Cloth scarf;
    private Life life;
    void Start() {
        animator = GetComponent<Animator>();
        control = GetComponent<ThirdPersonUserControl>();
        scarf = GetComponentInChildren<Cloth>();
        life = FindObjectOfType<Life>();
    }

    private IEnumerator LifeDrain() {
        while (true) {
            life.Decrease();
            yield return new WaitForSeconds(lifeDrainTime);
        }
    }

    void OnAnimatorIK() {
        if (!isDrowning ) return;


        Vector3 rightPosition = new Vector3(ikMax.x, ikMax.y, ikMax.z * Mathf.Sin(Time.time * cycleRate));
        Vector3 leftPosition = Quaternion.Euler(0, 180, 0) * rightPosition;

        animator.SetIKPosition(AvatarIKGoal.LeftHand, transform.TransformPoint(leftPosition));
        animator.SetIKPosition(AvatarIKGoal.RightHand, transform.TransformPoint(rightPosition));
        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, isDrowning ? 1 : 0);
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, isDrowning ? 1 : 0);
    }
}
