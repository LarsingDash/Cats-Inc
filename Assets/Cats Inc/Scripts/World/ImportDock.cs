using System;
using System.Collections;
using Cats_Inc.Scripts.Other;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Cats_Inc.Scripts.World
{
	public class ImportDock : MonoBehaviour
	{
		//Components
		private SpriteRenderer spriteRenderer;
		private CustomText customText;

		//Truck
		private bool shouldImport;
		private int amountRemaining { get; set; }
		private int amountReserved;

		public void Init(Sprite square)
		{
			transform.position = new Vector3(5, 1, transform.position.z);
			
			spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
			spriteRenderer.sprite = square;

			customText = new CustomText(transform);
			customText.ChangeText("Waiting");
		}

		public void Launch()
		{
			shouldImport = true;
			amountRemaining = 0;
			amountReserved = 0;
			
			StartCoroutine(DockBehaviour());
		}

		private IEnumerator DockBehaviour()
		{
			while (shouldImport)
			{
				//Truck Delay (can stack another truck in a queue while one is still at the dock
				yield return new WaitForSeconds(2.5f);

				//Dock new truck
				DockTruck();

				//Wait till the truck is empty
				yield return new WaitWhile(() => amountRemaining > 0);

				ReleaseTruck();
			}
		}

		private void DockTruck()
		{
			//Refill truck
			amountRemaining = WorldManager.stats.importTruckMax;
			
			customText.ChangeText(amountRemaining.ToString());
		}

		private void ReleaseTruck()
		{
			customText.ChangeText("Waiting");
		}

		public bool AttemptToClaim()
		{
			if (amountReserved >= amountRemaining) return false;
			amountReserved += WorldManager.stats.importMoverMax;

			return true;
		}

		public int Pickup()
		{
			var moverMax = WorldManager.stats.importMoverMax;
			var pickup = Math.Min(amountRemaining, moverMax);

			amountRemaining = Math.Max(0, amountRemaining - pickup);
			amountReserved = Math.Max(0, amountReserved - moverMax); //todo can maybe softlock when changing moverMax mid-pickup?

			customText.ChangeText(amountRemaining.ToString());

			return pickup;
		}
	}
}