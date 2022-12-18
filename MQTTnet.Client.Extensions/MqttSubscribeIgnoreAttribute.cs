using System;
using System.Collections.Generic;
using System.Text;

namespace MQTTnet.Client.Extensions
{
    /// <summary>
    /// MQTT订阅忽略
    /// </summary>
    /// <remarks>使用通配符进行订阅时，对应的方法可以打上该特性避免重复订阅</remarks>
    [AttributeUsage(AttributeTargets.Method)]
    public class MqttSubscribeIgnoreAttribute : Attribute
    {
        public MqttSubscribeIgnoreAttribute() { }
    }
}
