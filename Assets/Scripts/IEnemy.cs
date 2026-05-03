using UnityEngine;

public interface IEnemy
{

	// The amount of damage this enemy does
	int Damage { get; }

	Vector2 Position { get; }
	// The knockback velocity for the player when hit by this enemy
	float Knockback { get; }

	// Called when the enemy hits the player
	void HitPlayer();



}