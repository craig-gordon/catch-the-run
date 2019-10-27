using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using System.Net.Http;
using System;
using Newtonsoft.Json;
using Microsoft.Extensions.DependencyInjection;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace CatchTheRun.EventProcessing
{
    public class ProcessEvent
    {
        private readonly IServiceProvider serviceProvider;
        public ProcessEvent() : this(EventProcessingConfiguration.BuildContainer().BuildServiceProvider())
        {
        }

        public ProcessEvent(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        [LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
        public async Task<APIGatewayProxyResponse> Execute(APIGatewayProxyRequest req, ILambdaContext ctx)
        {
            ctx.Logger.LogLine($"Lambda Context: {JsonConvert.SerializeObject(ctx)}");
            ctx.Logger.LogLine($"Create Subscription Request: {JsonConvert.SerializeObject(req)}");

            var reqBody = JsonConvert.DeserializeObject<EventRequestBody>(req.Body);

            using (var DynamoClient = serviceProvider.GetService<IAmazonDynamoDB>())
            {
                var query = new QueryRequest()
                {
                    TableName = "Main",
                    KeyConditionExpression = $"PRT = :PRT and begins_with (SRT, :SRT)",
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue>()
                    {
                        { ":PRT", new AttributeValue() { S = reqBody.ProviderName } },
                        { ":SRT", new AttributeValue() { S = "F|SUB" } }
                    }
                };
                var getSubsResponse = await DynamoClient.QueryAsync(query);
                
                ctx.Logger.LogLine($"Subs: {JsonConvert.SerializeObject(getSubsResponse)}");

                return new APIGatewayProxyResponse() { StatusCode = 200 };
            }
        }
    }
}
