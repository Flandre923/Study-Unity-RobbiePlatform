using System.Numerics;
using UnityEngine;
using UnityEngine.UIElements;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D coll;
    
    [Header("移动参数")] public float speed = 8f;
    public float crouchSpeedDivisor = 3f;
    
    [Header("跳跃参数")] public float jumpForce = 6.3f;
    public float jumpHolder = 1.9f;
    public float jumpHoldDuration = 0.1f;
    public float crouchJumpBoost = 2.5f;
    public float hangingJumpForce = 15f;
    private float jumpTime;
    
    [Header("状态")] public bool isCrouch;
    public bool isOnGround;
    public bool isJump;
    public bool isHeadBlocked;
    public bool isHanging;
 
    
    [Header("环境检测")] public LayerMask groundLayer;
    public float footOffset = 0.4f;
    public float headClearance = 0.5f;
    public float groundDistance = 0.2f;
    public float playerHeight;
    public float eyeHeight  =1.5f;
    public float grabDistance = 0.4f;
    public float reachOffset = 0.7f;
    
    
    public float xVelocity;

    // 按键设置
    private bool jumpPressed;
    private bool jumpHeld;
    private bool crouchHeld;
    private bool crouchPressed;
    
    
    private Vector2 colliderStanSize;
    private Vector2 colliderStandOffset;
    private Vector2 colliderCrouchSize;
    private Vector2 colliderCrouchOffset;
    
    void Start()
    { 
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();

        playerHeight = coll.size.y;

        colliderStanSize = coll.size;
        colliderStandOffset = coll.offset;
        colliderCrouchSize = new Vector2(coll.size.x, coll.size.y/2f);
        colliderCrouchOffset = new Vector2(coll.offset.x, coll.offset.y/2f);
    }

    void Update()
    {
        if (GameManager.GameOver())
            return;
        if (!jumpPressed)
        {
            jumpPressed = Input.GetButtonDown("Jump");
        }
        jumpHeld = Input.GetButton("Jump");
        crouchHeld = Input.GetButton("Crouch");
        crouchPressed = Input.GetButtonDown("Crouch");  
        
    }


    private void FixedUpdate()
    {
        if (GameManager.GameOver())
            return;
        PhysicsCheck();
        GroundMovement();
        MidAirMovement();
    }

    void PhysicsCheck()
    {
        RaycastHit2D leftCheck = RayCast(new Vector2(-footOffset, 0f), Vector2.down, groundDistance, groundLayer);
        RaycastHit2D rightCheck = RayCast(new Vector2(footOffset, 0f), Vector2.down, groundDistance, groundLayer);
        
        if (leftCheck || rightCheck)
            isOnGround = true;
        else
            isOnGround = false;

        RaycastHit2D headCheck = RayCast(new Vector2(0f, coll.size.y), Vector2.up, headClearance, groundLayer);

        if (headCheck)
            isHeadBlocked = true;
        else isHeadBlocked = false;

        float direction = transform.localScale.x;
        Vector2 grabDir = new Vector2(direction, 0f);

        RaycastHit2D blockedCheck = RayCast(new Vector2(footOffset * direction, playerHeight), grabDir, grabDistance,
            groundLayer);
        RaycastHit2D wallCheck = RayCast(new Vector2(footOffset * direction, eyeHeight), grabDir, grabDistance,
            groundLayer);
        RaycastHit2D ledgeCheck = RayCast(new Vector2(reachOffset * direction, playerHeight), Vector2.down,
            grabDistance, groundLayer);

        if(!isOnGround && rb.linearVelocity.y < 0f &&  ledgeCheck && wallCheck && !blockedCheck)
        {
            Vector3 pos = transform.position;
            pos.x += (wallCheck.distance - 0.05f) * direction;
            pos.y -= ledgeCheck.distance;
            transform.position = pos;
            rb.bodyType = RigidbodyType2D.Static;
            isHanging = true;
        }
    }

    void GroundMovement()
    {
        if (isHanging)
            return;
            
        if(crouchHeld && !isCrouch && isOnGround )
            Crouch();
        else if(!crouchHeld && isCrouch && !isHeadBlocked)
            StandUp();
        else if(!isOnGround && isCrouch)
            StandUp();
            
        xVelocity = Input.GetAxis("Horizontal");
        if(isCrouch)
            xVelocity /= crouchSpeedDivisor;
        rb.linearVelocity = new Vector2(xVelocity * speed, rb.linearVelocity.y);
        FlipDirection();
    }

    void MidAirMovement()
    {
        if (isHanging)
        {
            if (jumpPressed)
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, hangingJumpForce);
                isHanging = false;
            }

            if (crouchHeld)
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
                isHanging = false;
            }
            
        }
        if (jumpPressed && isOnGround && !isJump && !isHeadBlocked)
        {
            if (isCrouch)
            {
                StandUp();
                rb.AddForce(new Vector2(0f,crouchJumpBoost),ForceMode2D.Impulse);
            }
            isJump = true;
            jumpTime = Time.time + jumpHoldDuration;


            rb.AddForce(new Vector2(0f,jumpForce),ForceMode2D.Impulse);
            jumpPressed = false;
            isOnGround=false;
            
            AudioManager.PlayJumpAudio();
        }
        else if (isJump)
        {
            if(jumpHeld)
                rb.AddForce(new Vector2(0f,jumpHolder),ForceMode2D.Impulse);
            if (jumpTime < Time.time)
                isJump = false;
        }
    }
    
    void FlipDirection()
    {
        if (xVelocity < 0)
            transform.localScale = new Vector3(-1, 1,1);
        if (xVelocity > 0)
            transform.localScale = new Vector3(1, 1,1);   
    }

    void Crouch()
    {
        isCrouch = true;
        coll.size =  colliderCrouchSize;
        coll.offset = colliderCrouchOffset;
    }

    void StandUp()
    {
        isCrouch = false;
        coll.size =  colliderStanSize;
        coll.offset = colliderStandOffset;
    }

    RaycastHit2D RayCast(Vector2 offset, Vector2 rayDireciton, float Length, LayerMask layer)
    {
        Vector2 pos = transform.position;
        RaycastHit2D hit = Physics2D.Raycast(pos + offset, rayDireciton, Length, layer);
        Color color = hit ? Color.red : Color.green;
        Debug.DrawRay(pos+offset,rayDireciton*Length,color);
        return hit;
    }
}
