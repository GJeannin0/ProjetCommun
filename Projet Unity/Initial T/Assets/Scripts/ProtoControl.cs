using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtoControl : MonoBehaviour
{
	[SerializeField] float acceleration;
	[SerializeField] float rotationSpeed;

	[SerializeField] GameObject rightPivot;
	[SerializeField] GameObject leftPivot;

	private bool accelerating = false;

	private Rigidbody myBody;

	private float currentSpeed;

	private Vector3 turnAxis = new Vector3(0, 1, 0);

	// Start is called before the first frame update
	void Start()
    {
		myBody = GetComponent<Rigidbody>();
	}

    // Update is called once per frame
    void Update()
    {
		currentSpeed = myBody.velocity.magnitude;

		if (Input.GetButton("Jump"))
		{
			accelerating = true;
			myBody.velocity += -gameObject.transform.right * acceleration * Time.deltaTime;
		}
		else
		{
			accelerating = false;
		}

		if (Input.GetButton("Fire1"))
		{
			if (accelerating)
			{
				RotateLeft();
			}
			else
			{
				RotateCenterLeft();
			}
		}

		if (Input.GetButton("Fire2"))
		{
			if (accelerating)
			{
				RotateRight();
			}
			else
			{
				RotateCenterRight();
			}
		}
    }

	void RotateRight()
	{
		gameObject.transform.RotateAround(rightPivot.transform.position, turnAxis, rotationSpeed * Time.deltaTime);
	}

	void RotateLeft()
	{
		gameObject.transform.RotateAround(leftPivot.transform.position, -turnAxis, rotationSpeed * Time.deltaTime);
	}

	void RotateCenterRight()
	{
		gameObject.transform.RotateAround(gameObject.transform.position, turnAxis, rotationSpeed * Time.deltaTime);
	}

	void RotateCenterLeft()
	{
		gameObject.transform.RotateAround(gameObject.transform.position, -turnAxis, rotationSpeed * Time.deltaTime);
	}
}
