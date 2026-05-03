using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{

	[SerializeField] float speed = 4f;


	public readonly int MaxHealth = 10;
	public int Health = 10;
	public Vector2 Position { 
		get { return new(body.transform.position.x, body.transform.position.y); } 
		set { body.transform.position = new (value.x, value.y, body.transform.position.z); }}

	private Vector2 _direction;
	private Vector2 Direction 
	{ 
		get 
		{ 
			var vel = body.linearVelocity;
			if (vel == Vector2.zero || state == State.TakenDamage || state == State.Dying )
				return _direction;

			if (Mathf.Round(vel.x) != 0)
				_direction = vel.x > 0 ? Vector2.right : Vector2.left;
			else 
				_direction = vel.y < 0 ? Vector2.down : Vector2.up;
			
			return _direction;
		} 
	}


	// Components
	InputActionMap input;
	Rigidbody2D body;
	Animator anim;
	CameraController cam;
	BoxCollider2D sword;
	BoxCollider2D hitbox;
	Timer iframeTime;

	// States 
	enum State { Normal, Attacking, TakenDamage, Falling, Dying }
	private State state = State.Normal;


	private void Start()
	{
		// Get input
		input = InputSystem.actions.actionMaps.First(m => m.name == "Player");
		input["Attack"].performed += AttackPressed;

		// Get components
		body = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		cam = GameObject.FindWithTag("MainCamera").GetComponent<CameraController>();
		sword = transform.Find("Sword").GetComponent<BoxCollider2D>();
		hitbox = transform.Find("HitBox").GetComponent<BoxCollider2D>();
		iframeTime = GetComponent<Timer>();
		iframeTime.Timeout += IFrameTime_Timeout;

	}



	private void Update()
	{
		Animate();
		cam.TargetPosition = transform.position;
	}

	private void Animate()
	{
		var vel = body.linearVelocity;

		// Get animation
		string animation;
		if (state == State.Dying)
			animation = "Death";
		else if (state == State.TakenDamage)
			animation = "Hit";
		else if (state == State.Attacking)
			animation = "Attack";
		else if (vel != Vector2.zero)
			animation = "Run";
		else
			animation = "Idle";

		// Get direction
		string directionName = "";
		if (animation != "Death")
			directionName = Direction == Vector2.up ? "Up" : Direction == Vector2.down ? "Down" : Direction == Vector2.right ? "Right" : "Left";

		// Play animation
		anim.Play(animation + directionName);

	}

	private void AttackPressed(InputAction.CallbackContext ctx)
	{
		if (state != State.Normal)
			return; 

		var rotation = Direction == Vector2.up ? 180f : Direction == Vector2.down ? 0f : Direction == Vector2.right ? 90f : 270f;
		sword.transform.eulerAngles = new (0, 0, rotation);
		sword.enabled = true;
		state = State.Attacking;
	}


	private void FixedUpdate()
	{
		hitbox.enabled = state == State.Normal || state == State.Attacking;
		HandleMovement();
	}


	private void HandleMovement()
	{
		if (state == State.Dying || state == State.TakenDamage)
		{
			body.linearVelocity = Vector2.Lerp(body.linearVelocity, Vector2.zero, Time.deltaTime * 5f);
		}
		else if (state == State.Normal)
		{
			Vector2 dir = Vector2.zero;
			dir = input["Move"].ReadValue<Vector2>();
			dir = Vector2.Normalize(dir);
			body.linearVelocity = dir * speed;
		}
		else
		{
			body.linearVelocity = Vector2.zero;
		}

	}

	public void GotHit(int damage, Vector2 knockback = new())
	{
		

		// Cancel attack
		sword.enabled = false;
		body.linearVelocity = knockback;


		Health -= damage;
		if ( Health > 0)
		{
			iframeTime.Run();
			state = State.TakenDamage;
		}
		else
		{
			state = State.Dying;
		}
	}



	// EVENTS
	private void IFrameTime_Timeout()
	{
		body.linearVelocity = Vector2.zero;
		state = State.Normal;

	}

	private void AttackAnimationFinished()
	{
		sword.enabled = false;
		state = State.Normal;
	}

	private void DeathAnimationFinished()
	{
		
	}

}
