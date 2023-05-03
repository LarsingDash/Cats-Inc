using System.Collections.Generic;
using Assets.Scripts.Other;
using UnityEngine;

namespace Assets.Scripts.World
{
	public class WorldManager : MonoBehaviour
	{
		//Publicly accessible data
		public Rect worldBounds { get; private set; }

		//Components
		public Sprite square;
		private GameObject background;

		//Import
		private readonly List<ImportDock> docks = new();
		private readonly List<Mover> importMovers = new();
		private Storage storage;

		private void Start()
		{
			worldBounds = new Rect(0, 0, 10, 50);

			GameController.finishStart(GameController.StartupOption.WorldBackground, SetBackground);
			GameController.finishStart(GameController.StartupOption.WorldImport, CreateImport);
			GameController.finishStart(GameController.StartupOption.WorldLaunch, Launch);
		}

		private void Launch()
		{
			foreach (var dock in docks)
				dock.Launch();
		}

		private void SetBackground()
		{
			//Create background object
			background = new GameObject("Background")
			{
				transform =
				{
					parent = transform,
					position = new Vector3(worldBounds.x + worldBounds.width / 2, worldBounds.y + worldBounds.height / 2, 0),
					localScale = new Vector3(worldBounds.width, worldBounds.height, 0)
				}
			};

			//Add and set the sprite
			var sprite = background.AddComponent<SpriteRenderer>();
			sprite.sprite = square;
			sprite.color = new Color(0.2f, 0.2f, 0.2f);
		}

		private void CreateImport()
		{
			//Dock
			var dockObject = new GameObject("Import Dock") { transform = { parent = transform } };
			dockObject.AddComponent<ImportDock>();
			var dock = dockObject.GetComponent<ImportDock>();
			docks.Add(dock);

			dock.Init(this, DockTruck, square);

			//Storage
			var storageObject = new GameObject("Storage") { transform = { parent = transform } };
			storageObject.AddComponent<Storage>();
			storage = storageObject.GetComponent<Storage>();
			storage.Init(100);
			
			//Mover
			var moverObject = new GameObject("Import Mover") { transform = { parent = transform } };
			moverObject.AddComponent<Mover>();
			var mover = moverObject.GetComponent<Mover>();
			importMovers.Add(mover);
			
			mover.Init(0, ImportCollect, storage.Deliver);
		}

		//todo make coroutine that waits till truck is empty: waitWhile(previousAmount != dock.getAmountRemaining())
		private void DockTruck()
		{
			//todo check for available movers and how many are needed to empty the truck
			var availableMover = importMovers[0];
			availableMover.Notify(new List<Vector2>());
		}

		private void ImportCollect(int index)
		{
			print($"Collected by {index}");
			
			//todo check how much has been collected
			importMovers[index].Collect(0);
			
			//Todo see todo DockTruck()
			docks[0].ReleaseTruck();
			
			importMovers[index].SendToDeliver(new List<Vector2>());
		}
	}
}