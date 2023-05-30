using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class movement : MonoBehaviour
{
    #region Fields
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float coyoteTime = 0.2f;
    [SerializeField] private float bufferTime = 0.2f;
    [SerializeField] private bool canDoubleJump = false;
    [SerializeField] private float JumpCount = 2f;

    private Rigidbody2D rb;
    private bool isJumping = false;
    private bool isGrounded = false;
    private float CoyoteTimeCounter;
    private float bufferTimer;

    private Vector2 moveInput;
    #endregion

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (isGrounded)
        {
            canDoubleJump = true;
            JumpCount = 2f;
        }

        if (isJumping && bufferTimer > 0f)
        {
            Jump();
            bufferTimer = 0f;
        }
        else
        {
            bufferTimer -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        Move();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && (isGrounded || (canDoubleJump && !isJumping)) && JumpCount > 0f)
        {
            Jump();
        }
        else if (context.canceled && isJumping)
        {
            isJumping = false;
        }
        else if (context.performed && !isGrounded)
        {
            bufferTimer = bufferTime;
        }
    }

    private void Move()
    {
        float moveX = moveInput.x * moveSpeed * Time.fixedDeltaTime;
        rb.velocity = new Vector2(moveX, rb.velocity.y);
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        CoyoteTimeCounter = coyoteTime;
        isJumping = true;
        JumpCount--;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            isJumping = false;
            CoyoteTimeCounter = coyoteTime;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            CoyoteTimeCounter = 0f;
        }
    }
}
