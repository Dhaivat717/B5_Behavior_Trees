using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{

	public float movementSpeed = 10f;
	public float freeLookSensitivity = 3f;
	public float zoomSensitivity = 10f;
	/// Set to true when free looking (on right mouse button).
	private bool looking = false;


	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKey(KeyCode.A))
			Camera.main.transform.position = Camera.main.transform.position + (-Camera.main.transform.right * movementSpeed * Time.deltaTime);

		if (Input.GetKey(KeyCode.D))
			Camera.main.transform.position = Camera.main.transform.position + (Camera.main.transform.right * movementSpeed * Time.deltaTime);

		if (Input.GetKey(KeyCode.W))
			Camera.main.transform.position = Camera.main.transform.position + (Camera.main.transform.forward * movementSpeed * Time.deltaTime);

		if (Input.GetKey(KeyCode.S))
			Camera.main.transform.position = Camera.main.transform.position + (-Camera.main.transform.forward * movementSpeed * Time.deltaTime);

		if (Input.GetKey(KeyCode.E))
			Camera.main.transform.position = Camera.main.transform.position + (Camera.main.transform.up * movementSpeed * Time.deltaTime);

		if (Input.GetKey(KeyCode.Q))
			Camera.main.transform.position = Camera.main.transform.position + (-Camera.main.transform.up * movementSpeed * Time.deltaTime);

		if (Input.GetKey(KeyCode.R))
			Camera.main.transform.position = Camera.main.transform.position + (Vector3.up * movementSpeed * Time.deltaTime);

		if (Input.GetKey(KeyCode.F))
			Camera.main.transform.position = Camera.main.transform.position + (-Vector3.up * movementSpeed * Time.deltaTime);

		if (looking)
		{
			float newRotationX = Camera.main.transform.localEulerAngles.y + Input.GetAxis("Mouse X") * freeLookSensitivity;
			float newRotationY = Camera.main.transform.localEulerAngles.x - Input.GetAxis("Mouse Y") * freeLookSensitivity;
			Camera.main.transform.localEulerAngles = new Vector3(newRotationY, newRotationX, 0f);
		}

		float axis = Input.GetAxis("Mouse ScrollWheel");
		if (axis != 0)
		{
			var zoomSensitivity = this.zoomSensitivity;
			Camera.main.transform.position = Camera.main.transform.position + Camera.main.transform.forward * axis * zoomSensitivity;
		}

		if (Input.GetKeyDown(KeyCode.Mouse1))
		{
			StartLooking();
		}
		else if (Input.GetKeyUp(KeyCode.Mouse1))
		{
			StopLooking();
		}
	}

	void OnDisable()
	{
		StopLooking();
	}

	public void StartLooking()
	{
		looking = true;
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
	}

	public void StopLooking()
	{
		looking = false;
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
	}
}

