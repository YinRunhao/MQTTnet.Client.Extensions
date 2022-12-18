using System;

namespace MQTTnet.Client.Extensions
{
    /// <summary>
    /// MQTT主题订阅处理器
    /// </summary>
    public class TopicHandler
    {
        /// <summary>
        /// 自己的Id
        /// </summary>
        public string ClietntId { get; internal set; }

        /// <summary>
        /// 消息
        /// </summary>
        public MqttApplicationMessage ApplicationMessage { get; internal set; }

        /// <summary>
        /// 上下文
        /// </summary>
        public TopicHandlerContext Context { get; internal set; }
    }
}
