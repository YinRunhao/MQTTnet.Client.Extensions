using MQTTnet.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace MQTTnet.Client.Extensions
{
    /// <summary>
    /// MQTT主题
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class MqttTopicAttribute : Attribute
    {
        /// <summary>
        /// 主题名
        /// </summary>
        public string Topic { get; set; }

        /// <summary>
        /// 服务质量
        /// </summary>
        public MqttQualityOfServiceLevel QoS { get; set; }

        public MqttTopicAttribute(string topic, MqttQualityOfServiceLevel qos)
        {
            Topic = topic;
            QoS = qos;
        }

        public MqttTopicAttribute(string topic)
        {
            Topic = topic;
            QoS = MqttQualityOfServiceLevel.AtMostOnce;
        }
    }
}
