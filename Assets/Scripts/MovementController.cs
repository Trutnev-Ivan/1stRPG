using UnityEngine;

[RequireComponent(typeof(CameraController))]
public class MovementController : MonoBehaviour
{
	private float speed = 400f;
	private CameraController cameraController;
	[SerializeField] private Rigidbody PlayerMesh;
	
	void Start()
	{
		cameraController = GetComponent <CameraController>();
	}
	
	private void FixedUpdate()
	{
		Vector3 movementOffset = new Vector3(Input.GetAxis("Vertical"), 0, 0);
		Vector3 rotationOffset = new Vector3(0, Input.GetAxis("Horizontal"), 0);
		
		PlayerMesh.velocity = movementOffset * Time.deltaTime * speed;
		PlayerMesh.rotation *= Quaternion.Euler(rotationOffset * Time.deltaTime * speed);

		cameraController.setPosition(PlayerMesh.transform.localPosition);
	}
}