using System;
using System.Collections.Generic;
using System.Text;

namespace TopicManagement
{
    public class CreateTopicRequestBody
    {
        public string Player { get; set; }
        public string Game { get; set; }
        public string Category { get; set; }
    }
}
