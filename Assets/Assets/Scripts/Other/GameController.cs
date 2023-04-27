using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Other
{
	public static class GameController
	{
		//List of startup actions so that they can be invoked in a controlled sequence
		private static readonly Dictionary<string, Action> startup = new()
		{
			{ "WorldManager", null },
			{ "CameraController", null },
			{ "PlayerManager", null },
			{ "StartupBegin", null },
			{ "StartupEnd", null },
		};

		//Notify the list that the caller's Start() is finished, start controlled startup sequence once all are finished
		public static void finishStart(string name, Action init)
		{
			//Typo check
			if (startup.ContainsKey(name))
			{
				//Assigning the Action value = notified that the Start() is finished
				startup[name] = init;

				//Check if the caller was the last to finish
				if (startup.Values.Any(component => component == null))
					return;

				//Display startup screen
				startup["StartupBegin"]();
				
				//Startup Sequence
				startup["WorldManager"]();
				startup["CameraController"]();
				startup["PlayerManager"]();
				
				//Hide startup screen
				startup["StartupEnd"]();
			}
			else
			{
				throw new Exception("startup sequence name not found");
			}
		}
	}
}