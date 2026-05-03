using System;
using UnityEngine;

public class Timer : MonoBehaviour
{
	[Tooltip("Optional identifier which can be used to access the timer.")]
	public string id = "";
	[Tooltip("The amount of time the timer runs for in seconds.")]
	public float time = 1;


	public event Action Timeout;

	public bool Running { get; private set; } = false;

	public float ElapsedTime { get; private set; } = 0f;


	// Starts the timer 
	public void Run()
	{
		Running = true;
	}

	// Stops the timer
	public void Stop()
	{
		Running = false;
		ElapsedTime = 0f;
	}

	// Returns the amount of time remaining on the timer
	public float GetRemainingTime()
		=> time - ElapsedTime;




	// PRIVATE METHODS

	void Update()
	{
		HandleTime();
	}

	void HandleTime()
	{
		if (!Running)
			return;

		ElapsedTime += Time.deltaTime;
		if (ElapsedTime >= time)
			{
				Stop();
				Timeout?.Invoke();
			}
	}
}
