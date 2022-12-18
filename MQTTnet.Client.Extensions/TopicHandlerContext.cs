using System;
using System.Collections.Generic;
using System.Text;

namespace MQTTnet.Client.Extensions
{
    /// <summary>
    /// MQTT主题订阅处理上下文
    /// </summary>
    public class TopicHandlerContext
    {
        /// <summary>
        /// 自己的ID
        /// </summary>
        public string ClientId { get; internal set; }

        /// <summary>
        /// 消息
        /// </summary>
        public MqttApplicationMessage ApplicationMessage { get; internal set; }

        /// <summary>
        /// 过滤器+执行中遇到的最后一个异常
        /// </summary>
        public Exception LastException { get; set; }

        /// <summary>
        /// 是否中止执行其余过滤器
        /// </summary>
        public bool IsBreak { get; set; }
    }
}
