using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : MonoBehaviour {
	// Modificador da gravidade padrão
	public float gravityModifier = 1f;
	// Corpo (RigidBody) do objeto
	protected Rigidbody2D body;
	// Matrix/Vetor de velocidade
	protected Vector2 velocity;

	// Ativa quando o objecto é ativado e está disponível
	void OnEnable () {
		body = GetComponent<Rigidbody2D> ();
	}

	// Chamado a cada frame, de forma fixa
	void FixedUpdate () {
		// Define o vetor de velocidade multiplicado pela gravidade e aumentando conforme o tempo
		velocity += gravityModifier * Physics2D.gravity * Time.deltaTime;
		// Define um vetor de posição baseado na velocidade
		Vector2 deltaPosition = velocity * Time.deltaTime;
		// Define o movimento em Y
		Vector2 move = Vector2.up * deltaPosition.y;
		// Move o corpo do objeto
		Movement (move);
	}

	// Função de movimento do corpo
	void Movement (Vector2 move) {
		body.position += move;
	}
}
