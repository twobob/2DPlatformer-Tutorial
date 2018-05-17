using UnityEngine;
using System.Collections;

//[ExecuteInEditMode]
[RequireComponent (typeof (Controller2D))]
public class Player : MonoBehaviour {

	public float maxJumpHeight = 4;
	public float minJumpHeight = 1;
	public float timeToJumpApex = .4f;
	public float jumpMuliplyer = 1f;

	public int maxAirJumpCnt = 1;
	public int airJumpCnt = 0;

	float accelerationTimeAirborne = 0.2f;
	float accelerationTimeGrounded = 0.1f;
	float moveSpeed = 6;
	public float speedMultiplier = 1;

	//public Vector2 wallJumpClimb;
	//public Vector2 wallJumpOff;
	//public Vector2 wallLeap;

	public WallJump wallJumpClimb;
    public WallJump wallJumpOff;
    public WallJump wallLeap;

	[System.Serializable]
    public struct WallJump
    {
        public float Angle;
        public float Force;

		[HideInInspector]
        public Vector2 XY;
      
        public void setXY()
		{
			XY.x = Force * Mathf.Cos(Angle * 0.017453f);
			XY.y = Force * Mathf.Sin(Angle * 0.017453f);
		}
    }


	public float wallSlideSpeedMax = 3;
	public float wallStickTime = .25f;
	public float wallStickSpeedMax = 0.5f;
	float timeToWallUnstick;

	float gravity;
	float maxJumpVelocity;
	float minJumpVelocity;
	Vector3 velocity;
	float velocityXSmoothing;
   
	Controller2D controller;   
	Vector2 directionalInput;
	public int faceDir = 1;

	bool wallSliding;
	int wallDirX;


	//public bool CanDash = true;
    //public float DashSpeed = 12;
    //public float DashDuration = 0.25f;
    //public float DashCoolDown = 0.5f;
    //private float DashEndTime = -1000f;
    //public bool IsDashing = false;

	void Start() 
	{
		wallJumpClimb.setXY();
		wallJumpOff.setXY();
		wallLeap.setXY();

		controller = GetComponent<Controller2D> ();

		gravity = -(2 * maxJumpHeight) / Mathf.Pow (timeToJumpApex, 2);
		maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
		minJumpVelocity = Mathf.Sqrt (2 * Mathf.Abs (gravity) * minJumpHeight);
	}

	void Update() 
	{
		CalculateVelocity ();
		HandleWallSliding ();

		controller.Move (velocity * Time.deltaTime, directionalInput);

		if (controller.collisions.above || controller.collisions.below) 
		{
			if (controller.collisions.slidingDownMaxSlope) 
			{
				velocity.y += controller.collisions.slopeNormal.y * -gravity * Time.deltaTime;
			} else 
			{
				velocity.y = 0;
			}
		}

        //reset air jump count
		if (controller.collisions.below)
		{
			airJumpCnt = 0;
		}

		if (wallSliding)
		{
			airJumpCnt = 0;
		}

		//print(controller.collisions.fallingThroughPlatformBelow);
	}




	public void SetDirectionalInput (Vector2 input) 
	{
		directionalInput = input;
		print(directionalInput.ToString());

		if (directionalInput.x > 0)
		{
			faceDir = 1;
		}
		else if (directionalInput.x < 0)
		{
			faceDir = -1;
		}

	}

	bool fallingThroughPlatform = false;
    

	public void fallThroughPlatform(bool doJump = false)
	{      
		if (controller.collisions.below && controller.collisions.fallingThroughPlatformBelow)
		{
			controller.allowFallThroughPlatform();
            fallingThroughPlatform = true;
		}
		else
		{
			if (doJump)
			{
				OnJumpInputDown();
			}
		}

	}

	public bool CanDash = true;
    public float DashSpeed = 12;
    public float DashDuration = 0.25f;
    public float DashCoolDown = 0.5f;
    private float DashEndTime = -1000f;
    public bool IsDashing = false;

	public bool rightDash = false;
	public bool leftDash = false;

    public void OnDash(bool R)
	{
		if (CanDash && !IsDashing && Time.time - DashEndTime >= DashCoolDown)
		{
			Invoke("ResetIsDashing", DashDuration);
			IsDashing = true;

			rightDash = R;
			leftDash = !R;
		}
	}

    public void OffDash()
	{
		if (CanDash && IsDashing)
        {         
			IsDashing = false;
			DashEndTime = Time.time;
        }
	}

	void ResetIsDashing()
    {
        IsDashing = false;
        DashEndTime = Time.time;
    }
    

