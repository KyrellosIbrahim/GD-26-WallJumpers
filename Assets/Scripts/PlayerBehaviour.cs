using UnityEngine;
using UnityEngine.InputSystem;


public enum PlayerState { Grounded, OnWall, Airborne }

public class PlayerBehaviour : MonoBehaviour
{
    public PlayerState state = PlayerState.OnWall;
    public bool isFacingRight = true;
    public bool hasAirJump = false;
    public float jumpForce = 5f;
    public float speed = 3f;
    public float wallJumpForce = 3f;
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
}
