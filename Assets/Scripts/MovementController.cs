using UnityEngine;

[RequireComponent(typeof(CameraController))]
public class MovementController : MonoBehaviour
{
	private CameraController cameraController;
	
	[SerializeField] private CharacterController PlayerMesh;
	[SerializeField] private float mass = 1; 
	[SerializeField] private float speed = 800f;
	[SerializeField] private float rotateSpeedForwardBack = 49.0f;
	[SerializeField] private float rotateSpeed45 = 20.0f;
	[SerializeField] private float rotateSpeedHorizontal = 55.0f;
	[SerializeField] private float maxJumpHeight = 5.0f;
	[SerializeField] private float stepJump = 1;
	
	private Vector3 velocityY;
	private bool isJumpingPressing;
	private bool isJumping;
	private bool needJump = true;

	void Start()
	{
		cameraController = GetComponent<CameraController>();
	}

	private void Update()
	{
		if (!isJumpingPressing && Input.GetKey(KeyCode.Space))
		{
			needJump = true;
		}
		
		isJumpingPressing = Input.GetKey(KeyCode.Space);
	}
	
	private void FixedUpdate()
	{
		float verticalMove = Input.GetAxisRaw("Vertical");
		float horizontalMove = Input.GetAxisRaw("Horizontal");

		Vector3 moveDirection = new Vector3(0, 0, 0);
		
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
		
		if (verticalMove != 0)
		{
			// rotate to 45 degrees
			if (horizontalMove != 0)
			{
				Vector3 cameraForward = cameraController.getCameraForwardVector();

				float angle = getAngle(verticalMove, horizontalMove);
				
				Vector3 vec = new Vector3(
					cameraForward.x * Mathf.Cos(angle) - cameraForward.z * Mathf.Sin(angle),
					0,
					cameraForward.z * Mathf.Cos(angle) + cameraForward.x * Mathf.Sin(angle));
				PlayerMesh.transform.forward = Vector3.MoveTowards(PlayerMesh.transform.forward, vec, rotateSpeed45);
			}
			// move forward / back
			else
			{
				PlayerMesh.transform.forward = Vector3.MoveTowards(PlayerMesh.transform.forward, verticalMove * cameraController.getCameraForwardVector(), rotateSpeedForwardBack);
			}

			moveDirection = PlayerMesh.transform.forward * Mathf.Abs(verticalMove) * speed;
		}
		else if (horizontalMove != 0)
		{
			Vector3 cameraForward = cameraController.getCameraForwardVector();
			float k = horizontalMove;

			PlayerMesh.transform.forward = Vector3.MoveTowards(PlayerMesh.transform.forward, new Vector3(k * cameraForward.z, 0, -k * cameraForward.x), rotateSpeedHorizontal);

			moveDirection = PlayerMesh.transform.forward * speed * Mathf.Abs(horizontalMove);
		}
		
		cameraController.setPosition(PlayerMesh.transform.localPosition);
		
		moveDirection.y = velocityY.y;
		PlayerMesh.Move(moveDirection*Time.deltaTime);
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

}