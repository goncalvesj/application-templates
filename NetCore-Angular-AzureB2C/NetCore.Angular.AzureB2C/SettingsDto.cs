using System.Collections.Generic;

namespace NetCore.Angular.AzureB2C
{
	public class SettingsDto
	{
		public string Instance { get; set; }
		public string ClientId { get; set; }
		public string Domain { get; set; }
		public string SignUpSignInPolicyId { get; set; }
		public List<string> Scope { get; set; }
		public string Authority => $"{Instance.Replace("/tfp/", string.Empty)}/{Domain}/{SignUpSignInPolicyId}";
	}
}