using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffect : MonoBehaviour
{

    private bool HitState = false;
    private Vector3 turnAxis = new Vector3(0, 1, 0);
    [SerializeField] Transform body;
    [SerializeField] private float rotationSpeed;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            
            ThrowAwayEffect();
            HitState = true;
        }

        if (Input.GetKey("space"))
        {
            RotationEffect((-rotationSpeed) * 40);
        }
    }

    void RotationEffect(float rotationSpeed)
    {
       body.transform.RotateAround(body.transform.position, turnAxis, rotationSpeed * Time.deltaTime);
    }

    void ThrowAwayEffect()
    {
        if (!HitState)
        {
            body.transform.position = new Vector3(body.transform.position.x + 20, body.transform.position.y,
            body.transform.position.z + 10);
        }
    }
}
