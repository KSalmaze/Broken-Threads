using System.Collections;
using UnityEditor;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Movement")]
    private bool isDashing;
    public float maxSpeed = 10f;
    private float currentMaxSpeed = 10f;
    public float jumpForce = 7;
    private float currentJumpForce = 7;
    private float fallSpeed, hinput, vinput;
    private Rigidbody rb;
    private Vector3 moveDirection, currentSpeed;
    public Transform playerTransform;

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
        isGrounded = Physics.Raycast(playerTransform.position, Vector3.down, 1.05f, groundMask);

        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing) StartCoroutine(Dash());

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
        currentSpeed *= 0.75f; //deixa o movimento mais controlavel
        currentSpeed += moveDirection * currentMaxSpeed /5;
        currentSpeed = Vector3.ClampMagnitude(currentSpeed, currentMaxSpeed);
        currentSpeed.y = fallSpeed;
        rb.linearVelocity = currentSpeed;
    }

    IEnumerator JumpHigher()
    {
        currentJumpForce = 1.5f*jumpForce;
        yield return new WaitForSecondsRealtime(0.35f);
        currentJumpForce = jumpForce;
    }

    IEnumerator Dash()
    {
        isDashing = true;
        currentMaxSpeed = 5*maxSpeed;
        Vector3 forceDirection = currentSpeed.normalized * currentMaxSpeed;
        forceDirection.y = 0;
        rb.AddForce(forceDirection, ForceMode.Impulse);
        while (currentMaxSpeed > maxSpeed)
        {
            currentMaxSpeed -= 3f;
            yield return null;
        }
        yield return new WaitForSeconds(1.5f); //cooldown do dash
        isDashing = false;
    }
}
