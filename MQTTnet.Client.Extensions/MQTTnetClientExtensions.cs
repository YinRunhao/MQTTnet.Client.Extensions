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
        /// 使用指定的MQTT客户端订阅处理器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <param name="act"></param>
        /// <returns></returns>
        public static IServiceCollection UseMqttTopicHandler<T>(this IServiceCollection services, Action<MqttTopicHandlerOptions> act) where T : MqttTopicSubscribeHandler
        {
            services.AddSingleton<IMqttApplicationMessageReceivedHandler, T>();
            services.AddSingleton<MqttTopicCollection>(sp =>
            {
                return MqttTopicCollection.GetInstance();
            });
            services.AddSingleton<TopicHandlerFilterCollection>(sp =>
            {
                return TopicHandlerFilterCollection.GetInstance();
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
        /// <returns></returns>
        public static async Task<MqttClientSubscribeResult> SubscribeTopicsAsync(this IMqttClient client)
        {
            // 分批次 TBD
            MqttTopicCollection collection = MqttTopicCollection.GetInstance();

            MqttClientSubscribeResult temp = new MqttClientSubscribeResult();
            List<TopicFilter> filters = new List<TopicFilter>();
            foreach (var topic in collection)
            {
                if (topic.HandlerInfo.HandleMethod.GetCustomAttribute<MqttSubscribeIgnoreAttribute>() == null)
                {
                    TopicFilter filter = new TopicFilter()
                    {
                        Topic = topic.Topic,
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
        /// <returns></returns>
        public static async Task<MqttClientUnsubscribeResult> UnSubscribeTopicsAsync(this IMqttClient client)
        {
            // 分批次 TBD
            MqttTopicCollection collection = MqttTopicCollection.GetInstance();

            MqttClientUnsubscribeResult temp = new MqttClientUnsubscribeResult();
            List<string> topics = new List<string>();
            foreach (var topic in collection)
            {
                if (topic.HandlerInfo.HandleMethod.GetCustomAttribute<MqttSubscribeIgnoreAttribute>() == null)
                {
                    topics.Add(topic.Topic);
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
