using UnityEngine;

[RequireComponent(typeof(SittingController))]
public class JumpingController : MonoBehaviour
{
	[SerializeField] private PlayerController playerController;
	[SerializeField] private float maxJumpHeight = 21.0f;
	[SerializeField] private float stepJump = 4;

	private SittingController sittingController;
	private float y = 0;

	void Awake()
	{
		sittingController = GetComponent<SittingController>();
	}

	void Update()
	{
		checkJump();
	}

	private void checkJump()
	{
		if (sittingController.isNeedStand())
		{
			IsJumpingPressing = false;
			NeedJump = false;

			return;
		}

		if (!IsJumpingPressing && isAxisJumpPressed() && !NeedJumpingKeyRelease)
		{
			NeedJump = true;
		}

		IsJumpingPressing = isAxisJumpPressed();

		if (!IsJumpingPressing && NeedJumpingKeyRelease)
		{
			NeedJumpingKeyRelease = false;
		}
	}

	protected void fillVerticalMove()
	{
		if (playerController.isGrounded()
		    && IsJumpingPressing
		    && NeedJump
		    && !NeedJumpingKeyRelease)
		{
			y = 0;
			NeedJump = false;
			IsJumping = true;
		}
		else if (y <= maxJumpHeight && IsJumping && !NeedJumpingKeyRelease)
		{
			y += 10 * stepJump * Time.deltaTime;
		}
		else if (y > playerController.getMass() * Physics.gravity.y)
		{
			IsJumping = false;
			y += playerController.getMass() * Physics.gravity.y;
		}
	}

	public float getVerticalCoordinate()
	{
		fillVerticalMove();
		return y;
	}

	public bool NeedJumpingKeyRelease { set; get; }
	public bool NeedJump { set; get; }
	public bool IsJumpingPressing { set; get; }
	public bool IsJumping { set; get; }

	public bool isAxisJumpPressed()
	{
		return Input.GetAxisRaw("Jump") != 0;
	}
}