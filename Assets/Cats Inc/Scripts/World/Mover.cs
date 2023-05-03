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
		private List<Vector2> activeRoute;
		private int pickupTarget;
		private int destinationTarget;
		private int holdingAmount;

		//Callbacks
		private Func<int> pickupCall;
		private Func<int, List<Vector2>> pickupRouteCall;
		private Func<int, int> collectCall;
		private Func<int, List<Vector2>> deliveryRouteCall;
		private Func<int, int> deliveryCall;

		public void Init(Sprite square,
			Func<int> pickup,
			Func<int, List<Vector2>> pickupRoute,
			Func<int, int> collect,
			Func<int, List<Vector2>> deliveryRoute,
			Func<int, int> delivery
		)
		{
			transform.position = new Vector3(5, 3, transform.position.z);

			spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
			spriteRenderer.sprite = square;
			
			pickupCall = pickup;
			pickupRouteCall = pickupRoute;
			deliveryRouteCall = deliveryRoute;
			collectCall = collect;
			deliveryCall = delivery;

			customText = new CustomText(transform);
			customText.ChangeText("Idle");
		}

		public void Launch()
		{
			holdingAmount = 0;
			shouldPickup = true;

			StartCoroutine(MoverBehaviour());
		}

		private IEnumerator MoverBehaviour()
		{
			while (shouldPickup)
			{
				customText.ChangeText("Idle");

				//Wait until pickup is available, if so: reserve amount and gain the route
				yield return new WaitUntil(AttemptPickupRequest);

				//Walk the route
				yield return WalkRoute();

				//Obtain the amount
				holdingAmount = collectCall(pickupTarget);
				customText.ChangeText(holdingAmount.ToString());

				//Keep delivering to destinations until the holdingAmount is empty
				while (holdingAmount > 0)
				{
					//Request route to available destination
					yield return new WaitUntil(AttemptDeliveryRequest);

					//Walk the route
					yield return WalkRoute();

					holdingAmount = deliveryCall(holdingAmount);
					customText.ChangeText(holdingAmount.ToString());
				}
			}
		}

		private IEnumerator WalkRoute()
		{
			yield return new WaitForSeconds(0.5f); //todo actually walk the route......
		}

		private bool AttemptPickupRequest()
		{
			pickupTarget = pickupCall();
			if (pickupTarget == -1) return false;

			activeRoute = pickupRouteCall(pickupTarget);
			return true;
		}

		private bool AttemptDeliveryRequest()
		{
			activeRoute = deliveryRouteCall(holdingAmount);
			return activeRoute != null;
		}
	}
}