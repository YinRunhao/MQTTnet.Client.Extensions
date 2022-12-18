using System;
using System.Collections.Generic;
using System.Text;

namespace MQTTnet.Client.Extensions
{
    /// <summary>
    /// 主题订阅处理过滤器
    /// </summary>
    public interface ITopicHandlerFilter
    {
        /// <summary>
        /// 执行前调用
        /// </summary>
        /// <param name="context"></param>
        void OnHandlerExecuting(TopicHandlerContext context);

        /// <summary>
        /// 执行后调用
        /// </summary>
        /// <param name="context"></param>
        void OnHandlerExecuted(TopicHandlerContext context);
    }
}
