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

    public class AugmentedMessageObject
    {
        public string UserId { get; set; }
        public string Text { get; set; }
        public string ExtraData { get; set; }
    }
}
