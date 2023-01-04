using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MQTTnet.Client.Receiving;
using MQTTnet.Client.Subscribing;
using MQTTnet.Client.Unsubscribing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MQTTnet.Client.Extensions
{
    public static class MQTTnetClientExtensions
    {
        /// <summary>
        /// 设置主题占位符变量字典类型
        /// </summary>
        /// <param name="services"></param>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static IServiceCollection UseTopicPlaceholder<T>(this IServiceCollection services) where T : class, ITopicPlaceholderDictionary
        {
            services.AddSingleton<ITopicPlaceholderDictionary, T>();
            return services;
        }

        /// <summary>
        /// 使用指定的MQTT客户端订阅处理器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <param name="act"></param>
        /// <returns></returns>
        public static IServiceCollection UseMqttTopicHandler<T>(this IServiceCollection services, Action<MqttTopicHandlerOptions> act) where T : MqttTopicSubscribeHandler
        {
            services.AddSingleton<IMqttApplicationMessageReceivedHandler, T>();
            services.TryAddSingleton<ITopicPlaceholderDictionary>(sp =>
            {
                return MqttTopicPlaceholderDictionary.GetInstance();
            });
            services.AddSingleton<MqttTopicCollection>(sp =>
            {
                return MqttTopicCollection.GetInstance();
            });
            services.AddSingleton<TopicHandlerFilterCollection>(sp =>
            {
                return TopicHandlerFilterCollection.GetInstance();
            });
            services.TryAddSingleton<MqttTopicPlaceholderDictionary>(sp =>
            {
                return MqttTopicPlaceholderDictionary.GetInstance();
            });

            MqttTopicHandlerOptions options = new MqttTopicHandlerOptions();
            act(options);

            var handlers = MqttTopicCollection.GetInstance();
            var filters = TopicHandlerFilterCollection.GetInstance();

            foreach (var assembly in options.HandlerAssembies)
            {
                handlers.AddTopics(assembly);
            }
            foreach (var handler in handlers)
            {
                services.AddScoped(handler.HandlerInfo.HandlerType);
            }

            foreach (var tp in options.FilterTypes)
            {
                filters.AddHandlerFilter(tp);
            }
            foreach (var filter in filters)
            {
                services.AddScoped(filter);
            }
            return services;
        }

        /// <summary>
        /// 使用MQTT客户端订阅处理器
        /// </summary>
        /// <param name="services"></param>
        /// <param name="act"></param>
        /// <returns></returns>
        public static IServiceCollection UseMqttTopicHandler(this IServiceCollection services, Action<MqttTopicHandlerOptions> act)
        {
            return UseMqttTopicHandler<MqttTopicSubscribeHandler>(services, act);
        }

        /// <summary>
        /// 订阅TopicHandler对应的主题
        /// </summary>
        /// <param name="client"></param>
        /// <remarks>使用默认占位符变量字典解析占位符</remarks>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns></returns>
        public static async Task<MqttClientSubscribeResult> SubscribeTopicsAsync(this IMqttClient client)
        {
            MqttTopicPlaceholderDictionary dic = MqttTopicPlaceholderDictionary.GetInstance();
            return await SubscribeTopicsAsync(client, dic);
        }

        /// <summary>
        /// 取消通过TopicHandler订阅的主题
        /// </summary>
        /// <param name="client"></param>
        /// <remarks>使用默认占位符变量字典解析占位符</remarks>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns></returns>
        public static async Task<MqttClientUnsubscribeResult> UnSubscribeTopicsAsync(this IMqttClient client)
        {
            MqttTopicPlaceholderDictionary dic = MqttTopicPlaceholderDictionary.GetInstance();
            return await UnSubscribeTopicsAsync(client, dic);
        }

        /// <summary>
        /// 订阅TopicHandler对应的主题
        /// </summary>
        /// <param name="client"></param>
        /// <remarks>使用指定占位符变量字典解析占位符</remarks>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns></returns>
        public static async Task<MqttClientSubscribeResult> SubscribeTopicsAsync(this IMqttClient client, ITopicPlaceholderDictionary dic)
        {
            // 分批次 TBD
            MqttTopicCollection collection = MqttTopicCollection.GetInstance();

            MqttClientSubscribeResult temp = new MqttClientSubscribeResult();
            List<TopicFilter> filters = new List<TopicFilter>();
            foreach (var topic in collection)
            {
                if (topic.HandlerInfo.HandleMethod.GetCustomAttribute<MqttSubscribeIgnoreAttribute>() == null)
                {
                    // 解析占位符
                    string topicVal = Utility.GetTopic(client.Options.ClientId, topic.Topic, dic);
                    TopicFilter filter = new TopicFilter()
                    {
                        Topic = topicVal,
                        QualityOfServiceLevel = topic.QoS
                    };
                    filters.Add(filter);
                }
            }
            if (filters.Count > 0)
            {
                temp = await client.SubscribeAsync(filters.ToArray());
            }
            return temp;
        }

        /// <summary>
        /// 取消通过TopicHandler订阅的主题
        /// </summary>
        /// <param name="client"></param>
        /// <remarks>使用指定占位符变量字典解析占位符</remarks>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns></returns>
        public static async Task<MqttClientUnsubscribeResult> UnSubscribeTopicsAsync(this IMqttClient client, ITopicPlaceholderDictionary dic)
        {
            // 分批次 TBD
            MqttTopicCollection collection = MqttTopicCollection.GetInstance();

            MqttClientUnsubscribeResult temp = new MqttClientUnsubscribeResult();
            List<string> topics = new List<string>();
            foreach (var topic in collection)
            {
                if (topic.HandlerInfo.HandleMethod.GetCustomAttribute<MqttSubscribeIgnoreAttribute>() == null)
                {
                    // 解析占位符
                    string topicVal = Utility.GetTopic(client.Options.ClientId, topic.Topic, dic);
                    topics.Add(topicVal);
                }
            }
            if (topics.Count > 0)
            {
                temp = await client.UnsubscribeAsync(topics.ToArray());
            }
            return temp;
        }
    }
}
