using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtoControl : MonoBehaviour
{
	[SerializeField] private float acceleration = 55;
	[SerializeField] private float rotationSpeed = 60;
	[SerializeField] private float driftRotationSpeed = 80;

	[SerializeField] private GameObject rightPivot;
	[SerializeField] private GameObject leftPivot;

	private Rigidbody myBody;

	private float currentSpeed;

	[SerializeField] private float driftBonusSpeed = 1;

    private Vector3 turnAxis = new Vector3(0, 1, 0);

	private bool waitingDriftDir = false;
	private float waitDriftDirTimer = 0.0f;
	[SerializeField] private float waitDriftDirTime = 0.50f;

	private float driftCharge = 0.0f;
	[SerializeField] private float tier1DriftCharge = 130.0f;
	[SerializeField] private float tier1DriftBoost = 30.0f;
	[SerializeField] private float tier1DriftBoostDuration = 1.0f;
	private float tier1DriftBoostTimer = 0.0f;
	private bool tier1DriftBoostOn = false;


	[SerializeField] private float driftBurstRotation = 10.0f;

	enum Acceleration { Forward, Null, Backward };

	Acceleration myAcceleration = Acceleration.Null;

	enum DriftState { Right, Null, Left };

	DriftState myDriftState = DriftState.Null;

	void Start()
    {
		myBody = GetComponent<Rigidbody>();
	}


    void Update()
    {
		float driftAddCharge = 0.0f;

		switch (myAcceleration)
		{
			case Acceleration.Null:
				myDriftState = DriftState.Null;
				driftCharge = 0.0f;
				if (Input.GetButton("Forward"))
				{
					myAcceleration = Acceleration.Forward;
					break;
				}
				if (Input.GetButton("Backward"))
				{
					myAcceleration = Acceleration.Backward;
					break;
				}
				if (Input.GetAxis("Horizontal") < 0)
				{
					RotateCenterRight(-rotationSpeed);
				}
				if (Input.GetAxis("Horizontal") > 0)
				{
					RotateCenterRight(rotationSpeed);
				}
				break;

			case Acceleration.Forward:
				if (!Input.GetButton("Forward"))
				{
					myAcceleration = Acceleration.Null;
					break;
				}

				if (Input.GetAxis("Horizontal") < 0)
				{
					RotateLeft(rotationSpeed);
					driftAddCharge += -rotationSpeed * Time.deltaTime;
				}
				if (Input.GetAxis("Horizontal") > 0)
				{
					driftAddCharge += rotationSpeed * Time.deltaTime;
					RotateRight(rotationSpeed);
				}

				switch (myDriftState)
				{
					case DriftState.Null:
						if (Input.GetButtonDown("Backward") && !waitingDriftDir)
						{
                            // TODO
                            // Jump
                            driftBonusSpeed = 1;
                            waitingDriftDir = true;
                        }
						break;

					case DriftState.Right:
						if (!Input.GetButton("Backward"))
						{
                            if (Mathf.Abs(driftCharge) >= tier1DriftCharge)
							{
								tier1DriftBoostOn = true;
                            }
							driftCharge = 0.0f;
							myDriftState = DriftState.Null;
							break;
						}
						RotateCenterRight(driftRotationSpeed);
						driftAddCharge += driftRotationSpeed * Time.deltaTime;
						driftCharge += driftAddCharge;
						break;

					case DriftState.Left:
						if (!Input.GetButton("Backward"))
						{
                            if (Mathf.Abs(driftCharge) >= tier1DriftCharge)
							{
								tier1DriftBoostOn = true;
                            }
							driftCharge = 0.0f;
							myDriftState = DriftState.Null;
							break;
						}
						RotateCenterRight(-driftRotationSpeed);
						driftAddCharge += -driftRotationSpeed * Time.deltaTime;
						driftCharge += driftAddCharge;
						break;
				}
				
				myBody.velocity += gameObject.transform.up * acceleration * driftBonusSpeed * Time.deltaTime;
                driftBonusSpeed = 1;
                break;

			case Acceleration.Backward:
				if (!Input.GetButton("Backward"))
				{
					Debug.Log("Should stop going backward");
					myAcceleration = Acceleration.Null;
					break;
				}
				if (Input.GetAxis("Horizontal") < 0)
				{
					RotateLeft(rotationSpeed);
				}
				if (Input.GetAxis("Horizontal") > 0)
				{
					RotateRight(rotationSpeed);
				}
				myBody.velocity += -gameObject.transform.up * acceleration * Time.deltaTime;
                driftBonusSpeed = 1;
                break;
		}

		if (waitingDriftDir)
		{
			if (waitDriftDirTimer >= waitDriftDirTime)
			{
				waitDriftDirTimer = 0.0f;
				waitingDriftDir = false;
			}
			else
			{
				if (Input.GetAxis("Horizontal") < 0)
				{
					myDriftState = DriftState.Left;
					waitDriftDirTimer = 0.0f;
					waitingDriftDir = false;
					RotateCenterRight(-driftBurstRotation);
				}
				else
				{
					if (Input.GetAxis("Horizontal") > 0)
					{
						myDriftState = DriftState.Right;
						waitDriftDirTimer = 0.0f;
						waitingDriftDir = false;
						RotateCenterRight(driftBurstRotation * 1/Time.deltaTime);
					}
					else
					{
						waitDriftDirTimer += Time.deltaTime;
					}
				}
			}
		}

		if (tier1DriftBoostOn)
		{
			if (tier1DriftBoostTimer >= tier1DriftBoostDuration)
			{
				tier1DriftBoostOn = false;
				tier1DriftBoostTimer = 0.0f;
			}
			else
			{
				myBody.velocity += gameObject.transform.up * tier1DriftBoost * Time.deltaTime;
				tier1DriftBoostTimer += Time.deltaTime;
			}
		}
	}

	void RotateRight(float rotationSpeed)
	{
		gameObject.transform.RotateAround(rightPivot.transform.position, turnAxis, rotationSpeed * Time.deltaTime);
	}

	void RotateLeft(float rotationSpeed)
	{
		gameObject.transform.RotateAround(leftPivot.transform.position, -turnAxis, rotationSpeed * Time.deltaTime);
	}

	void RotateCenterRight(float rotationSpeed)
	{
		gameObject.transform.RotateAround(gameObject.transform.position, turnAxis, rotationSpeed * Time.deltaTime);
	}
}
