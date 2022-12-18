using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MQTTnet.Client.Extensions
{
    /// <summary>
    /// 配置选项
    /// </summary>
    public class MqttTopicHandlerOptions
    {
        internal List<Assembly> HandlerAssembies { get; private set; }

        internal List<Type> FilterTypes { get; private set; }

        public MqttTopicHandlerOptions()
        {
            HandlerAssembies = new List<Assembly>();
            FilterTypes = new List<Type>();
        }

        /// <summary>
        /// 扫描程序集中的TopicHandler和方法添加主题
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public MqttTopicHandlerOptions AddMqttTopicHandlers(Assembly assembly)
        {
            HandlerAssembies.Add(assembly);
            return this;
        }

        /// <summary>
        /// 添加过滤器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public MqttTopicHandlerOptions AddHandlerFilter<T>() where T : ITopicHandlerFilter
        {
            FilterTypes.Add(typeof(T));
            return this;
        }

        /// <summary>
        /// 添加异步过滤器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public MqttTopicHandlerOptions AddAsyncHandlerFilter<T>() where T : ITopicHandlerAsyncFilter
        {
            FilterTypes.Add(typeof(T));
            return this;
        }
    }
}
