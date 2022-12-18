using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MQTTnet.Client.Extensions
{
    public class MqttTopicHandlerInfo
    {
        /// <summary>
        /// 处理器类型
        /// </summary>
        public Type HandlerType { get; private set; }

        /// <summary>
        /// 处理方法
        /// </summary>
        public MethodInfo HandleMethod { get; private set; }

        /// <summary>
        /// 是否异步方法
        /// </summary>
        public bool IsAwaitable { get; private set; }

        public MqttTopicHandlerInfo(Type handlerType, MethodInfo handleMethod, bool isAwaitable)
        {
            HandlerType = handlerType;
            HandleMethod = handleMethod;
            IsAwaitable = isAwaitable;
        }
    }
}
