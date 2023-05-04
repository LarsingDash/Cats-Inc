using System.Collections.Generic;
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
			dock.transform.Translate(-3,0,0);

			//Storage
			var storageObject = new GameObject("Storage Rack") { transform = { parent = transform } };
			storageObject.AddComponent<StorageRack>();
			var rack = storageObject.GetComponent<StorageRack>();
			racks.Add(rack);
			
			rack.Init(square);

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
		private static List<Vector2> GenerateRoute(Vector2 start, Vector2 end)
		{
			//Create route and add start
			var route = new List<Vector2> {start};
			
			//Make sure the start != end
			if (start == end)
			{
				route.Add(end);
				return route;
			}

			//Calculate the halfway height of the 2 horizontal points
			var halfwayHeight = (int) (start.y + end.y) / 2;
			var halfwayStart = new Vector2(start.x, halfwayHeight);
			var halfwayEnd = new Vector2(end.x, halfwayHeight);

			var verticalStep = end.y - start.y > 0 ? Vector2.up : Vector2.down;
			var firstVerPoint = new Vector2(start.x, start.y);
			for (var i = 0; i < 10; i++)
			{
				firstVerPoint += verticalStep;
				if (firstVerPoint == halfwayStart) break;
				
				route.Add(new Vector2(firstVerPoint.x, firstVerPoint.y));
			}
			
			//Add starting horizontal
			route.Add(halfwayStart);

			//Fill horizontal section
			var horizontalStep = end.x - start.x > 0 ? Vector2.right : Vector2.left;
			var horPoint = new Vector2(halfwayStart.x, halfwayStart.y);
			for (var i = 0; i < 10; i++)
			{
				horPoint += horizontalStep;
				if (horPoint == halfwayEnd) break;
				
				route.Add(new Vector2(horPoint.x, horPoint.y));
			}
			
			//Add ending horizontal
			route.Add(halfwayEnd);
			
			var secondVerPoint = new Vector2(halfwayEnd.x, halfwayEnd.y);
			for (var i = 0; i < 10; i++)
			{
				secondVerPoint += verticalStep;
				if (secondVerPoint == end) break;
				
				route.Add(new Vector2(secondVerPoint.x, secondVerPoint.y));
			}

			//Add end
			route.Add(end);

			// print("--------------" + start + "---" + end);
			// foreach (var point in route)
			// {
			// 	print(point);
			// }

			return route;
		}
	}
}