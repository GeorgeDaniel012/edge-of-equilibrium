using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 800;
    public float rollSpeed = 50;

    private Rigidbody rb;
    public Transform cameraTransform; // reference to the camera transform, so that
                                      // the input direction corresponds with the camera
                                      //angle

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // input
        float moveHorizontal = Input.GetAxis("Horizontal"); // A/D or left/right arrow
        float moveVertical = Input.GetAxis("Vertical"); // W/S or up/down arrow

        // movement direction relative to the camera
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        // purely horizontal directions, parallel to the ground,
        // regardless of the camera’s tilt
        forward.y = 0;
        right.y = 0;

        // we ensure that the movement has a fixed speed
        forward.Normalize();
        right.Normalize();

        // movement direction relative to camera's forward and right
        Vector3 movement = (forward * moveVertical + right * moveHorizontal).normalized;

        // we apply force based on mass (ForceMode.Force)
        rb.AddForce(movement * moveSpeed * Time.fixedDeltaTime, ForceMode.Force);

        // Calculate rotation based on movement direction
        if (movement != Vector3.zero)
        {
            float rollAmount = rollSpeed * Time.fixedDeltaTime;
            Vector3 rollAxis = Vector3.Cross(Vector3.up, movement);
            rb.AddTorque(rollAxis * rollAmount, ForceMode.Force); // Apply rotation
        }
    }
}
