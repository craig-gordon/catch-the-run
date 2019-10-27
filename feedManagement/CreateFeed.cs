using System.Threading.Tasks;
using System.Threading;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using System.Net.Http;
using System.Net;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;
using Microsoft.Extensions.DependencyInjection;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace CatchTheRun.FeedManagement
{
    public class CreateFeed
    {
        private readonly IServiceProvider serviceProvider;
        public CreateFeed() : this(TopicManagementConfiguration.BuildContainer().BuildServiceProvider())
        {
        }

        public CreateFeed(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        [LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
        public async Task<APIGatewayProxyResponse> Execute(APIGatewayProxyRequest req, ILambdaContext ctx)
        {
            ctx.Logger.LogLine($"Lambda Context: {JsonConvert.SerializeObject(ctx)}");
            ctx.Logger.LogLine($"CreateFeedRequest: {JsonConvert.SerializeObject(req)}");

            var reqBody = JsonConvert.DeserializeObject<CreateFeedRequestBody>(req.Body);

            using (var DynamoClient = serviceProvider.GetService<IAmazonDynamoDB>())
            {
                var attributes = new Dictionary<string, AttributeValue>();
                attributes["PRT"] = new AttributeValue() { S = reqBody.ProviderName };
                attributes["SRT"] = new AttributeValue() { S = "F" };
                attributes["G1S"] = new AttributeValue() { S = reqBody.ProviderName };
                var putItemRequest = new PutItemRequest() { TableName = "Main", Item = attributes };
                var putItemResponse = await DynamoClient.PutItemAsync(putItemRequest, new CancellationTokenSource().Token);

                if (putItemResponse != null)
                {
                    return new APIGatewayProxyResponse() { StatusCode = 200, Headers = new Dictionary<string, string> { { "Access-Control-Allow-Origin", "*" } } };
                }
                else
                {
                    return new APIGatewayProxyResponse() { StatusCode = 400, Headers = new Dictionary<string, string> { { "Access-Control-Allow-Origin", "*" } } };
                }
            }
        }
    }
}
