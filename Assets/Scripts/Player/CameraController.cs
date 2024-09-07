using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	[SerializeField] private GameObject CameraPivot;
	[SerializeField] private float cameraSpeedZoom = 100;
	[SerializeField] private float cameraMoveSpeedX = 300;
	[SerializeField] private float cameraMoveSpeedY = 800;
	[SerializeField] private Vector3 velocitySmoothnes;

	private float cameraPositionOffset = 0;
	private float mouseRotationX;
	private float mouseRotationY;
	private Camera playerCamera;

	const float MIN_CAMERA_POSITION = -7f;
	const float MAX_CAMERA_POSITION = -4f;
	
	RaycastHit wallHit = new RaycastHit();

	private Quaternion _rotation;
	private PlayerController pc;
	
	void Start()
	{
		playerCamera = GetComponentInChildren<Camera>();

		cameraPositionOffset = playerCamera.transform.position.z;
		if (cameraPositionOffset < MIN_CAMERA_POSITION)
		{
			cameraPositionOffset = MIN_CAMERA_POSITION;
		}
		else if (cameraPositionOffset > MAX_CAMERA_POSITION)
		{
			cameraPositionOffset = MAX_CAMERA_POSITION;
		}
	}
	
	void Update()
	{
		float mouseX = Input.GetAxisRaw("Mouse X");
		float mouseY = Input.GetAxisRaw("Mouse Y");

		mouseX = Mathf.Clamp(mouseX, -1, 1);
		mouseY = Mathf.Clamp(mouseY, -1, 1);

		mouseRotationX -= mouseY * cameraMoveSpeedX * Time.deltaTime;
		mouseRotationX = Mathf.Clamp(mouseRotationX, -20, 50);
		mouseRotationY = mouseX * cameraMoveSpeedY * Time.deltaTime;

		cameraPositionOffset = Input.GetAxis("Mouse ScrollWheel") * cameraSpeedZoom * Time.deltaTime;
		
		rotateCamera();
		zoomCamera();
	}

	protected void rotateCamera()
	{
		CameraPivot.transform.Rotate(0, mouseRotationY, 0);
		CameraPivot.transform.eulerAngles = new Vector3(mouseRotationX, CameraPivot.transform.eulerAngles.y, 0);
	}

	protected void zoomCamera()
	{
		bool canCloserCamera = cameraPositionOffset < 0 && playerCamera.transform.localPosition.z > MIN_CAMERA_POSITION;
		bool canAwayCamera = cameraPositionOffset > 0 && playerCamera.transform.localPosition.z < MAX_CAMERA_POSITION;

		if (canAwayCamera || canCloserCamera)
		{
			playerCamera.transform.Translate(0, 0, cameraPositionOffset);
		}
	}
	
	public void setPosition(ref PlayerController playerController)
	{
		
		UnityEngine.Collider[] colliders = Physics.OverlapSphere(playerCamera.transform.position, 1);

		// if (Physics.Linecast(playerCamera.transform.position, playerController.getGlobalPosition(), out wallHit))
		// if (colliders.Length > 0)
		// {
		// 	Vector3 closestPoint = colliders[0].ClosestPointOnBounds(playerCamera.transform.position);
		// 	
		// 	if (Vector3.Distance(closestPoint, playerController.getGlobalPosition()) < 5)
		// 	{
		// 		CameraPivot.transform.position = smoothMove(
		// 			CameraPivot.transform.position,
		// 			// new Vector3(wallHit.point.x, wallHit.point.y, wallHit.point.z)
		// 			colliders[0].ClosestPointOnBounds(playerCamera.transform.position)
		// 		);
		// 	}
		// 	else
		// 	{
		// 		CameraPivot.transform.position = smoothMove(
		// 			CameraPivot.transform.position,
		// 			// new Vector3(wallHit.point.x, wallHit.point.y, wallHit.point.z)
		// 			closestPoint - ( new Vector3(0, 0, 5))
		// 		);				
		// 	}
		//
		// 	// playerCamera.transform.localPosition = new Vector3(0, 0, Vector3.Distance(transform.position, wallHit.point));
		// }

		CameraPivot.transform.localPosition = smoothMove(
			CameraPivot.transform.localPosition,
			playerController.getLocalPosition()
		);

		int minDistance = 2;
		int maxDistance = 10;
		
		Vector3 desiredPos = CameraPivot.transform.TransformPoint(playerCamera.transform.localPosition.normalized * maxDistance);
		float distance = maxDistance;
		
		Debug.DrawLine(CameraPivot.transform.position + new Vector3(0, playerController.getHeight() / 2, 0), desiredPos, Color.red);
		
		if (Physics.Linecast(CameraPivot.transform.position + new Vector3(0, 0, playerController.getHeight() / 2), desiredPos, out wallHit))
		{
			distance = Mathf.Clamp(wallHit.distance, minDistance, maxDistance);
			
		}
		else
		{
			
			// CameraPivot.transform.localPosition = smoothMove(
				// CameraPivot.transform.localPosition,
				// playerController.getLocalPosition()
			// );
		}

		playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, playerCamera.transform.localPosition.normalized * distance, Time.deltaTime * 10);
	}

	protected Vector3 smoothMove(Vector3 from, Vector3 to)
	{
		return Vector3.SmoothDamp(from,
			to,
			ref velocitySmoothnes, 
			10 * Time.deltaTime);
	}
	
	public Vector3 getCameraForwardVector()
	{
		Vector3 forward = CameraPivot.transform.forward;
		forward.y = 0;
		return forward;
	}

	// Нормаль камеры в плоскости XZ (вокруг Y), повернутая на угол angle
	public Vector3 getRotatedForwardXZ(float angle)
	{
		Vector3 cameraForward = getCameraForwardVector();
		
		return new Vector3(
			cameraForward.x * Mathf.Cos(angle) - cameraForward.z * Mathf.Sin(angle),
			0,
			cameraForward.z * Mathf.Cos(angle) + cameraForward.x * Mathf.Sin(angle));
	}
}