using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTTnet.Client.Extensions.Test.Filters
{
    public class TestFilter : ITopicHandlerFilter
    {
        public void OnHandlerExecuted(TopicHandlerContext context)
        {
            if (string.IsNullOrEmpty(context.ApplicationMessage.ResponseTopic))
            {
                context.ApplicationMessage.ResponseTopic = "-A-";
            }
            else
            {
                context.ApplicationMessage.ResponseTopic += "-A-";
            }
        }

        public void OnHandlerExecuting(TopicHandlerContext context)
        {
            if (string.IsNullOrEmpty(context.ApplicationMessage.ResponseTopic))
            {
                context.ApplicationMessage.ResponseTopic = "-A-";
            }
            else
            {
                context.ApplicationMessage.ResponseTopic += "-A-";
            }
        }
    }
}
