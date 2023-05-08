using System;
using System.Collections;
using Cats_Inc.Scripts.Other;
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
		
		private int amountRemaining;
		private int amountReserved;

		/** Init **/
		public void Init(Sprite square)
		{
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

		/** Main **/
		//Loop: Dock new truck - Wait for truck to be emptied - Release truck - Wait for new truck
		private IEnumerator DockBehaviour()
		{
			//Stops when factory is unloaded //todo
			while (shouldImport)
			{
				DockTruck();

				yield return new WaitWhile(() => amountRemaining > 0);

				ReleaseTruck();
				
				yield return new WaitForSeconds(2.5f); //todo stat
			}
		}

		/** Dock / Release **/
		private const int baseSize = 10;
		private const int additionSize = 2;

		private void DockTruck()
		{
			//Refill truck
			amountRemaining = baseSize + additionSize * (DataManager.GetLevel(ImportVars.DockSize) - 1);
			
			customText.ChangeText(amountRemaining.ToString());
		}

		private void ReleaseTruck()
		{
			customText.ChangeText("Waiting");
		}

		/** Mover interaction **/
		//Returns / Checks if there is stock available to pickup (keeping reservations in mind)
		public bool AttemptToClaim()
		{
			if (amountReserved >= amountRemaining) return false;
			amountReserved += Mover.CalculateMoverSize(DataManager.GetLevel(ImportVars.MoverSize));

			return true;
		}

		//Lowers amountRemaining and amountReserved - Returns amount of stock actually picked up (in case max mover amount > amountRemaining)
		public int Pickup()
		{
			var moverMax = Mover.CalculateMoverSize(DataManager.GetLevel(ImportVars.MoverSize));
			var pickup = Math.Min(amountRemaining, moverMax);

			amountRemaining = Math.Max(0, amountRemaining - pickup);
			amountReserved = Math.Max(0, amountReserved - moverMax); //todo can maybe softlock when changing moverMax mid-pickup?

			customText.ChangeText(amountRemaining.ToString());

			return pickup;
		}
	}
}