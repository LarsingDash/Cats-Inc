using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cats_Inc.Scripts.World
{
	public class Mover : MonoBehaviour
	{
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

		public void Init(Func<int> pickup,
			Func<int, List<Vector2>> pickupRoute,
			Func<int, int> collect,
			Func<int, List<Vector2>> deliveryRoute,
			Func<int, int> delivery
		)
		{
			pickupCall = pickup;
			pickupRouteCall = pickupRoute;
			deliveryRouteCall = deliveryRoute;
			collectCall = collect;
			deliveryCall = delivery;
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
				//Wait until pickup is available, if so: reserve amount and gain the route
				yield return new WaitUntil(AttemptPickupRequest);

				//Walk the route
				yield return WalkRoute();

				//Obtain the amount
				holdingAmount = collectCall(pickupTarget);

				//Keep delivering to destinations until the holdingAmount is empty
				while (holdingAmount > 0)
				{
					//Request route to available destination
					yield return new WaitUntil(AttemptDeliveryRequest);

					//Walk the route
					yield return WalkRoute();

					holdingAmount = deliveryCall(holdingAmount);
				}
			}
		}

		private IEnumerator WalkRoute()
		{
			// print("Walking...");
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