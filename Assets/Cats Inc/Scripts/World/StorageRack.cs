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

		/** Init **/
		public void Init(Sprite square)
		{
			transform.position = new Vector3(5, 9, 5);

			spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
			spriteRenderer.sprite = square;

			customText = new CustomText(transform);
			customText.ChangeText("Empty");
		}

		/** Mover interaction **/
		public bool IsFull()
		{
			return storedAmount >= WorldManager.stats.importRackMax;
		}

		//Increases StoredAmount and returns any leftover amount that didn't fit within the max capacity
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

		/** Other **/
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