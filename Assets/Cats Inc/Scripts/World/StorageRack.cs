using UnityEngine;

namespace Cats_Inc.Scripts.World
{
	public class StorageRack : MonoBehaviour
	{
		//Components
		private SpriteRenderer spriteRenderer;

		//Values
		private int storedAmount;

		public void Init(Sprite square)
		{
			spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
			spriteRenderer.sprite = square;
			TempColor();

			transform.Translate(5, 0, 0);
		}

		public bool IsFull()
		{
			return storedAmount >= WorldManager.stats.importRackMax;
		}

		public int Deliver(int amount)
		{
			print($"Delivery: {amount}");
			var max = WorldManager.stats.importRackMax;
			var difference = storedAmount + amount - max;

			if (difference > 0)
			{
				storedAmount = max;
				TempColor();
				return difference;
			}

			storedAmount += amount;
			TempColor();
			return 0;
		}

		private void TempColor()
		{
			print("Rack: " + storedAmount);
			var val = storedAmount / (float)WorldManager.stats.importRackMax;
			spriteRenderer.color = new Color(val, val, val);
		}

		private bool first;
		public void FixedUpdate()
		{
			if (Input.GetButton("Submit"))
			{
				if (first)
				{
					first = false;
					storedAmount = 0;
					TempColor();
				}
			}
			else
			{
				first = true;
			}
		}
	}
}