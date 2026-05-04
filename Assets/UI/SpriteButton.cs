using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpriteButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{

	public bool Hovered { get; private set; }
	public Action Pressed;


	Vector3 startScale;
	Vector3 targetScale;


	void Start()
	{
		startScale = transform.localScale;
		targetScale = startScale;
	}

	void Update()
	{
		transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * 15f);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		Hovered = true;
		targetScale = new (startScale.x + 0.25f, startScale.y + 0.25f, startScale.z);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		Hovered = false;
		targetScale = startScale;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (eventData.button != PointerEventData.InputButton.Left )
			return;


		Pressed?.Invoke();
		
	}

}
