using UnityEngine;
using UnityEngine.InputSystem;


public enum PlayerState { Grounded, OnWall, Airborne }

public class PlayerBehaviour : MonoBehaviour
{
    public PlayerState state = PlayerState.OnWall;
    public bool isFacingRight = true;
    public bool hasAirJump = false;
    private bool isHoldingJump = false;
    private float jumpForce = 5f;
    private float speed = 5f;
    private float wallJumpForce = 5f;
    public float jumpHoldForce = 25f;      // additional upward force applied while space bar is held
    public float jumpHoldDuration = 0.2f;  // max seconds the hold is used
    private float jumpHoldTimer = 0f;
    private Rigidbody2D rb;
    private WallBehaviour currentWall = null;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleJump();
        HandleAutoWalk();
    }

    void FixedUpdate()
    {
        if (state == PlayerState.OnWall) {
            //rb.linearVelocity = new Vector2(0f, 0f); // Stick player to the wall
            if (currentWall != null && currentWall.wallType == WallType.Checkpoint) {
                rb.gravityScale = 0f;
                rb.linearVelocity = Vector2.zero; // Stop all movement on checkpoint walls
            } else {
                rb.gravityScale = 1f;
                rb.linearVelocity = new Vector2(0f, 0f);
            }
        }

        if (isHoldingJump) {
            if(jumpHoldTimer < jumpHoldDuration) {
                rb.AddForce(Vector2.up * jumpHoldForce, ForceMode2D.Force);
                jumpHoldTimer += Time.fixedDeltaTime;
            } else {
                isHoldingJump = false;
            }
        }
    }

    public void HandleJump() {
        Keyboard k = Keyboard.current;

        if (k.spaceKey.wasReleasedThisFrame) {
            isHoldingJump = false;
        }

        if (!k.spaceKey.wasPressedThisFrame) return;

        if (state == PlayerState.Grounded) {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            hasAirJump = true;
            isHoldingJump = true;
            jumpHoldTimer = 0f;
            state = PlayerState.Airborne;
        } else if (state == PlayerState.OnWall) {
            TriggerWallJump();
        } else if (state == PlayerState.Airborne && hasAirJump) {
            isFacingRight = !isFacingRight;
            float xDirection = isFacingRight ? speed : -speed;
            rb.linearVelocity = new Vector2(xDirection, jumpForce);
            isHoldingJump = true;
            jumpHoldTimer = 0f;
            hasAirJump = false;
        }
    }

    public void HandleAutoWalk() {
        if (state == PlayerState.Grounded) {
            float xDirection = isFacingRight ? speed : -speed;
            rb.linearVelocity = new Vector2(xDirection, rb.linearVelocity.y);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Ground")) {
            state = PlayerState.Grounded;
            isHoldingJump = false;
        } else if (collision.gameObject.CompareTag("Wall")) {
            WallBehaviour wall = collision.gameObject.GetComponent<WallBehaviour>();
            currentWall = wall;
            if (wall != null && wall.wallType == WallType.Bouncy) {
                TriggerWallJump();
                return;
            }
            state = PlayerState.OnWall;
            isHoldingJump = false;
        }
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground") || col.gameObject.CompareTag("Wall")) {
            state = PlayerState.Airborne;
            rb.gravityScale = 1f;
            currentWall = null;
        }
    }

    public void TriggerWallJump()
    {
        isFacingRight = !isFacingRight;
        float xDir = isFacingRight ? wallJumpForce : -wallJumpForce;
        rb.linearVelocity = new Vector2(xDir, jumpForce);
        hasAirJump = true;
        isHoldingJump = true;
        jumpHoldTimer = 0f;
        state = PlayerState.Airborne;
    }
}
