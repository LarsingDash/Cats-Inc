namespace Assets.Scripts.World
{
	public struct Stats
	{
		public Stats(int importTruckMax, int importMoverMax, int importRackMax)
		{
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