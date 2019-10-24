using System.Collections.Generic;

namespace CatchTheRun.SubscriptionManagement
{
    public class FilterPolicy
    {
        public string Type { get; set; }
        public List<string> Games { get; set; }
        public List<string> Categories { get; set; }
    }
}
