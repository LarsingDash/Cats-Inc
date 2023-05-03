using System;
using System.Collections;
using UnityEngine;

namespace Cats_Inc.Scripts.World
{
	public class ImportDock : MonoBehaviour
	{
		//Components
		private SpriteRenderer spriteRenderer;

		//Truck
		private bool shouldImport;
		private int amountRemaining { get; set; }
		private int amountReserved;

		public void Init(Sprite square)
		{
			spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
			spriteRenderer.sprite = square;
		}

		public void Launch()
		{
			shouldImport = true;
			amountRemaining = 0;
			amountReserved = 0;

			spriteRenderer.color = Color.blue;

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

			print("Docked");

			var val = amountRemaining / (float)WorldManager.stats.importTruckMax;
			spriteRenderer.color = new Color(val, val, val);
		}

		private void ReleaseTruck()
		{
			print("Released");
			spriteRenderer.color = Color.red;
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
			print("Truck: " + amountRemaining);

			var val = amountRemaining / (float)WorldManager.stats.importTruckMax;
			spriteRenderer.color = new Color(val, val, val);

			return pickup;
		}
	}
}