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

        [LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
        public async Task<CreateTopicResponse> Execute(APIGatewayProxyRequest req, ILambdaContext ctx)
        {
            ctx.Logger.LogLine($"Lambda Context: {JsonConvert.SerializeObject(ctx)}");
            ctx.Logger.LogLine($"Resource: {req.Resource}");
            ctx.Logger.LogLine($"Path: {req.Path}");
            ctx.Logger.LogLine($"HttpMethod: {req.HttpMethod}");
            ctx.Logger.LogLine($"Body: {req.Body}");
            ctx.Logger.LogLine($"RequestContext: {JsonConvert.SerializeObject(req.RequestContext)}");

            var reqBody = JsonConvert.DeserializeObject<CreateTopicRequestBody>(req.Body);
            using (var SNSClient = this.serviceProvider.GetService<IAmazonSimpleNotificationService>())
            {
                var res = await SNSClient.CreateTopicAsync($"{TruncateSpaces(reqBody.Player)}_{TruncateSpaces(reqBody.Game)}_{TruncateSpaces(reqBody.Category)}");
                return res;
            }
        }

        private static string TruncateSpaces(string str)
        {
            str = str.Replace(" ", String.Empty);
            return str.Replace("%", String.Empty);
        }
    }
}
