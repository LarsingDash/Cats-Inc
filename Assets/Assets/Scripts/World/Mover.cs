using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.World
{
	public class Mover : MonoBehaviour
	{
		//Vars
		private int index;
		
		//Callbacks
		private Action<int> collectFunc;
		private Func<int, int> deliverFunc;
		
		//Collection
		private List<Vector2> route;
		private int holdingAmount;

		public void Init(int i, Action<int> collect, Func<int, int> deliver)
		{
			index = i;
			collectFunc = collect;
			deliverFunc = deliver;
		}

		public void Notify(List<Vector2> newRoute)
		{
			route = newRoute;

			StartCoroutine(WalkRoute(true));
		}

		public void SendToDeliver(List<Vector2> newRoute)
		{
			route = newRoute;

			StartCoroutine(WalkRoute(false));
		}

		private IEnumerator WalkRoute(bool isPickup)
		{
			foreach (var point in route)
			{
				yield return new WaitForSeconds(1);
				print($"Walked {point}");
			}
			
			//Collecting / Delivering
			yield return new WaitForSeconds(1);

			if (isPickup) collectFunc(index);
			else Deliver();
		}

		public void Collect(int collected)
		{
			holdingAmount += collected;
		}

		private void Deliver()
		{
			print("Delivering");
			//todo do mover stuff like availability

			//todo check how much didn't fit in the storage (return value)
			var leftover = deliverFunc(holdingAmount);
		}
	}
}