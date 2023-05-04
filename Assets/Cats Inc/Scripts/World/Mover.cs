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

		private List<Vector2> activeRoute;
		private Vector2 currentPosition;
		private Vector2 currentTarget;
		private Vector2 nextTarget;
		private const int movementStep = 5; //0 - 100. 100 % movementStep HAS TO BE 0
		private int currentStep;
		private bool hasReachedDestination = true;

		private int pickupTarget;
		private int destinationTarget;

		//Callbacks
		private Func<int> pickupCall;
		private Func<Vector2, int, List<Vector2>> pickupRouteCall;
		private Func<int, int> collectCall;
		private Func<int> deliveryRequestCall;
		private Func<Vector2, int, List<Vector2>> deliveryRouteCall;
		private Func<int, int, int> deliveryCall;

		public void Init(Sprite square,
			Func<int> pickup,
			Func<Vector2, int, List<Vector2>> pickupRoute,
			Func<int, int> collect,
			Func<int> deliveryRequest,
			Func<Vector2, int, List<Vector2>> deliveryRoute,
			Func<int, int, int> delivery
		)
		{
			currentPosition = new Vector3(5, 6, 0);
			transform.position = currentPosition;

			spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
			spriteRenderer.sprite = square;
			spriteRenderer.color = Color.blue;
			
			pickupCall = pickup;
			pickupRouteCall = pickupRoute;
			collectCall = collect;
			deliveryRequestCall = deliveryRequest;
			deliveryRouteCall = deliveryRoute;
			deliveryCall = delivery;

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

		public void FixedUpdate()
		{
			if (hasReachedDestination) return;
			currentStep += movementStep;

			if (currentStep > 100)
			{
				if (activeRoute.IndexOf(nextTarget) + 1 >= activeRoute.Count)
				{
					hasReachedDestination = true; 
					return;
				}
				
				currentStep = 0;
				currentTarget = nextTarget;
				nextTarget = activeRoute[activeRoute.IndexOf(nextTarget) + 1];
			}
			
			transform.position = Vector2.Lerp(currentTarget, nextTarget, currentStep / 100f);
			
			//Temp
			var col = CalculateColor();
			spriteRenderer.color = new Color(0, col, 1 - col);
		}

		private float CalculateColor()
		{
			return 0.5f * (float) Math.Sin(2 * Math.PI * (currentStep / 100f) - 0.5 * Math.PI) + 0.5f;
		}

		private IEnumerator MoverBehaviour()
		{
			while (shouldPickup)
			{
				customText.ChangeText("Idle");

				//Wait until pickup is available, if so: reserve amount and gain the route
				currentPosition = transform.position;
				yield return new WaitUntil(AttemptPickupRequest);

				//Walk the route
				yield return WalkRoute();

				//Obtain the amount
				holdingAmount = collectCall(pickupTarget);
				customText.ChangeText(holdingAmount.ToString());

				//Keep delivering to destinations until the holdingAmount is empty
				while (holdingAmount > 0)
				{
					currentPosition = transform.position;
					//Request route to available destination
					yield return new WaitUntil(AttemptDeliveryRequest);

					//Walk the route
					yield return WalkRoute();

					holdingAmount = deliveryCall(destinationTarget, holdingAmount);
					customText.ChangeText(holdingAmount.ToString());
				}
			}
		}

		private IEnumerator WalkRoute()
		{
			currentStep = 0;
			currentTarget = activeRoute[0];
			nextTarget = activeRoute[1];
			
			hasReachedDestination = currentTarget == nextTarget;
			yield return new WaitUntil(() => hasReachedDestination);
		}

		private bool AttemptPickupRequest()
		{
			pickupTarget = pickupCall();
			if (pickupTarget == -1) return false;

			activeRoute = pickupRouteCall(currentPosition, pickupTarget);
			return true;
		}

		private bool AttemptDeliveryRequest()
		{
			destinationTarget = deliveryRequestCall();
			if (destinationTarget == -1) return false;
			
			activeRoute = deliveryRouteCall(currentPosition, destinationTarget);
			return true;
		}
	}
}