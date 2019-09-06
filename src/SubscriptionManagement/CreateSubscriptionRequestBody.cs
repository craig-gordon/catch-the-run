namespace SubscriptionManagement
{
    public class CreateSubscriptionRequestBody
    {
        public string Protocol { get; set; }
        public string Endpoint { get; set; }
        public string Player { get; set; }
        public string Game { get; set; }
        public string Category { get; set; }
    }
}
