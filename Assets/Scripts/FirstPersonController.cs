using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    CursorLockMode lockMode;
    private Vector3 mouseInput;
    private Vector3 moveInput;
    private Camera cam;
    private Rigidbody rb;

    //Player Options
        [Tooltip ("Adjust mouse sensitivity")]
        [Range(0.0f, 200f)]
    [SerializeField] private float mouseSens = 10;
        [Tooltip ("Invert mouse Y axis")]
    [SerializeField] private bool invertPitch = false;
        [Tooltip("Adjust movement speed")]
        [Range(0.0f, 5f)]
    [SerializeField] private float playerSpeed = 0.5f;
        [Tooltip("Adjust drag. How quickly you slow down.")]
        [Range(0.0f, 500f)]
    [SerializeField] private float playerDrag = 1f;
        [Tooltip("How high the player can jump")]
        [Range(0f, 500f)]
    [SerializeField] private float jumpHeight = 10f;
        [Tooltip("Adjust the player's gravity")]
        [Range(0f, 100f)]
    [SerializeField] private float gravity = 1f;
        [Tooltip("Adjust the player's mass. This changes how gravity affects them.")]
        [Range(0f, 10f)]
    [SerializeField]private float mass = 1f;

    private void Awake()
        //locks the cursor inside of the unity window during play mode.
    {
        lockMode = CursorLockMode.Locked;
        Cursor.lockState = lockMode;
    }
    // Start is called before the first frame update
    void Start()
    {
        SanityCheck();
        StartCoroutine(DebugLog());
        rb.useGravity = false;
    }

    // Update is called once per frame
    void Update()
    {
        ControlInput();
    }
    void FixedUpdate()
        //movement and control functions are performed under fixedupdate
    {
        ReplaceGravity();
        CameraLook();
        PlayerMove();
        Jump();
    }
    void SanityCheck()
    {
        //defaults the mouse input vector.
        mouseInput = Vector3.zero;
        //checks for main camera.
        if (cam == null)
        {
            cam = Camera.main;
        }
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }        
    }
    void ControlInput()
        //Gets info and converts to Vector3. Contains clauses for inverted camera as well.
    {
        mouseInput.y += Input.GetAxis("Mouse X") * Time.fixedDeltaTime;
        if (invertPitch)
        {
            mouseInput.x += Input.GetAxis("Mouse Y") * Time.fixedDeltaTime;
        }
        if (!invertPitch)
        {
            mouseInput.x -= Input.GetAxis("Mouse Y") * Time.fixedDeltaTime;
        }

        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.z = Input.GetAxisRaw("Vertical");
    }
    void CameraLook()
    {
        //transform.eulerAngles rotates the camera along the x and y axes using mouseInput
        // contains a limiter of 75o to prevent gimbal lock
        cam.transform.eulerAngles = mouseInput * mouseSens;
        if (mouseInput.x > 1.75f)
        {
            mouseInput.x = 1.75f;
        }
        else if (mouseInput.x < -1.75f)
        {
            mouseInput.x = -1.75f;
        }
        rb.transform.eulerAngles = new Vector3(0, mouseInput.y, 0) * mouseSens;
    }
    void PlayerMove()
    {
        rb.mass = mass;
        rb.drag = playerDrag; //only in her for testing and debugging purposes. should not need to be called every frame
        rb.AddRelativeForce(moveInput * playerSpeed, ForceMode.VelocityChange);
    }
    void Jump()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            if (rb.velocity.y == 0)
            {
                Debug.Log("jump");
                rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
            }
        }
    }
    void ReplaceGravity()
    {
        rb.AddForce(Vector3.down * rb.mass * gravity);
    }
    IEnumerator DebugLog()
        //Debug tool to show various info related to inputs
    {
        bool x = false;
        while (true)
        {
            if (!x)
            {
                x = true;
                Debug.Log("Mouse input co-ordinates: " + mouseInput);
                Debug.Log("Current speed: " + rb.velocity.normalized);
                yield return new WaitForSeconds(2);
                x = false;
            }
        }
    }
}
