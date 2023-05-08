using System;
using System.Collections;
using System.Collections.Generic;
using Cats_Inc.Scripts.Other;
using UnityEngine;

namespace Cats_Inc.Scripts.World
{
	public class Mover : MonoBehaviour
	{
		//Components
		private SpriteRenderer spriteRenderer;
		private CustomText customText;

		//Vars
		private bool shouldPickup;
		private int holdingAmount;
		
		//Targets
		private int pickupTarget;
		private int destinationTarget;
		
		//Route
		private List<Vector2> activeRoute;
		private bool hasReachedDestination = true;
		private Vector2 currentPosition;

		private Vector2 currentRouteTarget;
		private Vector2 nextRouteTarget;
		
		private const int movementStep = 5; //0 - 100. 100 % movementStep HAS TO BE 0
		private int currentStep;

		//Callbacks
		private Func<int> pickupCall;
		private Func<Vector2, int, List<Vector2>> pickupRouteCall;
		private Func<int, int> collectCall;
		private Func<int> deliveryRequestCall;
		private Func<Vector2, int, List<Vector2>> deliveryRouteCall;
		private Func<int, int, int> deliveryCall;

		/** Init **/
		public void Init(Sprite square,
			Func<int> pickup,
			Func<Vector2, int, List<Vector2>> pickupRoute,
			Func<int, int> collect,
			Func<int> deliveryRequest,
			Func<Vector2, int, List<Vector2>> deliveryRoute,
			Func<int, int, int> delivery
		)
		{
			//Assigning callbacks
			pickupCall = pickup;
			pickupRouteCall = pickupRoute;
			collectCall = collect;
			deliveryRequestCall = deliveryRequest;
			deliveryRouteCall = deliveryRoute;
			deliveryCall = delivery;
			
			//Init
			transform.position = currentPosition;

			//Adding components
			spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
			spriteRenderer.sprite = square;
			spriteRenderer.color = Color.blue;
			
			customText = new CustomText(transform);
			customText.ChangeText("Idle");
			customText.ChangeColor(Color.blue);
		}

		public void Launch()
		{
			holdingAmount = 0;
			shouldPickup = true;

			StartCoroutine(MoverBehaviour());
		}

		/** Main **/
		//todo improve IDLE animation timing
		private IEnumerator MoverBehaviour()
		{
			//Stops when factory is unloaded //todo
			while (shouldPickup)
			{
				customText.ChangeText("Idle");

				//Wait until pickup is available, if so: reserve amount and obtain the route
				currentPosition = transform.position;
				yield return new WaitUntil(AttemptPickupRequest);

				//Walk the route
				yield return WalkRoute();

				//Collect the amount
				holdingAmount = collectCall(pickupTarget);
				customText.ChangeText(holdingAmount.ToString());

				//Keep delivering to available destinations until the holdingAmount is empty
				while (holdingAmount > 0)
				{
					//Save current position for route generation
					currentPosition = transform.position;
					
					//Request route to available destination
					yield return new WaitUntil(AttemptDeliveryRequest);

					//Walk the route
					yield return WalkRoute();

					//Deliver available amount, save leftover amount for next round in the loop
					holdingAmount = deliveryCall(destinationTarget, holdingAmount);
					customText.ChangeText(holdingAmount.ToString());
				}
			}
		}

		/** Movement **/
		//Setup variables for FixedUpdate to execute, wait till completion
		private IEnumerator WalkRoute()
		{
			//Vars setup
			currentStep = 0;
			currentRouteTarget = activeRoute[0];
			nextRouteTarget = activeRoute[1];

			//Instantly complete movement if start == end, else set variable that starts FixedUpdate procedure
			//(for when the mover was idle on the interaction spot of the location that became available)
			hasReachedDestination = currentRouteTarget == nextRouteTarget;
			
			//Wait for FixedUpdate procedure to finish
			yield return new WaitUntil(() => hasReachedDestination);
		}

		//Walk the route if the Coroutine WalkRoute has set hasReachedDestination to false
		public void FixedUpdate()
		{
			//Check if end has been reached, else increment step towards 100
			if (hasReachedDestination) return;
			currentStep += movementStep;

			//CurrentStep over 100 means the route point has been reached
			if (currentStep > 100)
			{
				//Check if the reached point was the last, if so set finish bool to true and stop
				if (activeRoute.IndexOf(nextRouteTarget) + 1 >= activeRoute.Count)
				{
					hasReachedDestination = true;
					return;
				}

				//If end hasn't yet been reached: reset step counter and shift targets to the next
				currentStep = 0;
				currentRouteTarget = nextRouteTarget;
				nextRouteTarget = activeRoute[activeRoute.IndexOf(nextRouteTarget) + 1];
			}

			//Increment position towards next route point
			transform.position = Vector2.Lerp(currentRouteTarget, nextRouteTarget, currentStep / 100f);

			//Temp //todo cat stretch / walking animation
			var col = CalculateColor();
			spriteRenderer.color = new Color(0, col, 1 - col);
		}

		/** Requests **/
		//Requests an available pickup target, returns false if none were available, otherwise returns true and sets active route
		private bool AttemptPickupRequest()
		{
			pickupTarget = pickupCall();
			if (pickupTarget == -1) return false;

			activeRoute = pickupRouteCall(currentPosition, pickupTarget);
			return true;
		}
		
		//Requests an available delivery target, returns false if none were available, otherwise returns true and sets active route
		private bool AttemptDeliveryRequest()
		{
			destinationTarget = deliveryRequestCall();
			if (destinationTarget == -1) return false;

			activeRoute = deliveryRouteCall(currentPosition, destinationTarget);
			return true;
		}

		/** Other **/
		private const int baseSize = 2;
		private const int additionSize = 2;
		
		public static int CalculateMoverSize(int level)
		{
			return baseSize + additionSize * (level - 1);
		}
		
		private float CalculateColor()
		{
			return 0.5f * (float)Math.Sin(2 * Math.PI * (currentStep / 100f) - 0.5 * Math.PI) + 0.5f;
		}
	}
}