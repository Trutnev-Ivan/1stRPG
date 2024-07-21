using UnityEngine;

public class CameraController : MonoBehaviour
{
	[SerializeField] private GameObject CameraPivot;
	[SerializeField] private float cameraSpeedZoom = 100;
	[SerializeField] private float cameraMoveSpeedX = 300;
	[SerializeField] private float cameraMoveSpeedY = 800;

	private float cameraPositionOffset = 0;
	private float mouseRotationX;
	private float mouseRotationY;
	private Camera playerCamera;

	const float MIN_CAMERA_POSITION = -7f;
	const float MAX_CAMERA_POSITION = -4f;

	private Quaternion _rotation;
	private bool isLockedRotation = false;

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
		float mouseX = Input.GetAxis("Mouse X");
		float mouseY = Input.GetAxis("Mouse Y");

		mouseX = Mathf.Clamp(mouseX, -1, 1);
		mouseY = Mathf.Clamp(mouseY, -1, 1);

		mouseRotationX -= mouseY * cameraMoveSpeedX * Time.deltaTime;
		mouseRotationX = Mathf.Clamp(mouseRotationX, -20, 50);
		mouseRotationY = mouseX * cameraMoveSpeedY * Time.deltaTime;

		cameraPositionOffset = Input.GetAxis("Mouse ScrollWheel") * cameraSpeedZoom * Time.deltaTime;
	}

	void LateUpdate()
	{
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

	public void setPosition(Vector3 meshPosition)
	{
		CameraPivot.transform.localPosition = meshPosition;
	}

	public Vector3 getCameraForwardVector()
	{
		Vector3 forward = CameraPivot.transform.forward;
		forward.y = 0;
		return forward;
	}
}