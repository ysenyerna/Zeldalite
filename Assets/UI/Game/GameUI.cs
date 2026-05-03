using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{

	private Image health;
	private Player player;

	private float healthStartPos;


	private void Start()
	{
		health = GameObject.Find("Health").GetComponent<Image>();
		player = GameObject.FindWithTag("Player").GetComponent<Player>();

		healthStartPos = health.rectTransform.anchoredPosition.x;

	}


	private float healthPercent = 1f;
	private void Update()
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
}
