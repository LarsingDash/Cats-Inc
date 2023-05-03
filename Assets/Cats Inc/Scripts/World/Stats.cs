namespace Cats_Inc.Scripts.World
{
	public struct Stats
	{
		public Stats(int importTruckMax, int importMoverMax, int importRackMax)
		{
			//Import
			this.importTruckMax = importTruckMax;
			this.importMoverMax = importMoverMax;
			this.importRackMax = importRackMax;
		}
		
		//Import
		public int importTruckMax;
		public int importMoverMax;
		public int importRackMax;
	}
}