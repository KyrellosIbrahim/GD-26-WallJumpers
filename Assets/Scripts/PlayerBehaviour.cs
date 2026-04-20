using UnityEngine;
using UnityEngine.InputSystem;


public enum PlayerState { Grounded, OnWall, Airborne }

public class PlayerBehaviour : MonoBehaviour
{
    public PlayerState state = PlayerState.OnWall;
    public bool isFacingRight = true;
    public bool hasAirJump = false;
    private float jumpForce = 7f;
    private float speed = 5f;
    private float wallJumpForce = 7f;
    private Rigidbody2D rb;
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
            rb.linearVelocity = new Vector2(0f, 0f); // Stick player to the wall
        }
    }

    public void HandleJump() {
        Keyboard k = Keyboard.current;
        if (!k.spaceKey.wasPressedThisFrame) return;

        if (state == PlayerState.Grounded) {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            hasAirJump = true;
            state = PlayerState.Airborne;
        } else if (state == PlayerState.OnWall) {
            isFacingRight = !isFacingRight;
            float xDirection = isFacingRight ? wallJumpForce : -wallJumpForce;
            rb.linearVelocity = new Vector2(xDirection, jumpForce);
            hasAirJump = true;
            state = PlayerState.Airborne;
        } else if (state == PlayerState.Airborne && hasAirJump) {
            isFacingRight = !isFacingRight;
            float xDirection = isFacingRight ? speed : -speed;
            rb.linearVelocity = new Vector2(xDirection, jumpForce);
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
        } else if (collision.gameObject.CompareTag("Wall")) {
            state = PlayerState.OnWall;
        }
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground") || col.gameObject.CompareTag("Wall"))
            state = PlayerState.Airborne;
    }
}
