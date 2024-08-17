using UnityEngine;

[RequireComponent(typeof(CameraController), typeof(SittingController), typeof(JumpingController))]
public class MovementController : MonoBehaviour
{
	[SerializeField] private PlayerController playerController;

	[SerializeField] private float rotateSpeedForwardBack = 49.0f;
	[SerializeField] private float rotateSpeed45 = 20.0f;
	[SerializeField] private float rotateSpeedHorizontal = 55.0f;

	[Header("Speed")]
	[SerializeField] private float moveSpeed = 800f;
	[SerializeField] private float runSpeed = 800f;
	[SerializeField] private float sittingSpeed = 800f;
	[SerializeField] private float sittingRunSpeed = 800f;
	
	private CameraController cameraController;
	private SittingController sittingController;
	private JumpingController jumpingController;
	
	private bool isRunning;
	
	void Start()
	{
		cameraController = GetComponent<CameraController>();
		sittingController = GetComponent<SittingController>();
		jumpingController = GetComponent<JumpingController>();
	}

	private void Update()
	{
		toggleRun();
	}

	private void toggleRun()
	{
		if (Input.GetKeyDown(KeyCode.CapsLock)) {
			isRunning = !isRunning;
		}
	}
	
	private void FixedUpdate()
	{
		applyMove();
	}

	protected void applyMove()
	{
		Vector3 moveDirection = getMoveDirection();

		if (!moveDirection.Equals(Vector3.zero))
		{
			playerController.setTransformForward(moveDirection);
		}
		
		cameraController.setPosition(ref playerController);

		moveDirection.y = jumpingController.getVerticalCoordinate();
		playerController.move(moveDirection * Time.deltaTime);
	}

	protected Vector3 getMoveDirection()
	{
		Vector3 moveDirection = new Vector3(0, 0, 0);
		
		if (needRotateTo45())
		{
			moveDirection = getMoveRotatedTo45();
		}
		else if (needMoveVertical())
		{
			moveDirection = getVerticalMove();
		}
		else if (needMoveHorizontal())
		{
			moveDirection = getHorizontalMove();
		}
		
		moveDirection *= getSpeed();
		
		return moveDirection;
	}

	protected float getSpeed()
	{
		if (sittingController.isSitting() && isRunning)
			return sittingRunSpeed;

		if (sittingController.isSitting())
			return sittingSpeed;

		if (isRunning)
			return runSpeed;
		
		return moveSpeed;
	}
	
	protected Vector3 getMoveRotatedTo45()
	{
		float angle = getAngle(getAxisVerticalRaw(), getAxisHorizontalRaw());
		return Vector3.MoveTowards(
			playerController.getTransfromForward(),
			cameraController.getRotatedForwardXZ(angle),
			rotateSpeed45
		);
	}

	protected float getAngle(float vertical, float horizontal)
	{
		float angle = 45;
		bool isQuarter1 = vertical >= 0 && horizontal >= 0;
		bool isQuarter2 = vertical >= 0 && horizontal < 0;
		bool isQuarter3 = vertical < 0 && horizontal < 0;
		bool isQuarter4 = vertical < 0 && horizontal >= 0;

		if (isQuarter1)
			angle = -45;
		else if (isQuarter2)
			angle = 45;
		else if (isQuarter3)
			angle = 90;
		else if (isQuarter4)
			angle = -90;

		return angle;
	}

	protected Vector3 getVerticalMove()
	{
		return Vector3.MoveTowards(playerController.getTransfromForward(),
			getAxisVerticalRaw() * cameraController.getCameraForwardVector(), rotateSpeedForwardBack);
	}

	protected Vector3 getHorizontalMove()
	{
		Vector3 cameraForward = cameraController.getCameraForwardVector();
		float k = getAxisHorizontalRaw();

		return Vector3.MoveTowards(
			playerController.getTransfromForward(),
			new Vector3(k * cameraForward.z, 0, -k * cameraForward.x), 
			rotateSpeedHorizontal
		);
	}

	protected bool needRotateTo45()
	{
		return getAxisVerticalRaw() != 0 && getAxisHorizontalRaw() != 0;
	}

	protected bool needMoveVertical()
	{
		return getAxisVerticalRaw() != 0 && getAxisHorizontalRaw() == 0;
	}

	protected bool needMoveHorizontal()
	{
		return getAxisHorizontalRaw() != 0 && getAxisVerticalRaw() == 0;
	}
	
	protected float getAxisVerticalRaw()
	{
		return Input.GetAxisRaw("Vertical");
	}
	
	protected float getAxisHorizontalRaw()
	{
		return Input.GetAxisRaw("Horizontal");
	}
}