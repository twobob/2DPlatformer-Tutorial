using UnityEngine;
using System.Collections;
   
[RequireComponent(typeof(Player))]
public class PlayerInput : MonoBehaviour
{

	Player player;


	private float downPressPrevTime = -1000f;
    private float downPress = 0.5f;

	private float rightPressPrevTime = -1000f;
    private float rightPress = 0.5f;

	private float leftPressPrevTime = -1000f;
    private float leftPress = 0.5f;

	void Start()
	{
		player = GetComponent<Player>();
	}

	void Update()
	{
		Vector2 directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		player.SetDirectionalInput(directionalInput);


		if (Input.GetKeyDown(KeyCode.E))
        {
			player.OnDash();
        }

		if (
			Input.GetAxisRaw("Vertical") < 0f
            && Input.GetKeyDown(KeyCode.Space)
            )
        {
			player.fallThroughPlatform(true);
        }      
		else if (
			Input.GetAxisRaw("Vertical") >= 0f
            && Input.GetKeyDown(KeyCode.Space)
		    )
        {
            player.OnJumpInputDown();
        }

		if (Input.GetKeyUp(KeyCode.Space))
		{
			player.OnJumpInputUp();
		}

  
		if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
		{
			if (Time.time - downPressPrevTime <= downPress)
           {
				player.fallThroughPlatform(false);
           }
            
           downPressPrevTime = Time.time;   
		}

		if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
			if (player.leftDash)
            {
                player.OffDash();
            }

            if (Time.time - rightPressPrevTime <= rightPress)
            {
                //player.fallThroughPlatform(false);
				player.OnDash(true);
            }

            rightPressPrevTime = Time.time;

			leftPressPrevTime = -1000f;
        }

		if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
			if (player.rightDash)
			{
				player.OffDash();
			}

            if (Time.time - leftPressPrevTime <= leftPress)
            {
				//player.fallThroughPlatform(false);
				player.OnDash(false);
            }

			leftPressPrevTime = Time.time;

			rightPressPrevTime = -1000f;
        }



	}
}
