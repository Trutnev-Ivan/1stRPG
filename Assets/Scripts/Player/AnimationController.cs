using UnityEngine;

[RequireComponent(
	typeof(MovementController), 
	typeof(JumpingController),
	typeof(SittingController)
)]
public class AnimationController : MonoBehaviour
{
	[SerializeField] private Animator playerAnimator;
	[SerializeField] private PlayerController playerController;

	private MovementController movementController;
	private JumpingController jumpingController;
	private SittingController sittingController;

	void Awake()
	{
		movementController = GetComponent<MovementController>();
		jumpingController = GetComponent<JumpingController>();
		sittingController = GetComponent<SittingController>();
	}
	
	void Update()
	{
		playerAnimator.SetBool("isMoving", movementController.IsMoving);
		playerAnimator.SetBool("isRunning", movementController.IsRunning);
		playerAnimator.SetBool("isJumping", jumpingController.IsJumping);
		playerAnimator.SetBool("isGrounded", playerController.isGrounded());
		playerAnimator.SetBool("isSitting", sittingController.isSitting());
	}
}