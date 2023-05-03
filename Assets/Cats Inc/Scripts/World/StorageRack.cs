using Cats_Inc.Scripts.Other;
using UnityEngine;

namespace Cats_Inc.Scripts.World
{
	public class StorageRack : MonoBehaviour
	{
		//Components
		private SpriteRenderer spriteRenderer;
		private CustomText customText;

		//Values
		private int storedAmount;

		public void Init(Sprite square)
		{
			transform.position = new Vector3(5, 10, transform.position.z);

			spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
			spriteRenderer.sprite = square;

			customText = new CustomText(transform);
			customText.ChangeText("Empty");
		}

		public bool IsFull()
		{
			return storedAmount >= WorldManager.stats.importRackMax;
		}

		public int Deliver(int amount)
		{
			var max = WorldManager.stats.importRackMax;
			var difference = storedAmount + amount - max;

			if (difference > 0)
			{
				storedAmount = max;
				UpdateText(max);
				return difference;
			}

			storedAmount += amount;
			UpdateText(max);
			return 0;
		}

		private void UpdateText(int max)
		{
			customText.ChangeText(storedAmount == max ? "Full" : storedAmount.ToString());
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
					customText.ChangeText("Empty");
				}
			}
			else
			{
				first = true;
			}
		}
	}
}