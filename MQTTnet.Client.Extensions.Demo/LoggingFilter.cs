using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTTnet.Client.Extensions.Demo
{
    public class LoggingFilter : ITopicHandlerFilter
    {
        public void OnHandlerExecuted(TopicHandlerContext context)
        {
            if (context.LastException != null)
            {
                // 遇到异常，记录
            }
        }

        public void OnHandlerExecuting(TopicHandlerContext context)
        {
            // 记录收到的消息
        }
    }
}
