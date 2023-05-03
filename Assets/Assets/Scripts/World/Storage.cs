using UnityEngine;

namespace Assets.Scripts.World
{
	public class Storage : MonoBehaviour
	{
		private int storedQuantity;
		private int maxQuantity;

		public void Init(int max)
		{
			maxQuantity = max;
		}

		public int Deliver(int amount)
		{
			print($"Delivered: {amount}");
			var difference = storedQuantity + amount - maxQuantity;

			return difference < 0 ? difference : 0;
		}
	}
}