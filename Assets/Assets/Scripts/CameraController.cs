using UnityEngine;

namespace Assets.Scripts
{
	public class CameraController : MonoBehaviour
	{
		//Components
		public Transform test;
		private Camera gameCamera;

		//Moving
		private bool isMouseDown;
		private Vector3 dragOrigin;
		private Vector3 cameraTarget;

		private void Start()
		{
			gameCamera = GetComponent<Camera>();
			cameraTarget = gameCamera.transform.position;
		}

		private void FixedUpdate()
		{
			//todo if isPaused
			camMoving();


			//todo
			//optimize this with lastVerExtend and so on

			// var pos = transform.position;
			// const int mapSize = 500;
			//
			// var verExtend = gameCamera.orthographicSize;
			// var horExtend = verExtend * Screen.width / Screen.height;
			//
			// pos.x = Mathf.Clamp(pos.x, 0 + horExtend, mapSize - horExtend);
			// pos.y = Mathf.Clamp(pos.y, 0 + verExtend, mapSize - verExtend);
			// transform.position = pos;
		}

		private void camMoving()
		{
			var cameraPosition = gameCamera.transform.position;

			if (Input.GetMouseButton(0))
			{
				if (!isMouseDown)
				{
					isMouseDown = true;
					dragOrigin = gameCamera.ScreenToWorldPoint(Input.mousePosition);
					test.position = new Vector3(dragOrigin.x, dragOrigin.y, 0);
				}

				cameraTarget = dragOrigin
				               - gameCamera.ScreenToWorldPoint(Input.mousePosition)
				               + cameraPosition;
			}
			else isMouseDown = false;

			var difference = cameraTarget - cameraPosition;
			if (difference.magnitude > 0.05)
			{
				gameCamera.transform.Translate(difference / 10);
			}
			
			Debug.DrawLine(cameraPosition, cameraTarget, Color.blue);
			Debug.DrawLine(cameraPosition, cameraPosition + difference, Color.green);
		}
	}
}