namespace NotificationProcessing
{
    public class PlayerEventRequestBody
    {
        public string Player { get; set; }
        public string Game { get; set; }
        public string Category { get; set; }
        public string SplitName { get; set; }
        public double CurrentPace { get; set; }
        public string Message { get; set; }

    }
}
