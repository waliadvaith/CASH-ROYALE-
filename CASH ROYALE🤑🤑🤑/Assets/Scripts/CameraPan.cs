using UnityEngine;

public class CameraPan : MonoBehaviour
{
    [Header("Movement Settings")]
    public float panSpeed = 15f;      // How fast the camera moves when holding a key
    public float smoothSpeed = 5f;    // Higher = sharper stops, Lower = slipperier gliding

    [Header("Battlefield Boundaries")]
    public float minX = -5f;
    public float maxX = 25f;

    private float targetX;

    private void Start()
    {
        // Start our target exactly where the camera currently is placed
        targetX = transform.position.x;
    }

    private void Update()
    {
        // 1. Get keyboard input via standard Axis (A/D or Left/Right Arrow keys)
        float inputX = Input.GetAxisRaw("Horizontal");

        // 2. Move our invisible "target destination" based on input
        targetX += inputX * panSpeed * Time.deltaTime;

        // 3. Keep that target destination locked within our map boundaries
        targetX = Mathf.Clamp(targetX, minX, maxX);

        // 4. Smoothly blend (Lerp) the camera's current X position towards the target X position
        float smoothedX = Mathf.Lerp(transform.position.x, targetX, smoothSpeed * Time.deltaTime);

        // Apply the smoothed coordinate back to the camera transform
        transform.position = new Vector3(smoothedX, transform.position.y, transform.position.z);
    }
}