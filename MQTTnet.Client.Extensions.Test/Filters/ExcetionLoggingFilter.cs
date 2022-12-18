using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTTnet.Client.Extensions.Test.Filters
{
    public class ExcetionLoggingFilter : ITopicHandlerAsyncFilter
    {
        public Task OnHandlerExecutedAsync(TopicHandlerContext context)
        {
            if (context.LastException != null)
            {
                context.ApplicationMessage.ResponseTopic = "get last exception";
                return Task.CompletedTask;
            }
            else
            {
                return Task.CompletedTask;
            }
        }

        public Task OnHandlerExecutingAsync(TopicHandlerContext context)
        {
            return Task.CompletedTask;
        }
    }
}
