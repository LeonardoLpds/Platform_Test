using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PhysicsObject : MonoBehaviour {
	protected const float minMoveDistance = 0.001f;
	protected const float shellRadius = 0.01f;

	public float gravityModifier = 1f;
	public float minGroundNormalY = 0.65f;
		
	protected Vector2 targetVelocity;
	protected Vector2 groundNormal;
	protected bool grounded;
	protected Rigidbody2D body;
	protected Vector2 velocity;
	protected ContactFilter2D contactFilter;
	protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];

	void Start () {
		contactFilter.useTriggers = false;
		contactFilter.SetLayerMask (Physics2D.GetLayerCollisionMask (gameObject.layer) );
		contactFilter.useLayerMask = true;
	}

	void Update () {
		targetVelocity = Vector2.zero;
		ComputeVelocity ();
	}

	protected virtual void ComputeVelocity () {	}

	void OnEnable () {
		body = GetComponent<Rigidbody2D> ();
	}

	void FixedUpdate () {
		velocity += gravityModifier * Physics2D.gravity * Time.deltaTime;

		velocity.x = targetVelocity.x;

		grounded = false;

		Vector2 deltaPosition = velocity * Time.deltaTime;

		Vector2 moveAlongGround = new Vector2 (groundNormal.y, groundNormal.x);

		Vector2 move = moveAlongGround * deltaPosition.x;

		Movement (move, false);

		move = Vector2.up * deltaPosition.y;

		Movement (move, true);
	}

	void Movement (Vector2 move, bool yMovement) {
		float distance = move.magnitude;

		if (distance > minMoveDistance) {
			int count = body.Cast (move, contactFilter, hitBuffer, distance + shellRadius);

			for (int i = 0; i < count; i++) {
				Vector2 currentNormal = hitBuffer [i].normal;
				if (currentNormal.y > minGroundNormalY) {
					grounded = true;
					if (yMovement) {
						groundNormal = currentNormal;
						currentNormal.x = 0;
					}
				}

				if (Vector2.Dot (velocity, currentNormal) < 0) {
					velocity -= Vector2.Dot (velocity, currentNormal) * currentNormal;
				}

				float modifyDistance = hitBuffer [i].distance - shellRadius;
				distance = modifyDistance < distance ? modifyDistance : distance;
			}
		}

		body.position += move.normalized * distance;
	}
}
