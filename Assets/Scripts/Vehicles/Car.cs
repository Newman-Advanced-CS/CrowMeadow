using UnityEngine;

public class Car : VehicleBase
{
    [Header("Car Parameters")]
    public float speed = 50;
    public float downforce = 10;
    bool grounded = true;

    public override void ActiveUpdate()
    {
        base.ActiveUpdate();

        // Check if grounded

        // Layer mask the raycast
        int layerMask = 1 << 10;
        layerMask = ~layerMask;

        grounded = Physics.Raycast(transform.position, -transform.up, 2, layerMask);

        // Get input
        float xInput = Input.GetAxisRaw("Horizontal");
        float yInput = -Input.GetAxisRaw("Vertical");

        if(grounded)
        {
            rb.AddForce(transform.forward * yInput * speed);

            // Rotation
            transform.eulerAngles += new Vector3(0, xInput, 0);
        }

        // Add downforce
        rb.AddForce(-transform.up * downforce);

        // Clamp velocity
        if (rb.velocity.magnitude > 10)
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, 10);
    }
}
