using System.IO;
using Cats_Inc.Scripts.Player;
using Newtonsoft.Json.Linq;
using UnityEngine.Device;
using File = System.IO.File;

namespace Cats_Inc.Scripts.Other
{
	public enum DataVars {
		RecentFactory,
	}
	
	public enum StatSection
	{
		General,
		Import,
		Production,
		Export,
	}

	public enum GeneralVars
	{
		AmountOfLanes,
	}

	public enum ImportVars
	{
		DockSize,
		AmountOfDocks,
		MoverSize,
		AmountOfMovers,
		RackSize,
		AmountOfRacks,
	}
	
	public static class DataManager
	{
		private static readonly string path = Application.persistentDataPath;
		private static JObject data;
		private static JObject stats;

		//Checks if basic game data is present, creates first time playing data if not
		public static void CheckSavedData()
		{
			//First time playing
			if (!File.Exists(path + "/Data.txt"))
			{
				//Data
				data = new JObject { { DataVars.RecentFactory.ToString(), 0 } };

				var dataWriter = new StreamWriter(File.Create(path + "/Data.txt"));
				dataWriter.WriteLine(data.ToString());
				
				dataWriter.Close(); 
				
				//Factory 0
				stats = new JObject { {GeneralVars.AmountOfLanes.ToString(), 1} };
				var import = new JObject
				{
					{ImportVars.DockSize.ToString(),1},
					{ImportVars.AmountOfDocks.ToString(),1},
					{ImportVars.MoverSize.ToString(),1},
					{ImportVars.AmountOfMovers.ToString(),1},
					{ImportVars.RackSize.ToString(),1},
					{ImportVars.AmountOfRacks.ToString(),1},
				};
				stats.Add(StatSection.Import.ToString(), import);
			
				var statsWriter = new StreamWriter(File.Create(path + "/Factory0.txt"));
				statsWriter.Write(stats.ToString());
				statsWriter.Close();
			}
			else //Normal Startup
			{
				var dataReader = new StreamReader(File.Open(path + "/Data.txt", FileMode.Open));
				data = JObject.Parse(dataReader.ReadToEnd());
			}
		}

		public static void LoadFactory(int index)
		{
			var statsReader = new StreamReader(File.Open(path + $"/Factory{index}.txt", FileMode.Open));
			stats = JObject.Parse(statsReader.ReadToEnd());
		}

		public static JToken GetData(DataVars var)
		{
			return data[var.ToString()];
		}

		public static int GetLevel(GeneralVars stat)
		{
			return GetLevel(StatSection.General.ToString(), stat.ToString());
		}
		
		public static int GetLevel(ImportVars stat)
		{
			return GetLevel(StatSection.Import.ToString(), stat.ToString());
		}

		private static int GetLevel(string sectionName, string stat)
		{
			//Get Section
			var sectionJson = stats[sectionName];
			
			//1 if section == null
				//1 if stat == null
				//Actual Return
			return sectionJson == null ? 1 : 
				sectionJson[stat] == null ? 1 :
				sectionJson[stat].ToObject<int>();
		}
	}
}