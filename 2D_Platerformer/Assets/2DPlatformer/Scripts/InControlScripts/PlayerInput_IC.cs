namespace MyInControlScript
{
	using InControl;
	using UnityEngine;
	using System.Collections;

	[RequireComponent(typeof(Player))]
	public class PlayerInput_IC : MonoBehaviour
	{
		Renderer cachedRenderer;
		PlayerActions playerActions;
		string saveData;


		void OnEnable()
		{
			// See PlayerActions.cs for this setup.
			playerActions = PlayerActions.CreateWithDefaultBindings();
			//playerActions.Move.OnLastInputTypeChanged += ( lastInputType ) => Debug.Log( lastInputType );

			LoadBindings();
		}


		void OnDisable()
		{
			// This properly disposes of the action set and unsubscribes it from
			// update events so that it doesn't do additional processing unnecessarily.
			playerActions.Destroy();
		}



		Player player;

		private float downPressPrevTime = -1000f;
        private float downPress = 0.5f;

        private float rightPressPrevTime = -1000f;
        private float rightPress = 0.5f;

        private float leftPressPrevTime = -1000f;
        private float leftPress = 0.5f;


		void Start()
		{
			cachedRenderer = GetComponent<Renderer>();

			player = GetComponent<Player>();

			playerActions.Down.RepeatDelay = 0.5f;
		}


		void Update()
		{
			//transform.Rotate( Vector3.down, 500.0f * Time.deltaTime * playerActions.Move.X, Space.World );
			//transform.Rotate( Vector3.right, 500.0f * Time.deltaTime * playerActions.Move.Y, Space.World );

			//var fireColor = playerActions.Fire.IsPressed ? Color.red : Color.white;
			//var jumpColor = playerActions.Jump.IsPressed ? Color.green : Color.white;

			//cachedRenderer.material.color = Color.Lerp( fireColor, jumpColor, 0.5f );

			Vector2 directionalInput = new Vector2(playerActions.Move.X, playerActions.Move.Y);
            player.SetDirectionalInput(directionalInput);
            

            if (
				playerActions.Down.IsPressed
				&& playerActions.Jump.WasPressed
                )
            {
                player.fallThroughPlatform(true);
            }
            else if (
				playerActions.Jump.WasPressed
                )
            {
                player.OnJumpInputDown();
            }

			if (playerActions.Jump.WasReleased)
            {
                player.OnJumpInputUp();
            }


			//print("playerActions.Down.RepeatDelay: " + playerActions.Down.RepeatDelay);
			//print("playerActions.Down.FirstRepeatDelay: " + playerActions.Down.FirstRepeatDelay);
			//print("playerActions.Down.WasRepeated: " + playerActions.Down.WasRepeated);
			//print("playerActions.Down.State: " + playerActions.Down.State);
			//print("playerActions.Down.LastState: " + playerActions.Down.LastState);

            //on hold down
			if (playerActions.Down.WasRepeated)
			{
				player.fallThroughPlatform(false);
			}


			if (playerActions.Down.WasPressed)
            {
                if (Time.time - downPressPrevTime <= downPress)
                {
                    player.fallThroughPlatform(false);
                }

                downPressPrevTime = Time.time;
            }


		}


		void SaveBindings()
		{
			saveData = playerActions.Save();
			PlayerPrefs.SetString( "Bindings", saveData );
		}


		void LoadBindings()
		{
			if (PlayerPrefs.HasKey( "Bindings" ))
			{
				saveData = PlayerPrefs.GetString( "Bindings" );
				playerActions.Load( saveData );
			}
		}


		void OnApplicationQuit()
		{
			PlayerPrefs.Save();
		}

        /*
		void OnGUI()
		{
			const float h = 22.0f;
			var y = 10.0f;

			GUI.Label( new Rect( 10, y, 300, y + h ), "Last Input Type: " + playerActions.LastInputType );
			y += h;

			//GUI.Label( new Rect( 10, y, 300, y + h ), "Active Device: " + playerActions.ActiveDevice.Name );
			//y += h;

			GUI.Label( new Rect( 10, y, 300, y + h ), "Last Device Class: " + playerActions.LastDeviceClass );
			y += h;

			GUI.Label( new Rect( 10, y, 300, y + h ), "Last Device Style: " + playerActions.LastDeviceStyle );
			y += h;

			var actionCount = playerActions.Actions.Count;
			for (var i = 0; i < actionCount; i++)
			{
				var action = playerActions.Actions[i];

				var name = action.Name;
				if (action.IsListeningForBinding)
				{
					name += " (Listening)";
				}
				name += " = " + action.Value;
				//name += " via " + action.ActiveDevice.Name;
				//name += ", class: " + action.LastDeviceClass;
				//name += ", style: " + action.LastDeviceStyle;
				GUI.Label( new Rect( 10, y, 500, y + h ), name );
				y += h;

				var bindingCount = action.Bindings.Count;
				for (var j = 0; j < bindingCount; j++)
				{
					var binding = action.Bindings[j];

					GUI.Label( new Rect( 75, y, 300, y + h ), binding.DeviceName + ": " + binding.Name );

					if (GUI.Button( new Rect( 20, y + 3.0f, 20, h - 5.0f ), "-" ))
					{
						action.RemoveBinding( binding );
					}

					if (GUI.Button( new Rect( 45, y + 3.0f, 20, h - 5.0f ), "+" ))
					{
						action.ListenForBindingReplacing( binding );
					}

					y += h;
				}

				if (GUI.Button( new Rect( 20, y + 3.0f, 20, h - 5.0f ), "+" ))
				{
					action.ListenForBinding();
				}

				if (GUI.Button( new Rect( 50, y + 3.0f, 50, h - 5.0f ), "Reset" ))
				{
					action.ResetBindings();
				}

				y += 25.0f;
			}

			if (GUI.Button( new Rect( 20, y + 3.0f, 50, h ), "Load" ))
			{
				LoadBindings();
			}

			if (GUI.Button( new Rect( 80, y + 3.0f, 50, h ), "Save" ))
			{
				SaveBindings();
			}

			if (GUI.Button( new Rect( 140, y + 3.0f, 50, h ), "Reset" ))
			{
				playerActions.Reset();
			}
		}
		*/
	    
	}
}

