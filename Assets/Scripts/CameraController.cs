using UnityEngine;

public class CameraController : MonoBehaviour
{

	public Vector2 TargetPosition = Vector2.zero;
	public float LimitLeft = float.MinValue;
	public float LimitRight = float.MaxValue;
	public float LimitTop = float.MaxValue;
	public float LimitBottom  = float.MinValue;
	public float CameraSmoothing = 10f;
	public bool SyncToFixedUpdate = false;

	Camera cam;

	private void Start()
	{
		cam = GetComponent<Camera>();
	}


	private void Update()
	{
		if (!SyncToFixedUpdate)
			UpdatePosition(Time.deltaTime);
	}

	private void FixedUpdate()
	{
		if (SyncToFixedUpdate)
			UpdatePosition(Time.fixedDeltaTime);
	}

	private void OnTriggerEnter2D(Collider2D collider)
	{
		if (!collider.CompareTag("CameraZone"))
			return;

		if (collider is not BoxCollider2D box)
		{
			print($"Camera zone {collider.name} does not have a box collider.");
			return;
		}

		UpdateCameraLimitsToCameraZone(box);
	}

	public void UpdateCameraLimitsToCameraZone(BoxCollider2D zone)
	{
		var pos = new Vector2(zone.transform.position.x, zone.transform.position.y) + zone.offset;
		var halfWidth = zone.size.x / 2;
		var halfHeight = zone.size.y / 2;

		LimitLeft = pos.x - halfWidth;
		LimitRight = pos.x + halfWidth;
		LimitBottom = pos.y - halfHeight;
		LimitTop = pos.y + halfHeight;

	}

	private void UpdatePosition(float delta)
	{
		var actualTarget = ApplyLimits(TargetPosition);

		// Apply smoothing
		var newPos = Vector2.Lerp(cam.transform.position, actualTarget, CameraSmoothing * delta);

		// Apply new position
		cam.transform.position = new Vector3(newPos.x, newPos.y, cam.transform.position.z);

	}

	// Applies camera limits to a target position and returns the result
	private Vector2 ApplyLimits(Vector2 targetPos)
	{
		// Left and right limits
		var halfWidth = cam.orthographicSize * cam.aspect;

		if ( halfWidth * 2 > LimitRight - LimitLeft)
			targetPos.x = (LimitLeft + LimitRight) / 2;
		else
			targetPos.x = Mathf.Clamp(targetPos.x, LimitLeft + halfWidth, LimitRight - halfWidth);

		// Top and bottom limits
		var halfHeight = cam.orthographicSize;

		if (halfHeight * 2 > LimitTop - LimitBottom)
			targetPos.y = (LimitTop + LimitBottom) / 2;
		else 
			targetPos.y = Mathf.Clamp(targetPos.y, LimitBottom + halfHeight, LimitTop - halfHeight);

		return targetPos;
	}


	// Updates the camera position without smoothing (instantly) for one frame than goes back to normal
	public void UpdatePositionNoSmoothing()
	{
		var target = ApplyLimits(TargetPosition);
		cam.transform.position = new Vector3(target.x, target.y, cam.transform.position.z);
	}

}
