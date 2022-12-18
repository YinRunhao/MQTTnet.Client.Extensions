using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTTnet.Client.Extensions.Demo
{
    public class AuthFilter : ITopicHandlerAsyncFilter
    {
        public Task OnHandlerExecutedAsync(TopicHandlerContext context)
        {
            // pass
            return Task.CompletedTask;
        }

        public async Task OnHandlerExecutingAsync(TopicHandlerContext context)
        {
            if (CheckAuth(context))
            {
                // 假装在处理
                await Task.Delay(100);
            }
            else
            {
                // 不再往下执行过滤器和处理器
                context.IsBreak = true;
            }
        }

        private bool CheckAuth(TopicHandlerContext context)
        {
            return true;
        }
    }
}