	public void OnJumpInputDown()
	{

		if (wallSliding)
		{
			//wallJumpClimb
			//if (FloatsEqual(wallDirX,directionalInput.x))
			if (Mathf.Approximately(wallDirX,directionalInput.x))	
			{
				velocity.x = -wallDirX * (wallJumpClimb.XY.x * jumpMuliplyer);
				velocity.y = (wallJumpClimb.XY.y * jumpMuliplyer);

				print("wallJumpClimb: ");
				print("     Angle = " + wallJumpClimb.Angle + " ");
				print("     Force = " + wallJumpClimb.Force + " ");
				print("     XY = " + wallJumpClimb.XY.ToString() + " ");

			}
			//wallJumpOff
			//else if (FloatsEqual(directionalInput.x,0f))
			else if (Mathf.Approximately(directionalInput.x,0))   
			{
				velocity.x = -wallDirX * (wallJumpOff.XY.x * jumpMuliplyer) ;
				velocity.y = (wallJumpOff.XY.y * jumpMuliplyer);

				print("wallJumpOff: " );
				print("     Angle = " + wallJumpOff.Angle + " ");
				print("     Force = " + wallJumpOff.Force + " ");
				print("     XY = " + wallJumpOff.XY.ToString() + " ");
			}
			//wallLeap
			else
			{
				velocity.x = -wallDirX * (wallLeap.XY.x * jumpMuliplyer);
				velocity.y = (wallLeap.XY.y * jumpMuliplyer);

				print("wallLeap: ");
				print("     Angle = " + wallLeap.Angle + " ");
				print("     Force = " + wallLeap.Force + " ");
				print("     XY = " + wallLeap.XY.ToString() + " ");
			}
		}



		//fallingThroughPlatform = false;
			


		if (controller.collisions.below )//&& controller.collisions.fallingThroughPlatformBelow)
		{
			print(directionalInput.ToString());
           

			if (controller.collisions.slidingDownMaxSlope)
			{
				if (!directionalInput.x.Equals(-Mathf.Sign(controller.collisions.slopeNormal.x)))
				//if (directionalInput.x != -Mathf.Sign(controller.collisions.slopeNormal.x))
				{
					// not jumping against max slope
					velocity.y = maxJumpVelocity * controller.collisions.slopeNormal.y;
					velocity.x = maxJumpVelocity * controller.collisions.slopeNormal.x;
				}
			}

			velocity.y = maxJumpVelocity * jumpMuliplyer;

		}
		else
		{
			if (airJumpCnt < maxAirJumpCnt)
			{
				velocity.y = maxJumpVelocity * jumpMuliplyer;
				airJumpCnt++;
			}
		}
       
	}

		//if (wallSliding) {
		//	if (wallDirX == directionalInput.x) {
		//		velocity.x = -wallDirX * wallJumpClimb.x;
		//		velocity.y = wallJumpClimb.y;
		//	}
		//	else if (directionalInput.x == 0) {
		//		velocity.x = -wallDirX * wallJumpOff.x;
		//		velocity.y = wallJumpOff.y;
		//	}
		//	else {
		//		velocity.x = -wallDirX * wallLeap.x;
		//		velocity.y = wallLeap.y;
		//	}
		//}
		//if (controller.collisions.below) {
		//	if (controller.collisions.slidingDownMaxSlope) {
		//		if (directionalInput.x != -Mathf.Sign (controller.collisions.slopeNormal.x)) { // not jumping against max slope
		//			velocity.y = maxJumpVelocity * controller.collisions.slopeNormal.y;
		//			velocity.x = maxJumpVelocity * controller.collisions.slopeNormal.x;
		//		}
		//	} else {
		//		velocity.y = maxJumpVelocity;
		//	}
		//}


	public void OnJumpInputUp() 
	{
		if (velocity.y > minJumpVelocity) 
		{
			velocity.y = minJumpVelocity;
		}
	}
		

	void HandleWallSliding() 
	{
		wallDirX = (controller.collisions.left) ? -1 : 1;
		wallSliding = false;
		if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0) 
		{
			wallSliding = true;

			if (velocity.y < -wallSlideSpeedMax) 
			{
				velocity.y = -wallSlideSpeedMax;
			}

            //allow stick to wall
			//if (FloatsEqual(wallDirX,directionalInput.x))
			if (Mathf.Approximately(wallDirX,directionalInput.x))
			{
				if (velocity.y < -wallStickSpeedMax)
                {
					velocity.y = -wallStickSpeedMax;
                }
			}
            
			if (timeToWallUnstick > 0) 
			{
				velocityXSmoothing = 0;
				velocity.x = 0;
                
				//if ( !FloatsEqual(directionalInput.x,wallDirX) && !FloatsEqual(directionalInput.x, 0f))
				//if (!FloatsEqual(directionalInput.x, wallDirX))
				if (Mathf.Approximately(directionalInput.x,wallDirX))
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

	}

	void CalculateVelocity() 
	{
        
		float Speed = (IsDashing) ? (DashSpeed * speedMultiplier * faceDir) : (moveSpeed * speedMultiplier * directionalInput.x);
		float targetVelocityX =  Speed;
        
		velocity.x = Mathf.SmoothDamp (velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below)?accelerationTimeGrounded:accelerationTimeAirborne);
		velocity.y += gravity * Time.deltaTime;

		//if (IsDashing)
		//{
		//	velocity.y = 0;;
		//}
  
	}

 

	private Vector2 GetXYFromAngle(float Angle, float Radius)
    {
        float X = Radius * Mathf.Cos(Angle * 0.017453f);
        float Y = Radius * Mathf.Sin(Angle * 0.017453f);

        return new Vector2(X, Y);
    }

	//private bool FloatsEqual(float F1, float F2)
	//{
	//	float diff = Mathf.Abs(F1 - F2);

	//	return (diff <= 0.01) ? true : false;
	//}



    //drawing to debug
	void OnDrawGizmosSelected()
    {
		Gizmos.color = Color.white;
        
		if (wallSliding)
		{
			Gizmos.DrawLine(transform.position, transform.position + new Vector3(wallJumpClimb.XY.x * wallDirX * -1f, wallJumpClimb.XY.y, 0f));
			Gizmos.DrawLine(transform.position, transform.position + new Vector3(wallJumpOff.XY.x * wallDirX * -1f, wallJumpOff.XY.y, 0f));
			Gizmos.DrawLine(transform.position, transform.position + new Vector3(wallLeap.XY.x * wallDirX * -1f, wallLeap.XY.y, 0f));
		}




    }
}
