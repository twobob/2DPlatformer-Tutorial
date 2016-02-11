using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Controller2D))]
public class Player : MonoBehaviour {

    public ButtonNames buttonNames;

    public float maxJumpHeight = 4f;
    public float minJumpHeight = 1f;
    public float timeToJumpApex = .4f;
    public float jumpMuliplyer = 1f;

    float accelerationTimeAirborne = .2f;
    float accelerationTimeGrounded = .1f;
    float moveSpeed = 6f;
    float speedMultiplier = 1f;

    public WallJump wallJumpClimb;
    public WallJump wallJumpOff;
    public WallJump wallLeap;
    //public Vector2 wallJumpClimb;
    //public Vector2 wallJumpOff;
    //public Vector2 wallLeap;

    public float wallSlideSpeedMax = 3;
    public float wallStickTime = .25f;
    float timeToWallUnstick;

    float gravity;
    float maxJumpVelocity;
    float minJumpVelocity;
    Vector2 velocity;
    float velocityXSmoothing;

    private float DownPressPrevTime = -1000f;
    private float DownPressFTP = 0.5f;

    private Vector2 additionalVelocity;
    public float decelerationTimeGrounded_AV;
    public float decelerationTimeAirborne_AV;

    public bool CanDash =  true;
    public float DashSpeed = 12;
    private float DashPressPrevTime = -1000f;
    private float DashPress = 0.5f;
    public float DashDuration = 0.25f;
    public float DashCoolDown = 0.5f;
    private float DashEndTime = -1000f;
    public bool IsDashing = false;

    public int AirBorneJumpCnt = 0;
    public int AirBorneMaxJumpCnt = 0;

    Controller2D controller;

    [System.Serializable]
    public struct ButtonNames
    {
        public string Horizontal;
        public string Vertical;
        public KeyCode Right;
        public KeyCode Left;
        public KeyCode Down;
        public KeyCode Up;
        public string Dash;
        public string Jump;
    }

    [System.Serializable]
    public struct WallJump
    {
        public float Angle;
        public float Force;
        public Vector2 XY;
    }

    void Start()
    {

//        buttonNames.Horizontal = "Horizontal";
//        buttonNames.Vertical = "Vertical";
//        buttonNames.Right = KeyCode.D;
//        buttonNames.Left = KeyCode.A;
//        buttonNames.Down = KeyCode.S;
//        buttonNames.Up = KeyCode.W;
//        buttonNames.Dash = "Fire1";
//        buttonNames.Jump = "Jump";

        wallJumpClimb.XY = GetXYFromAngle(wallJumpClimb.Angle, wallJumpClimb.Force);
        wallJumpOff.XY = GetXYFromAngle(wallJumpOff.Angle, wallJumpOff.Force);
        wallLeap.XY = GetXYFromAngle(wallLeap.Angle, wallLeap.Force);

        controller = GetComponent<Controller2D> ();

        gravity = -(2 * maxJumpHeight) / Mathf.Pow (timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt (2 * Mathf.Abs (gravity) * minJumpHeight);
        print ("Gravity: " + gravity + "  Jump Velocity: " + maxJumpVelocity);
    }

    void Update()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw(buttonNames.Horizontal), Input.GetAxisRaw(buttonNames.Vertical));
        int wallDirX = (controller.collisions.left) ? -1 : 1;

        float Speed = (IsDashing) ? (DashSpeed * speedMultiplier) : (moveSpeed * speedMultiplier);
        float targetVelocityX = input.x * Speed;

        if (IsDashing && Mathf.Abs(input.x) != 1)
        {
            targetVelocityX = (DashSpeed * speedMultiplier) * controller.collisions.faceDir;
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
                Time.time - DashEndTime >= DashCoolDown
                && IsDashing == false
            )
            {

                if (
                    (Input.GetKeyDown(buttonNames.Right) && controller.collisions.faceDir == 1)
                    || (Input.GetButtonDown(buttonNames.Dash))
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
                    (Input.GetKeyDown(buttonNames.Left) && controller.collisions.faceDir == -1)
                    || (Input.GetButtonDown(buttonNames.Dash))
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
        }

        bool fallingThroughPlatform = false;

        if (controller.collisions.below && controller.collisions.fallingThroughPlatformBelow)
        {
            if (Input.GetKey(buttonNames.Down) && Input.GetButtonDown(buttonNames.Jump))
            {
                controller.fallThroughPlatform();
                fallingThroughPlatform = true;
            }

            if (Input.GetKeyDown(buttonNames.Down))
            {
                if (Time.time - DownPressPrevTime <= DownPressFTP)
                {
                    controller.fallThroughPlatform();
                }

                DownPressPrevTime = Time.time;

                fallingThroughPlatform = true;
            }
        }


        if (Input.GetButtonDown(buttonNames.Jump))
        {
            if (wallSliding)
            {
                if (wallDirX == input.x)
                {
                    velocity.x = -wallDirX * (wallJumpClimb.XY.x * jumpMuliplyer);
                    velocity.y = (wallJumpClimb.XY.y * jumpMuliplyer);
                }
                else if (input.x == 0)
                {
                    velocity.x = -wallDirX * (wallJumpClimb.XY.x * jumpMuliplyer);
                    velocity.y = (wallJumpClimb.XY.y  * jumpMuliplyer);
                }
                else
                {
                    velocity.x = -wallDirX * (wallJumpClimb.XY.x * jumpMuliplyer);
                    velocity.y = (wallJumpClimb.XY.y * jumpMuliplyer);
                }
            }

            if (controller.collisions.below && !fallingThroughPlatform )
            {
                velocity.y = (maxJumpVelocity * jumpMuliplyer);
            }

            if (!controller.collisions.below && !wallSliding)
            {
                if (AirBorneJumpCnt < AirBorneMaxJumpCnt)
                {
                    velocity.y = (maxJumpVelocity * jumpMuliplyer);
                    AirBorneJumpCnt++;
                }
            }
        }

        if (Input.GetButtonUp(buttonNames.Jump))
        {
            if (velocity.y > (minJumpVelocity * jumpMuliplyer))
            {
                velocity.y = (minJumpVelocity * jumpMuliplyer);
            }
        }

        velocity.y += gravity * Time.deltaTime;

        velocity += additionalVelocity;

        controller.Move (velocity * Time.deltaTime, input);

        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }

        if (controller.collisions.below)
        {
            AirBorneJumpCnt = 0;
        }

        additionalVelocity.x = Mathf.Lerp(additionalVelocity.x, 0f, (controller.collisions.below) ? decelerationTimeGrounded_AV : decelerationTimeAirborne_AV);
        additionalVelocity.y = Mathf.Lerp(additionalVelocity.y, 0f, (controller.collisions.below) ? decelerationTimeGrounded_AV : decelerationTimeAirborne_AV);

    }

    void AddForce(Vector3 Force)
    {
        additionalVelocity += new Vector2(Force.x,Force.y);
    }

    void AddForce(Vector2 Force)
    {
        additionalVelocity += Force;
    }

    void AddForce(float Angle, float Force)
    {
        Vector2 F = GetXYFromAngle(Angle,Force);
        additionalVelocity += F;
    }

    private Vector2 GetXYFromAngle(float Angle,float Radius)
    {
        float X = Radius * Mathf.Cos(Angle * 0.017453f);
        float Y = Radius * Mathf.Sin(Angle * 0.017453f);

        return new Vector2(X, Y);
    }

    void ResetIsDashing()
    {
        IsDashing = false;
        DashEndTime = Time.time;
    }
}