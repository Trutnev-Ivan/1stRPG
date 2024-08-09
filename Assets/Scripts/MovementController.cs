using UnityEngine;

[RequireComponent(typeof(CameraController))]
public class MovementController : MonoBehaviour
{
	[SerializeField] private CharacterController PlayerMesh;
	[SerializeField] private float mass = 1;
	[SerializeField] private float speed = 800f;
	[SerializeField] private float rotateSpeedForwardBack = 49.0f;
	[SerializeField] private float rotateSpeed45 = 20.0f;
	[SerializeField] private float rotateSpeedHorizontal = 55.0f;
	[SerializeField] private float maxJumpHeight = 5.0f;
	[SerializeField] private float stepJump = 1;

	private CameraController cameraController;
	private Vector3 velocityY;
	
	private bool isJumpingPressing;
	private bool isJumping;
	private bool needJump = true;
	
	private bool isSitting;
	private bool isSittingPressing;
	private bool _needSitting;
	private bool _needStand;
	private bool needSittingKeyRelease;
	
	private float personHeight;
	[SerializeField] private float personSittingHeight;

	void Start()
	{
		cameraController = GetComponent<CameraController>();
		personHeight = PlayerMesh.height;
	}

	private void Update()
	{
		checkSitting();
		checkJump();
	}

	private void checkJump()
	{
		if (_needStand)
		{
			isJumpingPressing = false;
			needJump = false;

			return;
		}
		
		if (!isJumpingPressing && isAxisJumpPressed())
		{
			needJump = true;
		}

		isJumpingPressing = isAxisJumpPressed();
	}

	private void checkSitting()
	{
		if (!needSittingKeyRelease && isAxisSittingPressed())
		{
			if (!isSitting)
			{
				_needSitting = true;
				_needStand = false;
			}
			else
			{
				_needStand = true;
				_needSitting = false;
			}
		}

		if (isSitting && isAxisJumpPressed())
		{
			_needStand = true;
		}

		isSittingPressing = isAxisSittingPressed();

		if (!isSittingPressing && needSittingKeyRelease)
		{
			needSittingKeyRelease = false;
		}
	}
	
	private void FixedUpdate()
	{
		applySitting();
		applyMove();
	}

	protected void applySitting()
	{
		if (needSitting())
		{
			sit();
		}
		else if (needStand())
		{
			stand();
		}
	}

	protected bool needSitting()
	{
		return PlayerMesh.isGrounded && isSittingPressing && _needSitting;
	}

	protected bool needStand()
	{
		return PlayerMesh.isGrounded && _needStand;
	}

	protected void sit()
	{
		PlayerMesh.height = personSittingHeight;
		isSitting = true;
		needSittingKeyRelease = true;
	}

	protected void stand()
	{
		PlayerMesh.height = personHeight;
		isSitting = false;
		_needStand = false;
		
		if (isSittingPressing)
		{
			needSittingKeyRelease = true;
		}
	}

	protected void applyMove()
	{
		fillVerticalMove();
		Vector3 moveDirection = getMoveDirection();

		if (!moveDirection.Equals(Vector3.zero))
		{
			PlayerMesh.transform.forward = moveDirection;
		}
		
		cameraController.setPosition(PlayerMesh.transform.localPosition);

		moveDirection.y = velocityY.y;
		PlayerMesh.Move(moveDirection * Time.deltaTime);
	}
	
	protected void fillVerticalMove()
	{
		if (PlayerMesh.isGrounded && isJumpingPressing && needJump)
		{
			velocityY.y = 0;
			needJump = false;
			isJumping = true;
		}
		else if (velocityY.y <= maxJumpHeight && isJumping)
		{
			velocityY.y += stepJump;
		}
		else if (velocityY.y > mass * Physics.gravity.y)
		{
			isJumping = false;
			velocityY.y += mass * Physics.gravity.y;
		}
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
		
		moveDirection *= speed;
		
		return moveDirection;
	}

	protected Vector3 getMoveRotatedTo45()
	{
		float angle = getAngle(getAxisVerticalRaw(), getAxisHorizontalRaw());
		return Vector3.MoveTowards(
			PlayerMesh.transform.forward,
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
		return Vector3.MoveTowards(PlayerMesh.transform.forward,
			getAxisVerticalRaw() * cameraController.getCameraForwardVector(), rotateSpeedForwardBack);
	}

	protected Vector3 getHorizontalMove()
	{
		Vector3 cameraForward = cameraController.getCameraForwardVector();
		float k = getAxisHorizontalRaw();

		return Vector3.MoveTowards(
			PlayerMesh.transform.forward,
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

	protected bool isAxisJumpPressed()
	{
		return Input.GetAxisRaw("Jump") != 0;
	}

	protected bool isAxisSittingPressed()
	{
		return Input.GetAxisRaw("Fire1") != 0;
	}
}