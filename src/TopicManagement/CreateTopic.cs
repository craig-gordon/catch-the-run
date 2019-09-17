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
    public class CreateTopic
    {
        private readonly IServiceProvider serviceProvider;
        public CreateTopic() : this(TopicManagementConfiguration.BuildContainer().BuildServiceProvider())
        {
        }

        public CreateTopic(IServiceProvider serviceProvider)
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
                return await SNSClient.CreateTopicAsync(reqBody.Player);
            }
        }
    }
}
