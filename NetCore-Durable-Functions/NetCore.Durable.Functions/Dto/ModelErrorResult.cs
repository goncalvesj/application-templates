namespace NetCore.Durable.Functions.Dto
{
	public class ModelErrorResult
	{
		public ModelErrorResult(string field, string error)
		{
			Field = field;
			Error = error;
		}

		public string Field { get; }
		public string Error { get; }
	}
}