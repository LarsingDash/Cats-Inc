using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.World
{
	public class ImportDock : MonoBehaviour
	{
		//Components
		private WorldManager worldManager;
		private SpriteRenderer spriteRenderer;

		//Truck
		private bool shouldImport;
		private bool isTruckDocked;
		private Action dockAction;

		public void Init(WorldManager parent, Action action, Sprite square)
		{
			worldManager = parent;
			dockAction = action;

			spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
			spriteRenderer.sprite = square;
		}

		public void Launch()
		{
			shouldImport = true;
			StartCoroutine(ImportTrucks());
		}

		private IEnumerator ImportTrucks()
		{
			while (shouldImport)
			{
				//Truck Delay (can stack another truck in a queue while one is still at the dock
				yield return new WaitForSeconds(2.5f);

				//Wait till the dock is empty
				yield return new WaitWhile(() => isTruckDocked);

				//Dock new truck
				DockTruck();
				
				//todo REMOVE
				shouldImport = false;
			}
		}

		private void DockTruck()
		{
			isTruckDocked = true;
			print("Docked");
			spriteRenderer.color = Color.blue;

			dockAction();
		}

		public void ReleaseTruck()
		{
			isTruckDocked = false;
			spriteRenderer.color = Color.red;
		}
	}
}