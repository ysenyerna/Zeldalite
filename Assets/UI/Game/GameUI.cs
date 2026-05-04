using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{

	private Image health;
	private Player player;
	private bool gameEnded = false;

	private float healthStartPos;

	[SerializeField] private float endScreenFadePercent;

	private readonly List<Object> endScreenFadeObjects = new();

	private SpriteButton restartButton;
	private TMP_Text endScreenMessage;

	private void Start()
	{
		player = GameObject.FindWithTag("Player").GetComponent<Player>();
		player.GameEnded += Player_GameEnded;

		// Health bar
		health = GameObject.Find("Health").GetComponent<Image>();
		healthStartPos = health.rectTransform.anchoredPosition.x;

		// Restart button
		restartButton = transform.Find("Fade/RestartButton").GetComponent<SpriteButton>();
		restartButton.Pressed += RestartButton_Pressed;

		// Add end screen objects to fade
		endScreenMessage = transform.Find("Fade/Message").GetComponent<TMP_Text>();
		endScreenFadeObjects.Add(endScreenMessage);
		endScreenFadeObjects.Add(restartButton.GetComponent<TMP_Text>());
		endScreenFadeObjects.Add(transform.Find("Fade").GetComponent<Image>());
	}


	private float healthPercent = 1f;
	private void Update()
	{
		HandleHealthBar();

		HandleEndScreen();
	}

	private void HandleHealthBar()
	{
		// Set position and scale of the health bar
		var targetHealthPercent = Mathf.Clamp01((float)player.Health / player.MaxHealth);
		healthPercent = Mathf.Lerp(healthPercent, targetHealthPercent, Time.deltaTime * 8f);

		if (healthPercent == 0)
			health.enabled = false;
		else
		{
			health.enabled = true;
			var trs = health.rectTransform;
			var halfWidth = trs.sizeDelta.x / 2f;
			health.rectTransform.anchoredPosition = new (-halfWidth * (1 - healthPercent) + healthStartPos, trs.anchoredPosition.y);
			trs.localScale = new (healthPercent, trs.localScale.y, trs.localScale.z);
		}
	}


	private float oldEndScreenPercent = -1;
	private void HandleEndScreen()
	{

		// Enable/Disable end screen
		const float fadeSpeed = 7.5f;
		if (gameEnded && endScreenFadePercent < 1f)
		{
			endScreenFadePercent = Mathf.Lerp(endScreenFadePercent, 1f, fadeSpeed * Time.deltaTime);
		}
		else if (endScreenFadePercent > 0f)
		{
			endScreenFadePercent = Mathf.Lerp(endScreenFadePercent, 0f, fadeSpeed * Time.deltaTime);
		}


		// Update end screen fade
		endScreenFadePercent = Mathf.Clamp01(endScreenFadePercent);
		if (endScreenFadePercent != oldEndScreenPercent)
		{
			oldEndScreenPercent = endScreenFadePercent;

			foreach (object obj in endScreenFadeObjects)
			{
				if (obj is Image i)
					i.color = new (i.color.r, i.color.g, i.color.b, endScreenFadePercent);
				if (obj is TMP_Text t)
					t.color = new (t.color.r, t.color.g, t.color.b, endScreenFadePercent);
			}
		}
	}

	private void RestartButton_Pressed()
	{
		SceneManager.LoadScene("World");
	}

	private void Player_GameEnded()
	{
		endScreenMessage.text = player.IsDead ? "You Died" : "You Win!";
		gameEnded = true;
		restartButton.enabled = true;
	}

}
