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
		private bool isTruckDocked = false;

		public void Init(WorldManager parent, Sprite square)
		{
			worldManager = parent;
			
			spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
			spriteRenderer.sprite = square;
			spriteRenderer.color = Color.blue;
		}

		private void FixedUpdate()
		{
			if (Input.GetButton("Submit"))
			{
				isTruckDocked = false;
				spriteRenderer.color = Color.red;
			}
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
			}
		}

		private void DockTruck()
		{
			isTruckDocked = true;
			print("Docked");
			spriteRenderer.color = Color.blue;
		}
	}
}