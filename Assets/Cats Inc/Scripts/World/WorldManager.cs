using System.Collections.Generic;
using System.Linq;
using Cats_Inc.Scripts.Other;
using UnityEngine;

namespace Cats_Inc.Scripts.World
{
	public class WorldManager : MonoBehaviour
	{
		//Publicly accessible data
		public Rect worldBounds { get; private set; }
		public static Stats stats { get; private set; }

		//Components
		public Sprite square;
		private GameObject background;

		//Import
		private readonly List<ImportDock> importDocks = new();
		private readonly List<Mover> importMovers = new();
		private readonly List<StorageRack> racks = new();

		/** Init **/
		private void Start()
		{
			worldBounds = new Rect(0, 0, 10, 50);

			stats = new Stats(10, 2, 7);

			GameController.finishStart(GameController.StartupOption.WorldBackground, SetBackground);
			GameController.finishStart(GameController.StartupOption.WorldImport, CreateImport);
			GameController.finishStart(GameController.StartupOption.WorldLaunch, Launch);
		}

		private void SetBackground()
		{
			//Create background object
			background = new GameObject("Background")
			{
				transform =
				{
					parent = transform,
					position = new Vector3(worldBounds.x + worldBounds.width / 2, worldBounds.y + worldBounds.height / 2, 10),
					localScale = new Vector3(worldBounds.width, worldBounds.height, 0)
				}
			};

			//Add and set the sprite
			var sprite = background.AddComponent<SpriteRenderer>();
			sprite.sprite = square;
			sprite.color = new Color(0.2f, 0.2f, 0.2f);
		}

		/** Creation **/
		private void CreateImport()
		{
			//Dock
			var dockObject = new GameObject("Import Dock") { transform = { parent = transform } };
			dockObject.AddComponent<ImportDock>();
			var dock = dockObject.GetComponent<ImportDock>();
			importDocks.Add(dock);

			dock.Init(square);

			//Storage
			var storageObject = new GameObject("Storage Rack") { transform = { parent = transform } };
			storageObject.AddComponent<StorageRack>();
			var rack = storageObject.GetComponent<StorageRack>();
			racks.Add(rack);
			
			rack.Init(square);
			
			var otherStorage = new GameObject("Second storage Rack") { transform = { parent = transform } };
			otherStorage.AddComponent<StorageRack>();
			var otherRack = otherStorage.GetComponent<StorageRack>();
			racks.Add(otherRack);
			
			otherRack.Init(square);
			otherRack.transform.Translate(2,0,0);

			//Mover
			var moverObject = new GameObject("Import Mover") { transform = { parent = transform } };
			moverObject.AddComponent<Mover>();
			var mover = moverObject.GetComponent<Mover>();
			importMovers.Add(mover);

			mover.Init(square, ImportRequestPickup, ImportRouteForPickup, ImportCollect, ImportRequestDelivery, ImportRouteForDelivery, ImportDeliver);
		}

		private void Launch()
		{
			foreach (var dock in importDocks)
				dock.Launch();

			foreach (var mover in importMovers)
				mover.Launch();
		}

		/** Import **/
		private int ImportRequestPickup()
		{
			for (var i = 0; i < importDocks.Count; i++)
			{
				if (importDocks[i].AttemptToClaim())
					return i;
			}

			return -1;
		}
		
		private List<Vector2> ImportRouteForPickup(Vector2 start, int dock)
		{
			//todo change to custom interaction point
			return GenerateRoute(start, importDocks[dock].transform.position);
		}

		private int ImportRequestDelivery()
		{
			var rackIndex = -1;

			for (var i = 0; i < racks.Count; i++)
			{
				if (!racks[i].IsFull())
				{
					rackIndex = i;
					break;
				}
			}

			return rackIndex;
		}
		
		private List<Vector2> ImportRouteForDelivery(Vector2 start, int targetRack)
		{
			//todo change to custom interaction point
			return GenerateRoute(start, racks[targetRack].transform.position);
		}

		private int ImportCollect(int dock)
		{
			return importDocks[dock].Pickup();
		}

		private int ImportDeliver(int target, int amount)
		{
			return racks[target].Deliver(amount);
		}

		/** Other **/
		private List<Vector2> GenerateRoute(Vector2 start, Vector2 end)
		{
			return new List<Vector2> {start, end};
		}
	}
}