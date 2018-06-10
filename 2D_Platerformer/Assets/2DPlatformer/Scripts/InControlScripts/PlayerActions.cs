//http://www.gallantgames.com/pages/incontrol-getting-started
/*
control.IsPressed;   // bool, is currently pressed
control.WasPressed;  // bool, pressed since previous tick
control.WasReleased; // bool, released since previous tick
control.HasChanged;  // bool, has changed since previous tick
control.State;       // bool, is currently pressed (same as IsPressed)
control.Value;       // float, in range -1..1 for axes, 0..1 for buttons / triggers
control.LastState;   // bool, previous tick state
control.LastValue;   // float, previous tick value
*/

namespace MyInControlScript
{
	using InControl;
	using UnityEngine;


	public class PlayerActions : PlayerActionSet
	{
		public PlayerAction Melee;
		public PlayerAction Jump;
		public PlayerAction Dash;
		public PlayerAction Left;
		public PlayerAction Right;
		public PlayerAction Up;
		public PlayerAction Down;
		public PlayerTwoAxisAction Move;
  
		public PlayerAction altLeft;
		public PlayerAction altRight;
		public PlayerAction altUp;
		public PlayerAction altDown;
		public PlayerTwoAxisAction altMove;

		public PlayerActions()
		{
			Melee = CreatePlayerAction( "Melee" );
			Jump = CreatePlayerAction( "Jump" );
			Left = CreatePlayerAction( "Move Left" );
			Right = CreatePlayerAction( "Move Right" );
			Up = CreatePlayerAction( "Move Up" );
			Down = CreatePlayerAction( "Move Down" );
			Move = CreateTwoAxisPlayerAction( Left, Right, Down, Up );
			Dash = CreatePlayerAction("Dash");

			altLeft = CreatePlayerAction("Alt Move Left");
            altRight = CreatePlayerAction("Alt Move Right");
            altUp = CreatePlayerAction("Alt Move Up");
            altDown = CreatePlayerAction("Alt Move Down");
			altMove = CreateTwoAxisPlayerAction(altLeft, altRight, altDown, altUp);
		}


		public static PlayerActions CreateWithDefaultBindings()
		{
			var playerActions = new PlayerActions();

			// How to set up mutually exclusive keyboard bindings with a modifier key.
			// playerActions.Back.AddDefaultBinding( Key.Shift, Key.Tab );
			// playerActions.Next.AddDefaultBinding( KeyCombo.With( Key.Tab ).AndNot( Key.Shift ) );

			playerActions.Dash.AddDefaultBinding(Key.E);
			playerActions.Dash.AddDefaultBinding(InputControlType.LeftBumper);

			playerActions.Jump.AddDefaultBinding( Key.Space );
			playerActions.Jump.AddDefaultBinding( InputControlType.Action1 );
			//playerActions.Fire.AddDefaultBinding( Mouse.LeftButton );

			playerActions.Melee.AddDefaultBinding( Key.A );
			playerActions.Melee.AddDefaultBinding( InputControlType.Action3 );
			playerActions.Melee.AddDefaultBinding( InputControlType.Back );

			playerActions.Up.AddDefaultBinding( Key.UpArrow );
			playerActions.Down.AddDefaultBinding( Key.DownArrow );
			playerActions.Left.AddDefaultBinding( Key.LeftArrow );
			playerActions.Right.AddDefaultBinding( Key.RightArrow );

			playerActions.Left.AddDefaultBinding( InputControlType.LeftStickLeft );
			playerActions.Right.AddDefaultBinding( InputControlType.LeftStickRight );
			playerActions.Up.AddDefaultBinding( InputControlType.LeftStickUp );
			playerActions.Down.AddDefaultBinding( InputControlType.LeftStickDown );

			playerActions.Left.AddDefaultBinding( InputControlType.DPadLeft );
			playerActions.Right.AddDefaultBinding( InputControlType.DPadRight );
			playerActions.Up.AddDefaultBinding( InputControlType.DPadUp );
			playerActions.Down.AddDefaultBinding( InputControlType.DPadDown );
            

			playerActions.altUp.AddDefaultBinding(Key.I);
            playerActions.altDown.AddDefaultBinding(Key.K);
            playerActions.altLeft.AddDefaultBinding(Key.J);
            playerActions.altRight.AddDefaultBinding(Key.L);

			playerActions.altLeft.AddDefaultBinding(InputControlType.RightStickLeft);
			playerActions.altRight.AddDefaultBinding(InputControlType.RightStickRight);
			playerActions.altUp.AddDefaultBinding(InputControlType.RightStickUp);
			playerActions.altDown.AddDefaultBinding(InputControlType.RightStickDown);
            

			//playerActions.Up.AddDefaultBinding( Mouse.PositiveY );
			//playerActions.Down.AddDefaultBinding( Mouse.NegativeY );
			//playerActions.Left.AddDefaultBinding( Mouse.NegativeX );
			//playerActions.Right.AddDefaultBinding( Mouse.PositiveX );

			playerActions.ListenOptions.IncludeUnknownControllers = true;
			playerActions.ListenOptions.MaxAllowedBindings = 4;
			//playerActions.ListenOptions.MaxAllowedBindingsPerType = 1;
			//playerActions.ListenOptions.AllowDuplicateBindingsPerSet = true;
			playerActions.ListenOptions.UnsetDuplicateBindingsOnSet = true;
			//playerActions.ListenOptions.IncludeMouseButtons = true;
			//playerActions.ListenOptions.IncludeModifiersAsFirstClassKeys = true;
			//playerActions.ListenOptions.IncludeMouseButtons = true;
			//playerActions.ListenOptions.IncludeMouseScrollWheel = true;

			playerActions.ListenOptions.OnBindingFound = ( action, binding ) => 
			{
				if (binding == new KeyBindingSource( Key.Escape ))
				{
					action.StopListeningForBinding();
					return false;
				}
				return true;
			};

			playerActions.ListenOptions.OnBindingAdded += ( action, binding ) => {
				Debug.Log( "Binding added... " + binding.DeviceName + ": " + binding.Name );
			};

			playerActions.ListenOptions.OnBindingRejected += ( action, binding, reason ) => {
				Debug.Log( "Binding rejected... " + reason );
			};

			return playerActions;
		}
	}
}


//control.IsPressed;   // bool, is currently pressed
//control.WasPressed;  // bool, pressed since previous tick
//control.WasReleased; // bool, released since previous tick
//control.HasChanged;  // bool, has changed since previous tick
//control.State;       // bool, is currently pressed (same as IsPressed)
//control.Value;       // float, in range -1..1 for axes, 0..1 for buttons / triggers
//control.LastState;   // bool, previous tick state
//control.LastValue;   // float, previous tick value