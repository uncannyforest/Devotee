using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class Drown : MonoBehaviour {
    public float cycleRate = 2f;
    public Vector3 ikMax = new Vector3(.5f, .5f, 1f);
    public float scarfForce = 20;
    public bool isDrowning = false;
    public GameObject splash;
    public float delayedDisable = .1f;

    public bool IsDrowning {
        set {
            isDrowning = value;

            scarf.externalAcceleration = Vector3.up * (isDrowning ? scarfForce : 0);
            if (isDrowning) {
                CancelInvoke();
                character.SetDrowning(true);
                splash.SetActive(true);
                StartCoroutine(LifeDrain());
            } else {
                Invoke("DisableDrowning", delayedDisable);
            }
        }
    }

    private Animator animator;
    private ThirdPersonCharacter character;
    private Cloth scarf;
    private Life life;
    void Start() {
        animator = GetComponent<Animator>();
        character = GetComponent<ThirdPersonCharacter>();
        scarf = GetComponentInChildren<Cloth>();
        life = FindObjectOfType<Life>();
    }

    private IEnumerator LifeDrain() {
        while (true) {
            life.Decrease();
            yield return null;
        }
    }

    private void DisableDrowning() {
        character.SetDrowning(false);
        splash.SetActive(false);
        StopAllCoroutines();
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
