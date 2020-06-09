namespace NetCore.Durable.Functions
{
	public class CosmosDbOptions
	{
		public string Endpoint { get; set; }
		public string Key { get; set; }
		public string Database { get; set; }
		public string Container { get; set; }
    }
}