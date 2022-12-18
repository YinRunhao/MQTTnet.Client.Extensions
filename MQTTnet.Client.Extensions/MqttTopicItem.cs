using MQTTnet.Protocol;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MQTTnet.Client.Extensions
{
    /// <summary>
    /// 订阅主题信息
    /// </summary>
    public class MqttTopicItem
    {
        /// <summary>
        /// 主题名
        /// </summary>
        public string Topic { get; private set; }

        /// <summary>
        /// 服务质量
        /// </summary>
        public MqttQualityOfServiceLevel QoS { get; private set; }

        /// <summary>
        /// 主题别名
        /// </summary>
        public ushort? TopicAlias { get; private set; }

        /// <summary>
        /// 订阅处理信息
        /// </summary>
        public MqttTopicHandlerInfo HandlerInfo { get; private set; }

        public MqttTopicItem()
        {
        }

        internal void SetTopicAlias(ushort val)
        {
            TopicAlias = val;
        }

        internal void SetQos(MqttQualityOfServiceLevel val)
        {
            QoS = val;
        }

        internal void SetTopic(string val)
        {
            Topic = val;
        }

        internal void SetHandlerInfo(MqttTopicHandlerInfo info)
        {
            HandlerInfo = info;
        }
    }
}
