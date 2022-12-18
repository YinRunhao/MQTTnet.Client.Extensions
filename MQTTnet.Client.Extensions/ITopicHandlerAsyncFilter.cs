using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MQTTnet.Client.Extensions
{
    /// <summary>
    /// 主题订阅处理异步过滤器
    /// </summary>
    public interface ITopicHandlerAsyncFilter
    {
        /// <summary>
        /// 执行前调用
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Task OnHandlerExecutingAsync(TopicHandlerContext context);

        /// <summary>
        /// 执行后调用
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Task OnHandlerExecutedAsync(TopicHandlerContext context);
    }
}
