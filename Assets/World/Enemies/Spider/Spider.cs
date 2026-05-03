using System;
using Unity.VisualScripting;
using UnityEngine;

public class Spider : MonoBehaviour
{

	[SerializeField] private int maxHp = 4;
	[SerializeField] private float speed = 2.4f;
	[SerializeField] private float aggroRange = 5f;

	private int hp;

	public bool Aggro { get; private set; } = false;

	Player player;

	Rigidbody2D body;
	Animator anim;

	private bool inHitAnimation;


	private void Start()
	{
		player = GameObject.FindWithTag("Player").GetComponent<Player>();

		body = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();

		hp = maxHp;
	}


	private void Update()
	{
		// Handle animation
		if (!inHitAnimation)
		{
			if (body.linearVelocity != Vector2.zero)
				anim.Play("SpiderRun");
			else
				anim.Play("SpiderIdle");
		}
	}

	private void FixedUpdate()
	{
		if ( !inHitAnimation )
		{
			var playerPos = player.transform.position;
			var pos = transform.position;

			// Handle aggro
			if ( !Aggro && Vector2.Distance(playerPos, pos) < aggroRange )
				Aggro = true;
			else if ( Aggro && Vector2.Distance(playerPos, pos) > aggroRange * 1.25f )
				Aggro = false;

			// Follow player while aggro
			if (Aggro)
			{
				var dir = Vector2.Normalize(playerPos - pos);
				body.linearVelocity = dir * speed;
			}
				
			else 
				body.linearVelocity = Vector2.zero;
		}
		else
		{
			body.linearVelocity = Vector2.Lerp(body.linearVelocity, Vector2.zero, Time.deltaTime * 5f);
		}

	}

	private void OnTriggerEnter2D(Collider2D collider)
	{

		if ( inHitAnimation || !collider.gameObject.CompareTag("PlayerSword") ) 
			return;

		var playerPos = player.transform.position;
		var pos = transform.position;
		body.linearVelocity = Vector2.Normalize(pos - playerPos) * 6f;
		hp -= 1;
		inHitAnimation = true;
		if (hp > 0)
		{
			anim.Play("SpiderHit");
		}
		else
		{
			anim.Play("SpiderDeath");
		}

		
	}

	private void HitAnimationFinished()
	{
		inHitAnimation = false;
	}

	private void DeathAnimationFinished()
	{
		Destroy(gameObject);
	}

}
