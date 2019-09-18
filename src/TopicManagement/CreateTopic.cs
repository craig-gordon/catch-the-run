using System.Threading.Tasks;
using System.Threading;
using Amazon.Lambda.Core;
using Amazon.Lambda.SNSEvents;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using System.Net.Http;
using System.Net;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;
using Microsoft.Extensions.DependencyInjection;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

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
        public async Task<APIGatewayProxyResponse> Execute(APIGatewayProxyRequest req, ILambdaContext ctx)
        {
            ctx.Logger.LogLine($"Lambda Context: {JsonConvert.SerializeObject(ctx)}");
            ctx.Logger.LogLine($"Resource: {req.Resource}");
            ctx.Logger.LogLine($"Path: {req.Path}");
            ctx.Logger.LogLine($"HttpMethod: {req.HttpMethod}");
            ctx.Logger.LogLine($"Body: {req.Body}");
            ctx.Logger.LogLine($"RequestContext: {JsonConvert.SerializeObject(req.RequestContext)}");

            var reqBody = JsonConvert.DeserializeObject<CreateTopicRequestBody>(req.Body);
            using (var SNSClient = serviceProvider.GetService<IAmazonSimpleNotificationService>())
            using (var DynamoClient = serviceProvider.GetService<IAmazonDynamoDB>())
            {
                var topicResponse = await SNSClient.CreateTopicAsync(reqBody.Player);
                if (topicResponse.TopicArn != null)
                {
                    var attributes = new Dictionary<string, AttributeValue>();
                    attributes["id"] = new AttributeValue() { S = Guid.NewGuid().ToString() };
                    attributes["name"] = new AttributeValue() { S = reqBody.Player };
                    attributes["topicArn"] = new AttributeValue() { S = topicResponse.TopicArn };
                    var putItemRequest = new PutItemRequest() { TableName = "players", Item = attributes };
                    var putItemResponse = await DynamoClient.PutItemAsync(putItemRequest, new CancellationTokenSource().Token);
                    return new APIGatewayProxyResponse() { StatusCode = 200 };
                }
                else
                {
                    return new APIGatewayProxyResponse() { StatusCode = 400 };
                }
            }
        }
    }
}
