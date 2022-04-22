using Newtonsoft.Json;

namespace SignalR.Functions
{
    public class MessageObject
    {
        [JsonProperty("userId")]
        public string UserId { get; set; }
        [JsonProperty("text")]
        public string Text { get; set; }
    }
}
