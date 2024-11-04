using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Move : MonoBehaviour
{
    public float speed = 5f;           
    public float turnSpeed = 2f;      
    public float waterDrag = 0.1f;      
    public float buoyancyForce = 9.8f;  
    public float waterLevel = 0f;      
    public float maxSpeed = 10f;       

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true; 
    }

    void Update()
    {
        HandleMovement();
        HandleRotation();
    }

    void HandleMovement()
    {
        float moveInput = Input.GetAxis("Vertical");
        Vector3 forwardMovement = transform.forward * moveInput * speed;
        rb.AddForce(forwardMovement, ForceMode.Force);

        rb.velocity *= (1f - waterDrag * Time.deltaTime);

        if (transform.position.y < waterLevel)
        {
            Vector3 buoyancy = Vector3.up * buoyancyForce * (waterLevel - transform.position.y);
            rb.AddForce(buoyancy, ForceMode.Acceleration);
        }

        // Limit the maximum speed
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }

    void HandleRotation()
    {
        // Turning the boat
        float turnInput = Input.GetAxis("Horizontal"); 
        Vector3 turn = Vector3.up * turnInput * turnSpeed;
        Quaternion rotation = Quaternion.Euler(turn * Time.deltaTime);
        rb.MoveRotation(rb.rotation * rotation);
    }
}
