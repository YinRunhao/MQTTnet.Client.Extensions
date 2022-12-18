using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTTnet.Client.Extensions.Test.Filters
{
    public class BreakFilter : ITopicHandlerFilter
    {
        public void OnHandlerExecuted(TopicHandlerContext context)
        {
            throw new NotImplementedException();
        }

        public void OnHandlerExecuting(TopicHandlerContext context)
        {
            // meet some error here
            if (string.IsNullOrEmpty(context.ApplicationMessage.ResponseTopic))
            {
                context.ApplicationMessage.ResponseTopic = "-Break-";
            }
            else
            {
                context.ApplicationMessage.ResponseTopic += "-Break-";
            }
            context.IsBreak = true;
        }
    }
}
