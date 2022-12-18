using System;
using System.Collections.Generic;
using System.Text;

namespace MQTTnet.Client.Extensions
{
    /// <summary>
    /// 主题处理定义异常
    /// </summary>
    public class TopicHandlerDeclareException : ApplicationException
    {
        /// <summary>
        /// 类名
        /// </summary>
        public string ClassName { get; private set; }

        /// <summary>
        /// 方法名
        /// </summary>
        public string MethodName { get; private set; }

        public TopicHandlerDeclareException(string className, string methodName, string msg) : base(msg)
        {
            ClassName = className;
            MethodName = methodName;
        }
    }
}
