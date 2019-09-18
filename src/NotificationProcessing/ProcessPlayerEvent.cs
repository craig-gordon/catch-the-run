using System.Threading;
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

namespace NotificationProcessing
{
    public class ProcessPlayerEvent
    {
        private readonly IServiceProvider serviceProvider;
        public ProcessPlayerEvent() : this(NotificationProcessingConfiguration.BuildContainer().BuildServiceProvider())
        {
        }

        public ProcessPlayerEvent(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        [LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
        public async Task<APIGatewayProxyResponse> Execute(APIGatewayProxyRequest req, ILambdaContext ctx)
        {
            ctx.Logger.LogLine($"Lambda Context: {JsonConvert.SerializeObject(ctx)}");
            ctx.Logger.LogLine($"Resource: {req.Resource}");
            ctx.Logger.LogLine($"Path: {req.Path}");
            ctx.Logger.LogLine($"HttpMethod: {req.HttpMethod}");
            ctx.Logger.LogLine($"Body: {req.Body}");
            ctx.Logger.LogLine($"RequestContext: {JsonConvert.SerializeObject(req.RequestContext)}");

            var reqBody = JsonConvert.DeserializeObject<PlayerEventRequestBody>(req.Body);
            using (var SNSClient = this.serviceProvider.GetService<IAmazonSimpleNotificationService>())
            {
                var topic = await SNSClient.FindTopicAsync(reqBody.Player);
                var publishRequest = new PublishRequest() { TopicArn = topic.TopicArn, Message = reqBody.Message };
                var res = await SNSClient.PublishAsync(publishRequest, new CancellationTokenSource().Token);
                if (res.MessageId != null)
                    return new APIGatewayProxyResponse() { StatusCode = 200 };
                else
                    return new APIGatewayProxyResponse() { StatusCode = 400 };
            }
        }
    }
}
