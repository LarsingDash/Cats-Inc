using TMPro;
using UnityEngine;

namespace Cats_Inc.Scripts.Other
{
	public class CustomText
	{
		private readonly TextMeshPro textMesh;
		public CustomText(Transform parentTrans)
		{
			var textObject = new GameObject("Text") { transform =
				{
					parent = parentTrans,
					position = parentTrans.position + new Vector3(0, -0.75f, 0)
				}
			};

			textMesh = textObject.AddComponent<TextMeshPro>();
			textMesh.alignment = TextAlignmentOptions.Center;
			textMesh.fontSize = 5;
		}

		public void ChangeText(string text)
		{
			textMesh.text = text;
		}
	}
}