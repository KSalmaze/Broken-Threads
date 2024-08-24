using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5.0f;
    public float jumpForce = 5.0f;
    private float hinput, vinput;
    private Rigidbody rb;
    private Vector3 moveDirection;
    public new Transform transform;
    
    [Header("Ground check")]
    public LayerMask groundMask;
    public bool isGrounded, doubleJump;
    
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }
    
    // Update is called once per frame
    void Update()
    {
        hinput = Input.GetAxis("Horizontal");
        vinput = Input.GetAxis("Vertical");
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 0.6f, groundMask);
        
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            doubleJump = true;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        if (doubleJump && !isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            doubleJump = false;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
    
    private void FixedUpdate()
    {
        moveDirection=transform.right * hinput + transform.forward * vinput;
        rb.AddForce(moveDirection.normalized * moveSpeed);
    }
}
