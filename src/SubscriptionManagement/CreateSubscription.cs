using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using System;
using Newtonsoft.Json;
using Microsoft.Extensions.DependencyInjection;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace CatchTheRun.SubscriptionManagement
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
        public async Task<APIGatewayProxyResponse> Execute(APIGatewayProxyRequest req, ILambdaContext ctx)
        {
            ctx.Logger.LogLine($"Lambda Context: {JsonConvert.SerializeObject(ctx)}");
            ctx.Logger.LogLine($"Create Subscription Request: {JsonConvert.SerializeObject(req)}");

            var reqBody = JsonConvert.DeserializeObject<CreateSubscriptionRequestBody>(req.Body);

            using (var DynamoClient = serviceProvider.GetService<IAmazonDynamoDB>())
            {
                var key = new Dictionary<string, AttributeValue>()
                {
                    { "PRT", new AttributeValue() { S = reqBody.ProviderName } },
                    { "SRT", new AttributeValue() { S = $"F|SUB|{reqBody.ConsumerName}"} }
                };
                var getSubResponse = await DynamoClient.GetItemAsync("Main", key);
                ctx.Logger.LogLine($"Get Sub Response: {JsonConvert.SerializeObject(getSubResponse)}");

                if (!getSubResponse.IsItemSet)
                {
                    var attributes = new Dictionary<string, AttributeValue>();
                    attributes["PRT"] = new AttributeValue() { S = reqBody.ProviderName };
                    attributes["SRT"] = new AttributeValue() { S = $"F|SUB|{reqBody.ConsumerName}" };
                    attributes["G1S"] = new AttributeValue() { S = reqBody.ProviderName };
                    attributes[reqBody.Protocol] = new AttributeValue()
                    {
                        M = new Dictionary<string, AttributeValue>()
                        {
                            { "Endpoint", new AttributeValue() { S = reqBody.Endpoint } },
                            { "DiscordServerId", new AttributeValue() { S = reqBody.DiscordServerId } },
                            { "FilterPolicy", new AttributeValue()
                                {
                                    M = new Dictionary<string, AttributeValue>()
                                    {
                                        { "PolicyType", new AttributeValue() { S = reqBody.FilterPolicy.Type } },
                                        { "Games", new AttributeValue() { SS = reqBody.FilterPolicy.Games } },
                                        { "Categories", new AttributeValue() { SS = reqBody.FilterPolicy.Categories } }
                                    }
                                }
                            }
                        }
                    };
                    var createSubRequest = new PutItemRequest() { TableName = "Main", Item = attributes };
                    var createSubResponse = await DynamoClient.PutItemAsync(createSubRequest);
                    return new APIGatewayProxyResponse() { StatusCode = 200, Headers = new Dictionary<string, string> { { "Access-Control-Allow-Origin", "*" } } };
                }
                else
                {
                    var sub = getSubResponse.Item;
                    sub[reqBody.Protocol] = new AttributeValue()
                    {
                        M = new Dictionary<string, AttributeValue>()
                        {
                            { "Endpoint", new AttributeValue() { S = reqBody.Endpoint } },
                            { "DiscordServerId", new AttributeValue() { S = reqBody.DiscordServerId } },
                            { "FilterPolicy", new AttributeValue()
                                {
                                    M = new Dictionary<string, AttributeValue>()
                                    {
                                        { "PolicyType", new AttributeValue() { S = reqBody.FilterPolicy.Type } },
                                        { "Games", new AttributeValue() { SS = reqBody.FilterPolicy.Games } },
                                        { "Categories", new AttributeValue() { SS = reqBody.FilterPolicy.Categories } }
                                    }
                                }
                            }
                        }
                    };
                    var addSubInstanceRequest = new PutItemRequest() { TableName = "Main", Item = sub };
                    var addSubInstanceResponse = await DynamoClient.PutItemAsync(addSubInstanceRequest);
                    return new APIGatewayProxyResponse() { StatusCode = 200, Headers = new Dictionary<string, string> { { "Access-Control-Allow-Origin", "*" } } };
                }
            }
        }
    }
}