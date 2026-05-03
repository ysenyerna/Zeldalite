using UnityEngine;

public class Spinner : MonoBehaviour, IEnemy
{

	[SerializeField] private int maxHp = 5;
	[SerializeField] private int hp;
	[SerializeField] private float speed = 5f;


	[SerializeField] private float leftPoint;
	[SerializeField] private float rightPoint;

	[SerializeField] bool moving = false;
	[SerializeField] bool goingLeft = false;
	[SerializeField] bool vertical = false;
	

	// Enemy Interface
	public int Damage { get; } = 1;
	public Vector2 Position { get { return new(body.transform.position.x, body.transform.position.y); } }
	public float Knockback { get; } = 12f;

	public void HitPlayer()
	{
		
	}

	private bool dead = false;
	bool inHitAnimation = false;
	Rigidbody2D body;
	Animator anim;
	Player player;
	Timer waitTime;
	BoxCollider2D hitbox;



	private void Start()
	{
		hp = maxHp;
		body = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		hitbox = transform.Find("HitBox").GetComponent<BoxCollider2D>();
		player = GameObject.FindWithTag("Player").GetComponent<Player>();
		waitTime = gameObject.AddComponent<Timer>();
		waitTime.time = 1f;
		waitTime.Timeout += WaitTime_Timeout;
		
	}

	private void Update()
	{
		if (dead)
			return;
		else if (inHitAnimation) 
			anim.Play("SpinnerHit");
		else  if (moving)
			anim.Play("SpinnerRun");
		else 
			anim.Play("SpinnerIdle");
	}

	private void FixedUpdate()
	{
		if (moving && !dead)
		{
			float newPos;
			var pos = !vertical ? Position.x : Position.y;
			// Going Left
			if (goingLeft)
			{
				newPos = Mathf.MoveTowards(pos, leftPoint, speed * Time.deltaTime);
				if (newPos <= leftPoint)
					StopMoving();
			}
			// Going right
			else
			{
				newPos = Mathf.MoveTowards(pos, rightPoint, speed * Time.deltaTime);
				if (newPos >= rightPoint)
					StopMoving();
			}
			
			// Update position
			if (!vertical)
				body.position = new (newPos, Position.y);
			else 
				body.position = new (Position.x, newPos);
		}
		body.linearVelocity = Vector2.Lerp(body.linearVelocity, Vector2.zero, Time.deltaTime * 5f);

	}

	void StopMoving()
	{
		goingLeft = !goingLeft;
		moving = false;
		waitTime.Run();
	}

	private void OnTriggerEnter2D(Collider2D collider)
	{
		// Handle falling in pits
		if (collider.gameObject.CompareTag("Pit"))
		{
			hitbox.enabled = false;
			dead = true;
			inHitAnimation = true;
			body.linearVelocity = Vector2.zero;
			anim.Play("SpinnerDeath");
		}

		// Handle getting hit by player sword
		if ( inHitAnimation || !collider.gameObject.CompareTag("PlayerSword") ) 
			return;

		var playerPos = player.transform.position;
		var pos = transform.position;
		body.linearVelocity = Vector2.Normalize(pos - playerPos) * 4f;
		hp -= 1;
		inHitAnimation = true;
		if (hp > 0)
		{
			anim.Play("SpinnerHit");
		}
		else
		{
			hitbox.enabled = false;
			dead = true;
			anim.Play("SpinnerDeath");
		}

		
	}



	private void HitAnimationFinished()
	{
		inHitAnimation = false;
	}

	private void WaitTime_Timeout()
	{
		moving = true;
	}

	private void DeathAnimationFinished()
	{
		Destroy(gameObject);
	}
}
