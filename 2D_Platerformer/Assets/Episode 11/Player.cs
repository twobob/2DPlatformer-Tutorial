using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Controller2D))]
public class Player : MonoBehaviour {

//    public ButtonNames buttonNames;

    public float maxJumpHeight = 4;
    public float minJumpHeight = 1;
    public float timeToJumpApex = .4f;
    float accelerationTimeAirborne = .2f;
    float accelerationTimeGrounded = .1f;
    float moveSpeed = 6;

    public Vector2 wallJumpClimb;
    public Vector2 wallJumpOff;
    public Vector2 wallLeap;

    public float wallSlideSpeedMax = 3;
    public float wallStickTime = .25f;
    float timeToWallUnstick;

    float gravity;
    float maxJumpVelocity;
    float minJumpVelocity;
    Vector3 velocity;
    float velocityXSmoothing;

    private float DownPressPrevTime = -1000f;
    private float DownPressFTP = 0.5f;

    private Vector3 AdditionalVelocity;
    public float AV_decelerationTimeGrounded;
    public float AV_decelerationTimeAirborne;

    public bool CanDash =  true;
    public string DashButton = "Fire1";
    public float DashSpeed = 12;
    private float DashPressPrevTime = -1000;
    private float DashPress = 0.5f;
    public float DashDuration = 0.25f;
    public bool IsDashing = false;

    public int AirBorneJumpCnt = 0;
    public int AirBorneMaxJumpCnt = 0;

    Controller2D controller;

//    [System.Serializable]
//    public struct ButtonNames
//    {
//        public string Horizontal;// = "Horizontal";
//        public string Vertical;// = "Vertical";
//        public string Dash;// = "";
//        public string Jump;// = "Jump";
//    }

    void Start()
    {
        controller = GetComponent<Controller2D> ();

        gravity = -(2 * maxJumpHeight) / Mathf.Pow (timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt (2 * Mathf.Abs (gravity) * minJumpHeight);
        print ("Gravity: " + gravity + "  Jump Velocity: " + maxJumpVelocity);
    }

    void Update()
    {
        Vector2 input = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
        int wallDirX = (controller.collisions.left) ? -1 : 1;

        float Speed = (IsDashing) ? DashSpeed : moveSpeed;
        float targetVelocityX = input.x * Speed;

        if (IsDashing && Mathf.Abs(input.x) != 1)
        {
            targetVelocityX = DashSpeed * controller.collisions.faceDir;
        }

        velocity.x = Mathf.SmoothDamp (velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below)?accelerationTimeGrounded:accelerationTimeAirborne);

        bool wallSliding = false;
        if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0)
        {
            wallSliding = true;

            if (velocity.y < -wallSlideSpeedMax)
            {
                velocity.y = -wallSlideSpeedMax;
            }

            if (timeToWallUnstick > 0)
            {
                velocityXSmoothing = 0;
                velocity.x = 0;

                if (input.x != wallDirX && input.x != 0)
                {
                    timeToWallUnstick -= Time.deltaTime;
                }
                else
                {
                    timeToWallUnstick = wallStickTime;
                }
            }
            else
            {
                timeToWallUnstick = wallStickTime;
            }

        }

        if (CanDash)
        {
            if (
                (Input.GetKeyDown(KeyCode.RightArrow) && controller.collisions.faceDir == 1)
                || (Input.GetButtonDown(DashButton))
                )
            {
                if (Time.time - DashPressPrevTime <= DashPress)
                {
                    IsDashing = true;
                    Invoke("ResetIsDashing", DashDuration);
                }

                DashPressPrevTime = Time.time;
            }

            if (
                (Input.GetKeyDown(KeyCode.LeftArrow) && controller.collisions.faceDir == -1)
                || (Input.GetButtonDown(DashButton))
                )
            {
                if (Time.time - DashPressPrevTime <= DashPress)
                {
                    IsDashing = true;
                    Invoke("ResetIsDashing", DashDuration);
                }

                DashPressPrevTime = Time.time;
            }
        }
            
        bool fallingThroughPlatform = false;

        if (controller.collisions.below && controller.collisions.fallingThroughPlatformBelow)
        {
            if (Input.GetKey(KeyCode.DownArrow) && Input.GetKeyDown(KeyCode.Space))
            {
                controller.fallThroughPlatform();
                fallingThroughPlatform = true;
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (Time.time - DownPressPrevTime <= DownPressFTP)
                {
                    controller.fallThroughPlatform();
                }

                DownPressPrevTime = Time.time;

                fallingThroughPlatform = true;
            }
        }


        if (Input.GetButtonDown ("Jump"))
        {
            if (wallSliding)
            {
                if (wallDirX == input.x)
                {
                    velocity.x = -wallDirX * wallJumpClimb.x;
                    velocity.y = wallJumpClimb.y;
                }
                else if (input.x == 0)
                {
                    velocity.x = -wallDirX * wallJumpOff.x;
                    velocity.y = wallJumpOff.y;
                }
                else
                {
                    velocity.x = -wallDirX * wallLeap.x;
                    velocity.y = wallLeap.y;
                }
            }

            if (controller.collisions.below && !fallingThroughPlatform )
            {
                velocity.y = maxJumpVelocity;
            }

            if (!controller.collisions.below && !wallSliding)
            {
                if (AirBorneJumpCnt < AirBorneMaxJumpCnt)
                {
                    velocity.y = maxJumpVelocity;
                    AirBorneJumpCnt++;
                }
            }
        }

        if (Input.GetButtonUp ("Jump"))
        {
            if (velocity.y > minJumpVelocity)
            {
                velocity.y = minJumpVelocity;
            }
        }

        velocity.y += gravity * Time.deltaTime;

        velocity += AdditionalVelocity;

        controller.Move (velocity * Time.deltaTime, input);

        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }

        if (controller.collisions.below)
        {
            AirBorneJumpCnt = 0;
        }

        AdditionalVelocity.x = Mathf.Lerp(AdditionalVelocity.x, 0f, (controller.collisions.below) ? AV_decelerationTimeGrounded : AV_decelerationTimeAirborne);
        AdditionalVelocity.y = Mathf.Lerp(AdditionalVelocity.y, 0f, (controller.collisions.below) ? AV_decelerationTimeGrounded : AV_decelerationTimeAirborne);

    }

    void AddForce(Vector3 Force)
    {
        AdditionalVelocity += Force;
    }

    void AddForce(Vector2 Force)
    {
//        AdditionalVelocity += Force;
    }

    void AddForce(float Degree, float Force)
    {
        //new Vector2 F = GetVect....()

        //AdditionalVelocity += F;
    }

    void ResetIsDashing()
    {
        IsDashing = false;
    }
}
