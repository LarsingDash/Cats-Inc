using System;
using Cats_Inc.Scripts.World;
using UnityEngine;

namespace Cats_Inc.Scripts.Other
{
	public class CameraController : MonoBehaviour
	{
		//Components
		public WorldManager worldManager;
		private Camera gameCamera;

		//Properties
		private float cameraWidth;
		private float cameraHeight;

		//Moving
		private const int CAMERA_SPEED = 10;
		private bool isMouseDown;
		private Vector3 dragOrigin;
		private Vector3 cameraTarget;

		private void Start()
		{
			gameCamera = GetComponent<Camera>();
			GameController.finishStart(GameController.StartupOption.AlignCamera, AlignCamera);
		}

		private void AlignCamera()
		{
			//Setting vars
			var worldBounds = worldManager.worldBounds;
			var trans = transform;
			var currentPos = trans.position;

			//Moving camera to the horizontal middle of the screen
			var newPosition = new Vector3(worldBounds.x + worldBounds.width / 2, worldBounds.y + worldBounds.height / 2, currentPos.z);
			trans.position = newPosition;
			cameraTarget = newPosition;
		}

		private void FixedUpdate()
		{
			var cameraTransform = transform;
			var cameraPosition = cameraTransform.position;

			//When LMB is pressed:
			//	Only first frame: origin of the drag = current mouse position
			//  Always: update camera target (origin of drag - current position in drag + current camera position)
			if (Input.GetMouseButton(0))
			{
				//Only first frame
				if (!isMouseDown)
				{
					isMouseDown = true;
					dragOrigin = gameCamera.ScreenToWorldPoint(Input.mousePosition);
				}

				//Always
				cameraTarget = dragOrigin
				               - gameCamera.ScreenToWorldPoint(Input.mousePosition)
				               + cameraPosition;
			}
			else isMouseDown = false;

			//Calculate difference between the target and the current position
			var difference = cameraTarget - cameraPosition;
			if (difference.magnitude > 0.05)
			{
				//Move camera towards target, but restrict horizontal movement
				difference.x = 0;
				cameraTransform.Translate(difference / CAMERA_SPEED);

				//Clamp camera within the bounds given by the WorldManager
				var bounds = worldManager.worldBounds;
				cameraHeight = gameCamera.orthographicSize;
				cameraWidth = cameraHeight * Screen.width / Screen.height;

				var currentPos = cameraTransform.position;
				var posClone = currentPos;
				posClone.y = Math.Clamp(currentPos.y, bounds.y + cameraHeight, bounds.height - cameraHeight);
				cameraTransform.position = posClone;
			}
		}
	}
}