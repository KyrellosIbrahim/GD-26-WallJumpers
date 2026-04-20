using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerBehaviour : MonoBehaviour
{
    public bool isGrounded, isFacingRight, isMidAir, isTouchingWall;
    public float jumpForce = 5f;
    public float speed = 3f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Keyboard k = Keyboard.current;

        if (k.spaceKey.wasPressedThisFrame && isTouchingWall) {
            // TODO: jump from wall
            isFacingRight = !isFacingRight;
        }
        
    }
}
