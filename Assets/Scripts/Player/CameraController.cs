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
		if (Physics.Linecast(playerCamera.transform.position, playerController.getGlobalPosition(), out wallHit))
		{
			CameraPivot.transform.position = smoothMove(
				CameraPivot.transform.position,
				new Vector3(wallHit.point.x, wallHit.point.y, wallHit.point.z)
			);
		}
		else
		{
			CameraPivot.transform.localPosition = smoothMove(
				CameraPivot.transform.localPosition,
				playerController.getLocalPosition()
			);
		}
	}

	protected Vector3 smoothMove(Vector3 from, Vector3 to)
	{
		return Vector3.SmoothDamp(from,
			to,
			ref velocitySmoothnes, 
			Time.deltaTime);
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