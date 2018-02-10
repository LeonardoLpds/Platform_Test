using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
	protected List<RaycastHit2D> hitBufferList = new List<RaycastHit2D> (16);

	void Start () {
		contactFilter.useTriggers = false;
		contactFilter.SetLayerMask (Physics2D.GetLayerCollisionMask (gameObject.layer) );
		contactFilter.useLayerMask = true;
	}

	void OnEnable () {
		body = GetComponent<Rigidbody2D> ();
	}

	void FixedUpdate () {
		velocity += gravityModifier * Physics2D.gravity * Time.deltaTime;

		velocity.x = targetVelocity.x;

		grounded = false;

		Vector2 deltaPosition = velocity * Time.deltaTime;
		Vector2 move = Vector2.up * deltaPosition.y;

		Movement (move, true);
	}

	void Movement (Vector2 move, bool yMovement) {
		float distance = move.magnitude;

		if (distance > minMoveDistance) {
			int count = body.Cast (move, contactFilter, hitBuffer, distance + shellRadius);
			hitBufferList.Clear ();

			for (int i = 0; i < count; i++) {
				hitBufferList.Add (hitBuffer [i]);
			}

			for (int i = 0; i < hitBufferList.Count; i++) {
				Vector2 currentNormal = hitBufferList [i].normal;
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

				float modifyDistance = hitBufferList [i].distance - shellRadius;
				distance = modifyDistance < distance ? modifyDistance : distance;
			}
		}

		body.position += move.normalized * distance;
	}
}
