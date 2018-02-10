using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : MonoBehaviour {
	// Distancia minima para verificar colisões
	protected const float minMoveDistance = 0.001f;
	// Define um raio de colisão segura
	protected const float shellRadius = 0.01f;

	// Modificador da gravidade padrão
	public float gravityModifier = 1f;
	// Distancia minima do chão para colidir
	public float minGroundNormalY = 0.65f;

	// Vetor de colisão com o chão
	protected Vector2 groundNormal;
	// Verifica se o corpo está colidindo com o chão
	protected bool grounded;
	// Corpo (RigidBody) do objeto
	protected Rigidbody2D body;
	// Matrix/Vetor de velocidade
	protected Vector2 velocity;
	// Filtra os resultados de colisões
	protected ContactFilter2D contactFilter;
	// Array de colisões
	protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];
	// Lista de colisões
	protected List<RaycastHit2D> hitBufferList = new List<RaycastHit2D> (16);

	// Inicialização do objeto
	void Start () {
		contactFilter.useTriggers = false;
		contactFilter.SetLayerMask (Physics2D.GetLayerCollisionMask (gameObject.layer) );
		contactFilter.useLayerMask = true;
	}

	// Ativa quando o objecto é ativado e está disponível
	void OnEnable () {
		body = GetComponent<Rigidbody2D> ();
	}

	// Chamado a cada frame, de forma fixa
	void FixedUpdate () {
		// Define o vetor de velocidade multiplicado pela gravidade e aumentando conforme o tempo
		velocity += gravityModifier * Physics2D.gravity * Time.deltaTime;
		// Define que o corpo não está colidindo com o chão (antes de determinar qualquer colisão)
		grounded = false;
		// Define um vetor de posição baseado na velocidade
		Vector2 deltaPosition = velocity * Time.deltaTime;
		// Define o movimento em Y
		Vector2 move = Vector2.up * deltaPosition.y;
		// Move o corpo do objeto
		Movement (move, true);
	}

	// Função de movimento do corpo
	void Movement (Vector2 move, bool yMovement) {
		float distance = move.magnitude;

		if (distance > minMoveDistance) {
			// Recupera a quantidade de colisões
			int count = body.Cast (move, contactFilter, hitBuffer, distance + shellRadius);
			// Adiciona as colisões na lista
			hitBufferList.Clear ();
			for (int i = 0; i < count; i++) {
				hitBufferList.Add (hitBuffer [i]);
			}

			// Valida as normalizações da lista
			for (int i = 0; i < hitBufferList.Count; i++) {
				Vector2 currentNormal = hitBufferList [i].normal;
				// Verifica se o corpo está tocando no chão
				if (currentNormal.y > minGroundNormalY) {
					grounded = true;
					if (yMovement) {
						groundNormal = currentNormal;
						currentNormal.x = 0;
					}
				}

				// Verifica o produto escalar entre a velocidade e a colisão atual
				// Caso esteja colidingo cancela a velocidade e para parar o corpo
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
