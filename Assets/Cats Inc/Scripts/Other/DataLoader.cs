using System.IO;
using Newtonsoft.Json.Linq;
using UnityEngine.Device;
using File = System.IO.File;

namespace Cats_Inc.Scripts.Other
{
	public static class DataLoader
	{
		private static readonly string path = Application.persistentDataPath;

		//Checks if basic game data is present, creates first time playing data if not
		public static void CheckSavedData()
		{
			if (!File.Exists(path + "/Data.txt"))
			{
				var json = new JObject { { "RecentFactory", 0 } };

				var writer = new StreamWriter(File.Create(path + "/Data.txt"));
				writer.WriteLine(json.ToString());
				
				writer.Close();
			}
		}
	}
}