using System.Collections.Generic;
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

namespace SubscriptionManagement
{
    public class CreateSubscription
    {
        private readonly IServiceProvider serviceProvider;
        public CreateSubscription() : this(SubscriptionManagementConfiguration.BuildContainer().BuildServiceProvider())
        {
        }

        public CreateSubscription(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        [LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
        public async Task<SubscribeResponse> Execute(APIGatewayProxyRequest req, ILambdaContext ctx)
        {
            ctx.Logger.LogLine($"Lambda Context: {JsonConvert.SerializeObject(ctx)}");
            ctx.Logger.LogLine($"Resource: {req.Resource}");
            ctx.Logger.LogLine($"Path: {req.Path}");
            ctx.Logger.LogLine($"HttpMethod: {req.HttpMethod}");
            ctx.Logger.LogLine($"Body: {req.Body}");
            ctx.Logger.LogLine($"RequestContext: {JsonConvert.SerializeObject(req.RequestContext)}");

            var reqBody = JsonConvert.DeserializeObject<CreateSubscriptionRequestBody>(req.Body);
            using (var SNSClient = this.serviceProvider.GetService<IAmazonSimpleNotificationService>())
            {
                var topic = await SNSClient.FindTopicAsync(reqBody.Player);
                var subscribeRequest = new SubscribeRequest() { TopicArn = topic.TopicArn, Endpoint = reqBody.Endpoint, ReturnSubscriptionArn = true };
                switch (reqBody.Protocol)
                {
                    case "SMS":
                        subscribeRequest.Protocol = SubscriptionProtocols.SMS;
                        break;
                    case "Email":
                        subscribeRequest.Protocol = SubscriptionProtocols.Email;
                        break;
                    case "WebPush":
                        subscribeRequest.Protocol = SubscriptionProtocols.WebPush;
                        break;
                    case "AppPush":
                        subscribeRequest.Protocol = SubscriptionProtocols.AppPush;
                        break;
                    case "HTTPS":
                        subscribeRequest.Protocol = SubscriptionProtocols.HTTPS;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                var source = new CancellationTokenSource();
                var cancellationToken = source.Token;

                var res = await SNSClient.SubscribeAsync(subscribeRequest, cancellationToken);
                return res;
            }
        }
    }
}