using System.Collections;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Movement")]
    public float maxSpeed = 5.0f;
    public float jumpForce = 2.5f;
    private float fallSpeed, hinput, vinput;
    private Rigidbody rb;
    private Vector3 moveDirection, currentSpeed;
    public new Transform playerTransform;
    
    [Header("Ground check")]
    public LayerMask groundMask;
    public bool isGrounded, doubleJump;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }
    
    void Update()
    {
        hinput = Input.GetAxis("Horizontal");
        vinput = Input.GetAxis("Vertical");  //raycast pra ver se encosta no chao \ maxDistance=altura/2
        isGrounded = Physics.Raycast(playerTransform.position, Vector3.down, 1.05f, groundMask);
        
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            isGrounded = false; 
            doubleJump = true;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
        
        if (doubleJump && !isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            doubleJump = false;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
        
        if (!isGrounded && (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.LeftAlt)))
        {
            rb.AddForce(Vector3.down * (3 * jumpForce), ForceMode.Impulse);
            StartCoroutine(JumpHigher());
        }
        
        //input.normalize \ salva a V(y) \ "acelera" o player limita a velocidade pra maxSpeed \ restaura o V(y)
        moveDirection= (playerTransform.right * hinput + playerTransform.forward * vinput).normalized;
        fallSpeed = rb.velocity.y;
        currentSpeed = rb.velocity;
        currentSpeed.y = 0;
        currentSpeed += moveDirection * maxSpeed /5;
        currentSpeed = Vector3.ClampMagnitude(currentSpeed, maxSpeed);
        currentSpeed.y = fallSpeed;
        rb.velocity = currentSpeed;
    }
    
    IEnumerator JumpHigher()
    {
        jumpForce = 4f;
        yield return new WaitForSecondsRealtime(0.5f);
        jumpForce = 2.5f;
    }
}
