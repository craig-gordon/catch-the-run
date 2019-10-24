namespace CatchTheRun.SubscriptionManagement
{
    public class CreateSubscriptionRequestBody
    {
        public string ProviderName { get; set; }
        public string ConsumerName { get; set; }
        public string Protocol { get; set; }
        public string Endpoint { get; set; }
        public string DiscordServerId { get; set; }
        public FilterPolicy FilterPolicy { get; set; }
    }
}
