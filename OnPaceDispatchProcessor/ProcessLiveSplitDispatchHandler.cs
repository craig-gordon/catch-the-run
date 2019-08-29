using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.SNSEvents;

[assembly:LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace OnPaceDispatchProcessor
{
    public class ProcessLiveSplitDispatch
    {
        public ProcessLiveSplitDispatch ()
        {
           
        }

        public async void ProcessDispatch(SNSEvent e, ILambdaContext ctx)
        {
            foreach (var record in e.Records)
            {
                await LogRecordAsync(record, ctx);
            }
        }

        public async Task LogRecordAsync(SNSEvent.SNSRecord record, ILambdaContext ctx)
        {
            ctx.Logger.LogLine($"Processed record with message: {record.Sns.Message}");

            await Task.CompletedTask;
        }
    }
}
