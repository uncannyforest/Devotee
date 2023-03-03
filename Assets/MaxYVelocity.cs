using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MaxYVelocity : MonoBehaviour {
	public float maxYVelocity = 10f;

	new Rigidbody rigidbody;

    void Start() {
		rigidbody = GetComponent<Rigidbody>();
    }

		void Update() {
			Vector3 velocity = rigidbody.velocity;
			if (Mathf.Abs(velocity.y) > maxYVelocity) {
				Debug.Log(gameObject.name + " velocity.y was " + velocity.y + ", clamping to " + maxYVelocity);
				rigidbody.velocity = new Vector3(velocity.x, Mathf.Clamp(velocity.y, -maxYVelocity, maxYVelocity), velocity.z);
			}
		}
}
