using System;
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

	void Start()
	{
		Debug.Log(Physics.gravity);
		
		cameraController = GetComponent<CameraController>();
	}
	
	private void FixedUpdate()
	{
		float verticalMove = Input.GetAxisRaw("Vertical");
		float horizontalMove = Input.GetAxisRaw("Horizontal");

		Vector3 moveDirection = new Vector3(0, 0, 0);
		
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
				PlayerMesh.transform.forward = Vector3.MoveTowards(PlayerMesh.transform.forward, vec, Time.deltaTime * rotateSpeed45);
			}
			// move forward / back
			else
			{
				PlayerMesh.transform.forward = Vector3.MoveTowards(PlayerMesh.transform.forward, verticalMove * cameraController.getCameraForwardVector(), Time.deltaTime * rotateSpeedForwardBack);
			}

			moveDirection = PlayerMesh.transform.forward * Mathf.Abs(verticalMove) * Time.deltaTime * speed;
		}
		else if (horizontalMove != 0)
		{
			Vector3 cameraForward = cameraController.getCameraForwardVector();
			float k = horizontalMove * Time.deltaTime;

			PlayerMesh.transform.forward = Vector3.MoveTowards(PlayerMesh.transform.forward, new Vector3(k * cameraForward.z, 0, -k * cameraForward.x), Time.deltaTime * rotateSpeedHorizontal);

			moveDirection = PlayerMesh.transform.forward * Time.deltaTime * speed * Mathf.Abs(horizontalMove);
		}
		
		cameraController.setPosition(PlayerMesh.transform.localPosition);

		moveDirection.y = mass * Physics.gravity.y * Time.deltaTime;

		PlayerMesh.Move(moveDirection);
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