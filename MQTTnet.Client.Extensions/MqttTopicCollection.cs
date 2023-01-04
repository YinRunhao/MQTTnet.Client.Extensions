using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace MQTTnet.Client.Extensions
{
    /// <summary>
    /// 主题集合
    /// </summary>
    public class MqttTopicCollection : IEnumerable<MqttTopicItem>
    {
        private List<MqttTopicItem> m_Items;

        private List<string> m_LoadedAssembly;

        private static readonly MqttTopicCollection s_Instance = new MqttTopicCollection();

        private MqttTopicCollection()
        {
            m_Items = new List<MqttTopicItem>();
            m_LoadedAssembly = new List<string>();
        }

        /// <summary>
        /// 获取单例
        /// </summary>
        /// <returns></returns>
        public static MqttTopicCollection GetInstance()
        {
            return s_Instance;
        }

        /// <summary>
        /// 清理所有已注册的主题
        /// </summary>
        /// <remarks>请谨慎调用并自行处理依赖注入容器的注入</remarks>
        /// <returns></returns>
        public MqttTopicCollection Clear()
        {
            m_Items.Clear();
            m_LoadedAssembly.Clear();
            return this;
        }

        /// <summary>
        /// 扫描程序集中的TopicHandler和方法添加主题
        /// </summary>
        /// <param name="assembly"></param>
        /// <remarks>自行调用时需自行向依赖注入容器注入</remarks>
        /// <returns></returns>
        /// <exception cref="TopicHandlerDeclareException"></exception>
        public MqttTopicCollection AddTopics(Assembly assembly)
        {
            if (assembly != null)
            {
                if (!m_LoadedAssembly.Contains(assembly.FullName))
                {
                    m_LoadedAssembly.Add(assembly.FullName);
                    var topicType = typeof(TopicHandler);
                    var tps = assembly.GetTypes();
                    foreach (var t in tps)
                    {
                        if (topicType.IsAssignableFrom(t))
                        {
                            AddTopicByClass(t);
                        }
                    }
                }
            }
            return this;
        }

        public IEnumerator<MqttTopicItem> GetEnumerator()
        {
            return ((IEnumerable<MqttTopicItem>)m_Items).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)m_Items).GetEnumerator();
        }

        private void AddTopicByClass(Type type)
        {
            string prefix = string.Empty;
            var tpAttr = type.GetCustomAttribute<MqttTopicAttribute>();
            if (tpAttr != null)
            {
                prefix = tpAttr.Topic.ToLower();
            }
            else
            {
                prefix = type.Name.ToLower();
                int idx = prefix.LastIndexOf("handler");
                if (idx > 0)
                {
                    prefix = prefix.Substring(0, idx);
                }
            }
            if (prefix[prefix.Length - 1] != '/')
            {
                prefix += "/";
            }
            // 特性里没设置主题
            if (prefix.Length < 1)
            {
                return;
            }
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            MqttTopicItem item = default;
            string topicExp = string.Empty;
            foreach (var method in methods)
            {
                item = new MqttTopicItem();
                var attr = method.GetCustomAttribute<MqttTopicAttribute>();
                if (attr != null)
                {
                    //item.SetTopic(prefix + attr.Topic);
                    topicExp = prefix + attr.Topic;
                    item.SetQos(attr.QoS);
                }
                else
                {
                    //item.SetTopic(prefix + method.Name.ToLower());
                    topicExp = prefix + method.Name.ToLower();
                    if (tpAttr != null)
                    {
                        item.SetQos(tpAttr.QoS);
                    }
                }
                // 校验主题中占位符的格式
                if (Utility.CheckTopicFormat(topicExp))
                {
                    item.SetTopic(topicExp);
                }
                else
                {
                    throw new TopicHandlerDeclareException(type.Name, method.Name, $"订阅主题[{topicExp}]占位符格式不正确");
                }

                var info = GetHandlerInfo(type, method);
                item.SetHandlerInfo(info);

                m_Items.Add(item);
            }
        }

        /// <summary>
        /// 检查方法定义并获取处理信息
        /// </summary>
        /// <param name="handlerTp"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        /// <exception cref="TopicHandlerDeclareException"></exception>
        private MqttTopicHandlerInfo GetHandlerInfo(Type handlerTp, MethodInfo method)
        {
            // 是否异步
            var asyncAttr = method.GetCustomAttribute<AsyncStateMachineAttribute>();
            bool isAwaitable = asyncAttr != null;
            // 检查返回值
            if (isAwaitable)
            {
                if (method.ReturnType.IsGenericType)
                {
                    throw new TopicHandlerDeclareException(handlerTp.Name, method.Name, "方法返回值必须为 void 或 async Task");
                }
            }
            else
            {
                if (method.ReturnType != typeof(void))
                {
                    throw new TopicHandlerDeclareException(handlerTp.Name, method.Name, "方法返回值必须为 void 或 async Task");
                }
            }

            // 检查参数
            if (method.GetParameters().Length > 0)
            {
                throw new TopicHandlerDeclareException(handlerTp.Name, method.Name, "方法不能带有参数");
            }

            MqttTopicHandlerInfo info = new MqttTopicHandlerInfo(handlerTp, method, isAwaitable);
            return info;
        }
    }
}
