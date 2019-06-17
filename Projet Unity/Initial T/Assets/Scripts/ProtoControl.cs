using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtoControl : MonoBehaviour
{
	[SerializeField] private float acceleration;
	[SerializeField] private float rotationSpeed;
	[SerializeField] private float driftRotationSpeed;

	[SerializeField] private GameObject rightPivot;
	[SerializeField] private GameObject leftPivot; 
    [SerializeField] private TrailRenderer leftTrail;
    [SerializeField] private TrailRenderer rightTrail;

    private bool accelerating = false;

	private Rigidbody myBody;

	private float currentSpeed;

    private Vector3 turnAxis = new Vector3(0, 1, 0);

	private bool waitingDriftDir = false;
	private float waitDriftDirTimer = 0.0f;
    [SerializeField] private float trailDelay = 2.0f;
    private float startTime = 0.0f;
    private bool increaseTime = false;
    [SerializeField] private float waitDriftDirTime;

	private float driftCharge = 0.0f;
	[SerializeField] private float tier1DriftCharge = 130.0f;
	[SerializeField] private float tier1DriftBoost = 30.0f;
	[SerializeField] private float tier1DriftBoostDuration = 1.0f;
	private float tier1DriftBoostTimer = 0.0f;
	private bool tier1DriftBoostOn = false;

	enum Acceleration { Forward, Null, Backward };

	Acceleration myAcceleration = Acceleration.Null;

	enum DriftState { Right, Null, Left };

	DriftState myDriftState = DriftState.Null;

	void Start()
    {
		myBody = GetComponent<Rigidbody>();
        leftTrail.emitting = false;
        rightTrail.emitting = false;
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
                            acceleration = 80.0f;
                            StartCoroutine("Deceleration");
                            waitingDriftDir = true;
                            leftTrail.emitting = true;
                            rightTrail.emitting = true;
                        }
						break;

					case DriftState.Right:
						if (!Input.GetButton("Backward"))
						{
                            startTime = 0.0f;
                            increaseTime = true;
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
                            startTime = 0.0f;
                            increaseTime = true;
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
				myBody.velocity += gameObject.transform.up * acceleration * Time.deltaTime;
                break;

			case Acceleration.Backward:
				if (!Input.GetButton("Backward"))
				{
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
				}
				else
				{
					if (Input.GetAxis("Horizontal") > 0)
					{
						myDriftState = DriftState.Right;
						waitDriftDirTimer = 0.0f;
						waitingDriftDir = false;
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
			if (tier1DriftBoostTimer >= waitDriftDirTime)
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
        // Show the GameObjects when time exceeds delay.
        if (increaseTime)
        {
            startTime += Time.deltaTime;
            if (startTime > trailDelay)
            {
                leftTrail.emitting = false;
                rightTrail.emitting = false;
                increaseTime = false;
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

    IEnumerator Deceleration()
    {
        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(1.0f);
            acceleration -= 10.0f;
        }
        StopCoroutine("Deceleration");
    }
}
