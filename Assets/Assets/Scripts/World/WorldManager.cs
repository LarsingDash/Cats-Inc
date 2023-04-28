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

		private readonly List<ImportDock> docks = new();

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

			dock.Init(this, square);
		}
	}
}