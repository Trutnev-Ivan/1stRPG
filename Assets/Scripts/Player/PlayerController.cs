using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
	[SerializeField] private float mass = 1;
	
	private CharacterController characterController;
	private bool _canStandUp = true;

	void Awake()
	{
		characterController = GetComponent<CharacterController>();
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