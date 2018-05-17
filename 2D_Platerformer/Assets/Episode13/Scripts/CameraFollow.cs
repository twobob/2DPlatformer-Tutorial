﻿using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

	public Controller2D target;
	public float verticalOffset;
	public float lookAheadDstX;
	public float lookSmoothTimeX;
	public float verticalSmoothTime;
	public Vector2 focusAreaSize;

	FocusArea focusArea;

	float currentLookAheadX;
	float targetLookAheadX;
	float lookAheadDirX;
	float smoothLookVelocityX;
	float smoothVelocityY;

	bool lookAheadStopped;

	//public bool allowInput;
	//public Vector3 maxLook;
	//public float lookSpeed;
	//Vector3 look;

	void Start() 
	{
		focusArea = new FocusArea (target.collider.bounds, focusAreaSize);
	}

	void LateUpdate() 
	{
		focusArea.Update (target.collider.bounds);

		Vector2 focusPosition = focusArea.centre + Vector2.up * verticalOffset;

		//if (focusArea.velocity.x != 0) 
		//if (!FloatsEqual(focusArea.velocity.x,0))
		if (!Mathf.Approximately(focusArea.velocity.x,0))
		{
			lookAheadDirX = Mathf.Sign (focusArea.velocity.x);
			//if (Mathf.Sign(target.playerInput.x) == Mathf.Sign(focusArea.velocity.x) && target.playerInput.x != 0) 
			//if (FloatsEqual(Mathf.Sign(target.playerInput.x),Mathf.Sign(focusArea.velocity.x)) && !FloatsEqual(target.playerInput.x,0f))
			if (Mathf.Approximately(Mathf.Sign(target.playerInput.x), Mathf.Sign(focusArea.velocity.x)) && !Mathf.Approximately(target.playerInput.x, 0f))
			{
				lookAheadStopped = false;
				targetLookAheadX = lookAheadDirX * lookAheadDstX;
			}
			else 
			{
				if (!lookAheadStopped) 
				{
					lookAheadStopped = true;
					targetLookAheadX = currentLookAheadX + (lookAheadDirX * lookAheadDstX - currentLookAheadX)/4f;
				}
			}
		}


		currentLookAheadX = Mathf.SmoothDamp (currentLookAheadX, targetLookAheadX, ref smoothLookVelocityX, lookSmoothTimeX);
        
		focusPosition.y = Mathf.SmoothDamp (transform.position.y, focusPosition.y, ref smoothVelocityY, verticalSmoothTime);
		focusPosition += Vector2.right * currentLookAheadX;

		////look = new Vector2(Input.GetAxisRaw("altHorizontal") * maxLook.x, Input.GetAxisRaw("altVertical") * maxLook.y) ;
		//look = Vector3.MoveTowards(look,
		//                           new Vector2(Input.GetAxisRaw("altHorizontal") * maxLook.x, Input.GetAxisRaw("altVertical") * maxLook.y)
		//                        , lookSpeed);

		//print(Input.GetAxisRaw("altHorizontal").ToString());

		transform.position = (Vector3)focusPosition + Vector3.forward * -10;
	}

	void OnDrawGizmos() 
	{
		Gizmos.color = new Color (1, 0, 0, .5f);
		Gizmos.DrawCube (focusArea.centre, focusAreaSize);
	}

	struct FocusArea 
	{
		public Vector2 centre;
		public Vector2 velocity;
		float left,right;
		float top,bottom;


		public FocusArea(Bounds targetBounds, Vector2 size) {
			left = targetBounds.center.x - size.x/2;
			right = targetBounds.center.x + size.x/2;
			bottom = targetBounds.min.y;
			top = targetBounds.min.y + size.y;

			velocity = Vector2.zero;
			centre = new Vector2((left+right)/2,(top +bottom)/2);
		}

		public void Update(Bounds targetBounds) {
			float shiftX = 0;
			if (targetBounds.min.x < left) {
				shiftX = targetBounds.min.x - left;
			} else if (targetBounds.max.x > right) {
				shiftX = targetBounds.max.x - right;
			}
			left += shiftX;
			right += shiftX;

			float shiftY = 0;
			if (targetBounds.min.y < bottom) {
				shiftY = targetBounds.min.y - bottom;
			} else if (targetBounds.max.y > top) {
				shiftY = targetBounds.max.y - top;
			}
			top += shiftY;
			bottom += shiftY;
			centre = new Vector2((left+right)/2,(top +bottom)/2);
			velocity = new Vector2 (shiftX, shiftY);
		}
	}

	//private bool FloatsEqual(float F1, float F2)
    //{
    //    float diff = Mathf.Abs(F1 - F2);

    //    return (diff <= 0.01) ? true : false;
    //}

}