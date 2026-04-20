using UnityEngine;
using UnityEngine.InputSystem;


public enum PlayerState { Grounded, OnWall, Airborne }

public class PlayerBehaviour : MonoBehaviour
{
    public CameraFollow cam;
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
    public AudioSource audioSource;
    public AudioClip jumpSound;
    public AudioClip boingSound;
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
            if (currentWall != null && currentWall.wallType == WallType.Checkpoint) {
                rb.AddForce(-Physics2D.gravity * rb.mass);
                rb.gravityScale = 1f;
                rb.linearVelocity = new Vector2(0f, 0f);
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
            audioSource.PlayOneShot(jumpSound);
        } else if (state == PlayerState.OnWall) {
            TriggerWallJump();
            audioSource.PlayOneShot(jumpSound);
        } else if (state == PlayerState.Airborne && hasAirJump) {
            isFacingRight = !isFacingRight;
            float xDirection = isFacingRight ? speed : -speed;
            rb.linearVelocity = new Vector2(xDirection, jumpForce);
            isHoldingJump = true;
            jumpHoldTimer = 0f;
            hasAirJump = false;
            audioSource.PlayOneShot(jumpSound);
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
                audioSource.PlayOneShot(boingSound);
                return;
            }

            if (wall != null && wall.wallType == WallType.Checkpoint) {
                cam.PurgeBelowCamera();
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
        float direction = 1f;        
        if (currentWall != null)
        {
            // Compare positions to determine which side the wall is on
            if (transform.position.x < currentWall.transform.position.x)
            {
                // wall is on the right → jump left
                direction = -1f;
                isFacingRight = false;
            }
            else
            {
                // wall is on the left → jump right
                direction = 1f;
                isFacingRight = true;
            }
        }

        rb.linearVelocity = new Vector2(direction * wallJumpForce, jumpForce);

        hasAirJump = true;
        isHoldingJump = true;
        jumpHoldTimer = 0f;
        state = PlayerState.Airborne;
    }
}
