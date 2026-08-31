using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;

    [Header("Coyote Time")]
    [SerializeField] private float coyoteTime;
    private float coyoteCounter;

    [Header("Multiple Jumps")]
    [SerializeField] private int maxJumps;
    private int jumpCounter;

    [Header("Wall Jump")]
    [SerializeField] private float wallJumpX;
    [SerializeField] private float wallJumpY;
    [SerializeField] private int maxWallJumps;
    [SerializeField] private float wallSlideSpeed;
    private int wallJumpCounter;

    [Header("Layers")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;

    [Header("Movement Sound")]
    [SerializeField] private AudioClip jumpSound;
    private Rigidbody2D body;
    private Animator anim;
    private BoxCollider2D boxCollider;
    private float wallJumpCooldown;
    private float horizontalInput;


    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        wallJumpCooldown = 1f;
    }

    private void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        if (TouchInputManager.instance != null && TouchInputManager.instance.GetHorizontal() != 0)
            horizontalInput = TouchInputManager.instance.GetHorizontal();

        //flip player when moving left or right
        if (horizontalInput > 0.01f)
            transform.localScale = Vector3.one;
        else if (horizontalInput < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);

        //set animator parameters
        anim.SetBool("run", horizontalInput != 0);
        anim.SetBool("grounded", isGrounded());

        //Jump
        if (Input.GetKeyDown(KeyCode.Space) || (TouchInputManager.instance != null && TouchInputManager.instance.jumpPressed))
        {
            Jump();
            if (TouchInputManager.instance != null)
                TouchInputManager.instance.ConsumeJump();
        }

        //Adjustable Jump Height (works for both keyboard release and touch release)
        bool touchJumpReleased = TouchInputManager.instance != null && TouchInputManager.instance.jumpReleased;

        if ((Input.GetKeyUp(KeyCode.Space) || touchJumpReleased) && body.linearVelocity.y > 0)
            body.linearVelocity = new Vector2(body.linearVelocity.x, body.linearVelocity.y * 0.5f);

        if (touchJumpReleased)
            TouchInputManager.instance.ConsumeJumpReleased();

        bool touchingWall = onWall() && !isGrounded();

        if (touchingWall && wallJumpCounter > 0)
        {
            body.gravityScale = 0;
            body.linearVelocity = Vector2.zero;
        }
        else if (touchingWall && wallJumpCounter <= 0)
        {
            body.gravityScale = 7;
            float clampedFallSpeed = Mathf.Max(body.linearVelocity.y, -wallSlideSpeed);
            body.linearVelocity = new Vector2(horizontalInput * speed, clampedFallSpeed);
        }
        else
        {
            body.gravityScale = 7;
            body.linearVelocity = new Vector2(horizontalInput * speed, body.linearVelocity.y);

            if (isGrounded())
            {
                coyoteCounter = coyoteTime;
                jumpCounter = maxJumps;
                wallJumpCounter = maxWallJumps;
            }
            else
            {
                coyoteCounter -= Time.deltaTime;
            }
        }
    }

    private void Jump()
    {
        if (coyoteCounter <= 0 && !onWall() && jumpCounter <= 0) return;

        if (onWall() && !isGrounded())
        {
            if (wallJumpCounter <= 0) return;

            SoundManager.instance.PlaySound(jumpSound);
            wallJump();
        }
        else
        {
            SoundManager.instance.PlaySound(jumpSound);

            if (isGrounded())
                body.linearVelocity = new Vector2(body.linearVelocity.x, jumpPower);
            else
            {
                if (coyoteCounter > 0)
                    body.linearVelocity = new Vector2(body.linearVelocity.x, jumpPower);
                else
                {
                    if (jumpCounter > 0)
                    {
                        body.linearVelocity = new Vector2(body.linearVelocity.x, jumpPower);
                        jumpCounter--;
                    }
                }
            }
            coyoteCounter = 0;
        }
    }

    private void wallJump()
    {
        body.AddForce(new Vector2(-Mathf.Sign(transform.localScale.x) * wallJumpX, wallJumpY));
        wallJumpCooldown = 0;
        wallJumpCounter--;
    }

    private bool isGrounded()
    {
        RaycastHit2D raycasthit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down, .1f, groundLayer);
        return raycasthit.collider != null;
    }

    private bool onWall()
    {
        RaycastHit2D raycasthit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, new Vector2(transform.localScale.x, 0), .1f, wallLayer);
        return raycasthit.collider != null;
    }

    public bool canAttack()
    {
        return horizontalInput == 0 && isGrounded() && !onWall();
    }
}