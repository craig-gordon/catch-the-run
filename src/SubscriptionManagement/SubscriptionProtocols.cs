using System;
using System.Collections.Generic;
using System.Text;

namespace SubscriptionManagement
{
    internal static class SubscriptionProtocols
    {
        public const string SMS = "sms";
        public const string Email = "email";
        public const string WebPush = "lambda";
        public const string AppPush = "application";
        public const string HTTPS = "https";
    }
}
