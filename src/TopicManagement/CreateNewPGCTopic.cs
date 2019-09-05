using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.SNSEvents;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using System.Net.Http;
using System;
using Newtonsoft.Json;
using Microsoft.Extensions.DependencyInjection;

[assembly:LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace TopicManagement
{
    public class CreateNewPGCTopic
    {
        private readonly IServiceProvider serviceProvider;
        public CreateNewPGCTopic() : this(TopicManagementConfiguration.BuildContainer().BuildServiceProvider())
        {
        }

        public CreateNewPGCTopic(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public async Task<CreateTopicResponse> Execute(APIGatewayProxyRequest req, ILambdaContext ctx)
        {
            ctx.Logger.LogLine($"Lambda Context: {ctx}");
            ctx.Logger.LogLine($"Received HTTP request at {DateTime.UtcNow}");
            ctx.Logger.LogLine($"Resource: {req.Resource}");
            ctx.Logger.LogLine($"Path: {req.Path}");
            ctx.Logger.LogLine($"HttpMethod: {req.HttpMethod}");
            ctx.Logger.LogLine($"RequestContext: {req.RequestContext}");
            ctx.Logger.LogLine($"Body: {req.Body}");
            ctx.Logger.LogLine($"IsBase64Encoded: {req.IsBase64Encoded}");
            ctx.Logger.LogLine($"RequestIdentity: {req.RequestContext.Identity}");
            var reqBody = JsonConvert.DeserializeObject<CreateTopicRequestBody>(req.Body);
            var SNSClient = this.serviceProvider.GetService<IAmazonSimpleNotificationService>();
            ctx.Logger.LogLine($"SNSClient: {JsonConvert.SerializeObject(SNSClient)}");
            var res = await SNSClient.CreateTopicAsync($"{TruncateSpaces(reqBody.Player)}_{TruncateSpaces(reqBody.Game)}_{TruncateSpaces(reqBody.Category)}");
            SNSClient.Dispose();
            return res;
        }

        private static string TruncateSpaces(string str)
        {
            str = str.Replace(" ", String.Empty);
            return str.Replace("%", String.Empty);
        }
    }
}
