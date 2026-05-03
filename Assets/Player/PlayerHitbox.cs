using UnityEngine;

public class PlayerHitbox : MonoBehaviour
{

	private Player player;


	private void Start()
	{
		player = GameObject.FindWithTag("Player").GetComponent<Player>();
	}


	private void OnTriggerEnter2D(Collider2D collider)
	{
		if ( !collider.transform.parent.TryGetComponent<IEnemy>(out var enemy ))
			return;


		// Get hit
		var knockback = Vector2.Normalize(player.Position - enemy.Position) * enemy.Knockback;
		player.GotHit(enemy.Damage, knockback);
		enemy.HitPlayer();

	}
}
