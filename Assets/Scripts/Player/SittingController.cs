using UnityEngine;

[RequireComponent(typeof(JumpingController))]
public class SittingController : MonoBehaviour
{
	[SerializeField] private float personSittingHeight = 10;
	[SerializeField] private PlayerController playerController;
	[SerializeField] private float colliderY = 1;
	
	private bool _isSitting;
	private bool isSittingPressing;
	private bool _needSitting;
	private bool _needStand;
	private bool needSittingKeyRelease;
	private float personHeight;
	private float _colliderY;
	
	private JumpingController jumpingController;

	void Start()
	{
		jumpingController = GetComponent<JumpingController>();
		personHeight = playerController.getHeight();
		_colliderY = playerController.ColliderY;
	}
	
	void Update()
	{
		checkSitting();
	}

	private void FixedUpdate()
	{
		applySitting();
	}
	
	private void checkSitting()
	{
		if (!needSittingKeyRelease && isAxisSittingPressed())
		{
			if (!_isSitting)
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

		if (_isSitting && jumpingController.isAxisJumpPressed())
		{
			_needStand = true;
		}

		isSittingPressing = isAxisSittingPressed();

		if (!isSittingPressing && needSittingKeyRelease)
		{
			needSittingKeyRelease = false;
		}
	}
	
	protected void sit()
	{
		playerController.setHeight(personSittingHeight);
		_isSitting = true;
		needSittingKeyRelease = true;
		playerController.ColliderY = colliderY;
	}

	protected void stand()
	{
		playerController.setHeight(personHeight);
		_isSitting = false;
		jumpingController.NeedJump = false;
		_needStand = false;
		playerController.ColliderY = _colliderY;
		
		if (isSittingPressing)
		{
			needSittingKeyRelease = true;
		}

		if (jumpingController.isAxisJumpPressed())
		{
			jumpingController.NeedJumpingKeyRelease = true;
		}
	}
	
	protected bool needSitting()
	{
		return playerController.isGrounded() && isSittingPressing && _needSitting;
	}

	protected bool needStand()
	{
		return playerController.isGrounded() && _needStand;
	}
	
	protected void applySitting()
	{
		if (needSitting())
		{
			sit();
		}
		else if (needStand() && playerController.canStandUp())
		{
			stand();
		}
	}
	
	public bool isNeedStand() => _needStand;

	public bool isSitting() => _isSitting;
	
	protected bool isAxisSittingPressed()
	{
		return Input.GetAxisRaw("Fire1") != 0;
	}
}