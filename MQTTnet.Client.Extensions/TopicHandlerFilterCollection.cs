using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace MQTTnet.Client.Extensions
{
    /// <summary>
    /// 过滤器集合
    /// </summary>
    public class TopicHandlerFilterCollection : IEnumerable<Type>
    {
        private List<Type> m_Types;

        private static readonly TopicHandlerFilterCollection s_Instance = new TopicHandlerFilterCollection();

        private TopicHandlerFilterCollection()
        {
            m_Types = new List<Type>();
        }

        /// <summary>
        /// 获取单例
        /// </summary>
        /// <returns></returns>
        public static TopicHandlerFilterCollection GetInstance()
        {
            return s_Instance;
        }

        /// <summary>
        /// 清理所有过滤器
        /// </summary>
        /// <remarks>请谨慎调用并自行处理依赖注入容器的注入</remarks>
        /// <returns></returns>
        public TopicHandlerFilterCollection Clear()
        {
            m_Types.Clear();
            return this;
        }

        /// <summary>
        /// 添加过滤器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <remarks>自行调用时需自行向依赖注入容器注入</remarks>
        /// <returns></returns>
        public TopicHandlerFilterCollection AddHandlerFilter<T>() where T : ITopicHandlerFilter
        {
            m_Types.Add(typeof(T));
            return this;
        }

        /// <summary>
        /// 添加异步过滤器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <remarks>自行调用时需自行向依赖注入容器注入</remarks>
        /// <returns></returns>
        public TopicHandlerFilterCollection AddAsyncHandlerFilter<T>() where T : ITopicHandlerAsyncFilter
        {
            m_Types.Add(typeof(T));
            return this;
        }

        /// <summary>
        /// 添加过滤器
        /// </summary>
        /// <param name="t"></param>
        /// <remarks>自行调用时需自行向依赖注入容器注入</remarks>
        /// <returns></returns>
        /// <exception cref="ArgumentException">过滤器未实现接口</exception>
        public TopicHandlerFilterCollection AddHandlerFilter(Type t)
        {
            if (typeof(ITopicHandlerFilter).IsAssignableFrom(t) || typeof(ITopicHandlerAsyncFilter).IsAssignableFrom(t))
            {
                m_Types.Add(t);
                return this;
            }
            else
            {
                throw new ArgumentException("Type should implement ITopicHandlerFilter or ITopicHandlerAsyncFilter");
            }
        }

        public IEnumerator<Type> GetEnumerator()
        {
            return ((IEnumerable<Type>)m_Types).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)m_Types).GetEnumerator();
        }
    }
}
