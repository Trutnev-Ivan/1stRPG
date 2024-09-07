using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
	[SerializeField] private float mass = 1;
	[SerializeField] private float stamina = 10;
	[SerializeField] private float stepStaminaDecrease = 0.01f;
	[SerializeField] private MovementController movementController;
	
	private CharacterController characterController;
	private bool _canStandUp = true;
	public float Stamina {get; protected set; }

	void Awake()
	{
		characterController = GetComponent<CharacterController>();
		Stamina = stamina;
	}

	private void Update()
	{
		if (movementController.IsAccelerating && movementController.IsMoving && Stamina > 0)
		{
			Stamina -= stepStaminaDecrease * Time.deltaTime;
			Stamina = Math.Max(Stamina, 0);
		}

		if ((!movementController.IsAccelerating || !movementController.IsMoving) && Stamina < stamina)
		{
			Stamina += stepStaminaDecrease * Time.deltaTime;
			Stamina = Math.Min(Stamina, stamina);
		}
	}

	public void setHeight(float height)
	{
		characterController.height = height;
	}
	
	public void move(Vector3 moveDirection)
	{
		characterController.Move(moveDirection);
	}

	public void setTransformForward(Vector3 forward)
	{
		characterController.transform.forward = forward;
	}

	public float getHeight() => characterController.height;

	public bool isGrounded() => characterController.isGrounded;

	public Vector3 getTransfromForward() => characterController.transform.forward;

	public Vector3 getLocalPosition() => characterController.transform.localPosition;

	public Vector3 getGlobalPosition() => characterController.transform.position;

	public bool canStandUp() => _canStandUp;

	public float getMass() => mass;

	public float ColliderY
	{
		get => characterController.center.y;
		set => characterController.center = new Vector3(
			characterController.center.x,
			value,
			characterController.center.z
			);
	}
	
	void OnTriggerEnter(Collider other)
	{
		if (other.TryGetComponent<Triggers.NoStandTrigger>(out var comp))
		{
			_canStandUp = false;
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.TryGetComponent<Triggers.NoStandTrigger>(out var comp))
		{
			_canStandUp = true;
		}
	}
}