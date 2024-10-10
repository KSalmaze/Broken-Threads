using System.Collections;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Movement")]
    public float maxSpeed = 5.0f;
    public float jumpForce = 5f;
    private float currentJumpForce=5;
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
        doubleJump = true;
    }
    
    void Update()
    {
        hinput = Input.GetAxis("Horizontal");
        vinput = Input.GetAxis("Vertical");  //raycast pra ver se encosta no chao \ maxDistance=altura/2
        isGrounded = Physics.Raycast(playerTransform.position, Vector3.down, 1.1f, groundMask);
        rb.linearDamping = isGrounded ? 2f : 0f;
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                isGrounded = false;
                doubleJump = true;
                Debug.Log("pulo normal");
                rb.AddForce(Vector3.up * currentJumpForce, ForceMode.Impulse);
            }
            else if (doubleJump && !isGrounded)
            {
                doubleJump = false;
                rb.linearVelocity -= new Vector3(0f, rb.linearVelocity.y, 0f);
                Debug.Log("pulo duplo");
                rb.AddForce(Vector3.up * currentJumpForce, ForceMode.Impulse);
            }
        }
        
        if (!isGrounded && (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.LeftAlt)))
        {
            rb.linearVelocity -= new Vector3(0f, rb.linearVelocity.y, 0f);
            rb.AddForce(Vector3.down * (5 * jumpForce), ForceMode.Impulse);
            StartCoroutine(JumpHigher());
        }
        
        //input.normalize \ salva a V(y) \ "acelera" o player limita a velocidade pra maxSpeed \ restaura o V(y)
        moveDirection= (playerTransform.right * hinput + playerTransform.forward * vinput).normalized;
        fallSpeed = rb.linearVelocity.y;
        currentSpeed = rb.linearVelocity;
        currentSpeed.y = 0;
        currentSpeed += moveDirection * maxSpeed /5;
        currentSpeed = Vector3.ClampMagnitude(currentSpeed, maxSpeed);
        currentSpeed.y = fallSpeed;
        rb.linearVelocity = currentSpeed;
    }
    
    IEnumerator JumpHigher()
    {
        currentJumpForce = 2*jumpForce;
        yield return new WaitForSecondsRealtime(0.5f);
        currentJumpForce = jumpForce;
    }
}
