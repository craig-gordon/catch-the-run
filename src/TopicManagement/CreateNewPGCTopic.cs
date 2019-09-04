using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.SNSEvents;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.SimpleNotificationService;
using System.Net.Http;
using System;

namespace TopicManagement
{
    public class CreateNewPGCTopic
    {
        public CreateNewPGCTopic()
        {
           
        }

        [LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
        public void Execute(APIGatewayProxyRequest req, ILambdaContext ctx)
        {
            ctx.Logger.LogLine($"Lambda Context: {ctx}");
            ctx.Logger.LogLine($"Received HTTP request at {DateTime.UtcNow}");
            ctx.Logger.LogLine($"Resource: {req.Resource}");
            ctx.Logger.LogLine($"Path: {req.Path}");
            ctx.Logger.LogLine($"HttpMethod: {req.HttpMethod}");
            ctx.Logger.LogLine($"MultiValueHeaders: {req.MultiValueHeaders}");
            ctx.Logger.LogLine($"QueryStringParameters: {req.QueryStringParameters}");
            ctx.Logger.LogLine($"MultiValueQueryStringParameters: {req.MultiValueQueryStringParameters}");
            ctx.Logger.LogLine($"PathParameters: {req.PathParameters}");
            ctx.Logger.LogLine($"StageVariables: {req.StageVariables}");
            ctx.Logger.LogLine($"RequestContext: {req.RequestContext}");
            ctx.Logger.LogLine($"Body: {req.Body}");
            ctx.Logger.LogLine($"IsBase64Encoded: {req.IsBase64Encoded}");
            ctx.Logger.LogLine($"RequestIdentity: {req.RequestContext.Identity}");
            var SNSClient = new AmazonSimpleNotificationServiceClient();
        }
    }
}
