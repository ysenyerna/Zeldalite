using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{

	[SerializeField] float speed = 4f;



	private Vector2 _direction;
	private Vector2 Direction 
	{ 
		get 
		{ 
			var vel = body.linearVelocity;
			if (vel == Vector2.zero )
				return _direction;

			if (Mathf.Round(vel.x) != 0)
				_direction = vel.x > 0 ? Vector2.right : Vector2.left;
			else 
				_direction = vel.y < 0 ? Vector2.down : Vector2.up;
			
			return _direction;
		} 
	}



	private bool attacking;


	InputActionMap input;
	Rigidbody2D body;
	Animator anim;
	Camera cam;
	BoxCollider2D sword;

	private void Start()
	{
		// Get input
		input = InputSystem.actions.actionMaps.First(m => m.name == "Player");
		input["Attack"].performed += AttackPressed;

		// Get components
		body = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		cam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
		sword = GameObject.Find("Sword").GetComponent<BoxCollider2D>();

	}





	private void Update()
	{
		Animate();
		cam.transform.position = new Vector3 (transform.position.x, transform.position.y, cam.transform.position.z);
	}

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
		var directionName = Direction == Vector2.up ? "Up" : Direction == Vector2.down ? "Down" : Direction == Vector2.right ? "Right" : "Left";

		// Play animation
		anim.Play(animation + directionName);

	}

	private void AttackPressed(InputAction.CallbackContext ctx)
	{
		var rotation = Direction == Vector2.up ? 180f : Direction == Vector2.down ? 0f : Direction == Vector2.right ? 90f : 270f;
		sword.transform.eulerAngles = new (0, 0, rotation);
		sword.enabled = true;
		attacking = true;
	}

	private void AttackAnimationFinished()
	{
		sword.enabled = false;
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
