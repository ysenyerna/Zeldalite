using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{

	[SerializeField] float speed = 4f;




	private bool attacking;


	InputActionMap input;
	Rigidbody2D body;
	Animator anim;

	private void Start()
	{
		// Get input
		input = InputSystem.actions.actionMaps.First(m => m.name == "Player");

		// Get components
		body = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();

	}





	private void Update()
	{
		Animate();
		HandleAttacking();
	}

	private string animationDirection = "Down";
	private void Animate()
	{
		var vel = body.linearVelocity;

		// Get animation
		string animation;
		if (attacking)
			animation = "Attack";
		else if (vel != Vector2.zero)
			animation = "Run";
		else
			animation = "Idle";

		// Get direction
		if (vel != Vector2.zero)
		{
			if (vel.x != 0)
				animationDirection = vel.x > 0 ? "Right" : "Left";
			else 
				animationDirection = vel.y > 0 ? "Up" : "Down";
		}


		// Play animation
		anim.Play(animation + animationDirection);

	}

	private void HandleAttacking()
	{
		if (input["Attack"].WasPressedThisFrame())
		{
			attacking = true;
		}
		
	}

	private void AttackAnimationFinished()
	{
		attacking = false;
	}


	private void FixedUpdate()
	{
		HandleMovement();
	}


	private void HandleMovement()
	{
		Vector2 dir = Vector2.zero;
		if (!attacking)
		{
			dir = input["Move"].ReadValue<Vector2>();
			dir = Vector2.Normalize(dir);
		}



		body.linearVelocity = dir * speed;


		
	}




}
