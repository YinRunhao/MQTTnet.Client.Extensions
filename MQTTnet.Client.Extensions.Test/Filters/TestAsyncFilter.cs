using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTTnet.Client.Extensions.Test.Filters
{
    public class TestAsyncFilter : ITopicHandlerAsyncFilter
    {
        public async Task OnHandlerExecutedAsync(TopicHandlerContext context)
        {
            await Task.Delay(50);
            if (string.IsNullOrEmpty(context.ApplicationMessage.ResponseTopic))
            {
                context.ApplicationMessage.ResponseTopic = "-B-";
            }
            else
            {
                context.ApplicationMessage.ResponseTopic += "-B-";
            }
        }

        public async Task OnHandlerExecutingAsync(TopicHandlerContext context)
        {
            await Task.Delay(50);
            if (string.IsNullOrEmpty(context.ApplicationMessage.ResponseTopic))
            {
                context.ApplicationMessage.ResponseTopic = "-B-";
            }
            else
            {
                context.ApplicationMessage.ResponseTopic += "-B-";
            }
        }
    }
}
