using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMove : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 7f;
    public float groundDrag = 5f;
    public float jumpForce = 12f;
    public float jumpCooldown = 0.25f;
    public float airMultiplier = 0.4f;

    [Header("Ground Check")]
    public float playerHeight = 2f;
    public LayerMask whatIsGround;
    private bool grounded;

    [Header("References")]
    public Transform orientation;
    private Rigidbody rb;
    private Transform mainCam;

    private float horizontalInput;
    private float verticalInput;
    private bool readyToJump = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        mainCam = Camera.main.transform;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput();
        SpeedControl();
        UpdateOrientation();

        rb.linearDamping = grounded ? groundDrag : 0f;
    }

    void FixedUpdate()
    {
        MovePlayer();
        HandleRotation();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(KeyCode.Space) && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MovePlayer()
    {
        Vector3 moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        float finalSpeed = moveSpeed * PlayerSettings.currentSpeedMultiplier;

        if (grounded)
            rb.AddForce(moveDirection.normalized * finalSpeed * 10f, ForceMode.Force);
        else
            rb.AddForce(moveDirection.normalized * finalSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    private void HandleRotation()
    {
        Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;
        if (inputDir.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(inputDir.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
        }
    }

    private void UpdateOrientation()
    {
        Vector3 viewDir = transform.position - new Vector3(mainCam.position.x, transform.position.y, mainCam.position.z);
        orientation.forward = viewDir.normalized;
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump() => readyToJump = true;

    public void TogglePhysics(bool pause)
    {
        rb.isKinematic = pause;
    }
}