using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    public Vector3 offset = new Vector3(0, 5, -10); // offset of the camera from the player
    public float rotationSpeed = 5;

    private float pitch = 0; // vertical rotation angle
    private float yaw = 0;   // horizontal rotation angle

    [SerializeField]
    public float startingYaw = 0; // starting horizontal angle
    [SerializeField]
    public float startingPitch = 0; // starting vertical angle

    void Start()
    {
        // we initialize the camera offset if none is set
        if (offset == Vector3.zero)
        {
            offset = transform.position - player.position;
        }

        // we set the initial yaw and pitch to specified starting values
        yaw = startingYaw;
        pitch = startingPitch;
    }

    void LateUpdate() //we want to move the camera after the player moves
    {
        // we follow the player position with an offset
        Vector3 desiredPosition = player.position + offset;
        transform.position = desiredPosition;

        //if (Input.GetMouseButton(1)) // right mouse button is held down
        //{
        // we get the mouse input to rotate the camera around the player
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -30f, 60f); // we clamp the up/down rotation angle
                                               // to avoid extreme angles
                                               //}

        // apply rotation based on yaw and pitch
        // we use quaternion to have a smoother camera rotation
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        transform.position = player.position + rotation * offset;
        transform.LookAt(player);
    }
}