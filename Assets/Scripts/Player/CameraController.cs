using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

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
	private bool canZoom = true;

	const float MIN_CAMERA_POSITION = -4f;
	const float MAX_CAMERA_POSITION = -3f;
	
	RaycastHit wallHit = new RaycastHit();

	private Quaternion _rotation;
	private Vector3 cameraPosition;
	
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
		
		playerCamera.transform.localPosition = new Vector3(
			playerCamera.transform.localPosition.x, 
			playerCamera.transform.localPosition.y, 
			cameraPositionOffset);
		
		cameraPosition = playerCamera.transform.localPosition;
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

		if (canZoom && (canAwayCamera || canCloserCamera))
		{
			playerCamera.transform.Translate(0, 0, cameraPositionOffset);
			cameraPosition = playerCamera.transform.localPosition;
		}
	}
	
	public void setPosition(ref PlayerController playerController)
	{
		CameraPivot.transform.localPosition = smoothMove(
			CameraPivot.transform.localPosition,
			playerController.getLocalPosition()
		);

		int minDistance = 2;
		int maxDistance = 5;
		
		Vector3 desiredPos = CameraPivot.transform.TransformPoint(playerCamera.transform.localPosition.normalized * maxDistance);
		
		if (Physics.Linecast(CameraPivot.transform.position + new Vector3(0, 0, playerController.getHeight() / 2), desiredPos, out wallHit))
		{
			float _distance = Mathf.Clamp(wallHit.distance, minDistance, maxDistance);
			playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, playerCamera.transform.localPosition.normalized * _distance, Time.deltaTime * 10);
			canZoom = false;
		}
		else
		{
			canZoom = true;
			playerCamera.transform.localPosition = cameraPosition;
		}
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