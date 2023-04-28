using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Other
{
	public static class GameController
	{
		public enum StartupOption
		{
			StartupBegin,
			StartupEnd,
			WorldBackground,
			WorldImport,
			WorldLaunch,
			AlignCamera,
			PlayerManager,
		}
		
		//List of startup actions so that they can be invoked in a controlled sequence
		private static readonly Dictionary<StartupOption, Action> startup = new()
		{
			{ StartupOption.StartupBegin, null },
			{ StartupOption.StartupEnd, null },
			{ StartupOption.WorldBackground, null },
			{ StartupOption.WorldImport, null },
			{ StartupOption.WorldLaunch, null },
			{ StartupOption.AlignCamera, null },
			{ StartupOption.PlayerManager, null },
		};

		//Notify the list that the caller's Start() is finished, start controlled startup sequence once all are finished
		public static void finishStart(StartupOption name, Action init)
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
				startup[StartupOption.StartupBegin]();
				
				//Startup Sequence
				startup[StartupOption.WorldBackground]();
				startup[StartupOption.WorldImport]();
				
				startup[StartupOption.AlignCamera]();
				
				startup[StartupOption.PlayerManager]();

				//Launch Main Components
				startup[StartupOption.WorldLaunch]();
				
				//Hide startup screen
				startup[StartupOption.StartupEnd]();
			}
			else
			{
				throw new Exception("startup sequence name not found");
			}
		}
	}
}