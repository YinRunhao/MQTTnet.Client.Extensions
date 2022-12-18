using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MQTTnet.Client.Receiving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MQTTnet.Client.Extensions
{
    /// <summary>
    /// 主题订阅处理器
    /// </summary>
    public class MqttTopicSubscribeHandler : IMqttApplicationMessageReceivedHandler
    {
        /// <summary>
        /// 主题处理集合
        /// </summary>
        protected MqttTopicCollection m_Topics;

        /// <summary>
        /// 依赖注入
        /// </summary>
        protected IServiceProvider m_MainProvider;

        /// <summary>
        /// 日志记录
        /// </summary>
        protected ILogger<MqttTopicSubscribeHandler> m_Logger;

        /// <summary>
        /// 过滤器集合
        /// </summary>
        protected List<Type> m_FilterTypes;

        public MqttTopicSubscribeHandler(IServiceProvider sp)
        {
            m_Topics = sp.GetRequiredService<MqttTopicCollection>();
            m_MainProvider = sp;
            m_Logger = m_MainProvider.GetService<ILogger<MqttTopicSubscribeHandler>>();
            var temp = sp.GetRequiredService<TopicHandlerFilterCollection>();
            m_FilterTypes = temp.ToList();
        }

        /// <summary>
        /// 接收回调，执行各过滤器与订阅处理
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        public virtual async Task HandleApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs eventArgs)
        {
            if (eventArgs.ProcessingFailed)
            {
                return;
            }
            // create context
            TopicHandlerContext context = new TopicHandlerContext();
            context.ApplicationMessage = eventArgs.ApplicationMessage;
            context.ClientId = eventArgs.ClientId;
            // create scope
            var scope = m_MainProvider.CreateScope();
            var service = scope.ServiceProvider;

            List<object> filters = new List<object>();
            // get filters
            foreach (var tp in m_FilterTypes)
            {
                var obj = service.GetService(tp);
                if (obj != null)
                {
                    filters.Add(obj);
                }
            }
            // execute filter (A->B->C)
            foreach (var obj in filters)
            {
                try
                {
                    if (obj is ITopicHandlerAsyncFilter asyncFilter)
                    {
                        await asyncFilter.OnHandlerExecutingAsync(context);
                    }
                    else if (obj is ITopicHandlerFilter filter)
                    {
                        filter.OnHandlerExecuting(context);
                    }
                }
                catch (Exception ex)
                {
                    context.LastException = ex;
                }

                if (context.IsBreak)
                {
                    goto DONE;
                }
            }

            await ActionExecuteAsync(service, eventArgs, context);
            if (context.IsBreak)
            {
                goto DONE;
            }

            // execute filter (C->B->A)
            for (int i = filters.Count - 1; i >= 0; i--)
            {
                var obj = filters[i];
                try
                {
                    if (obj is ITopicHandlerAsyncFilter asyncFilter)
                    {
                        await asyncFilter.OnHandlerExecutedAsync(context);
                    }
                    else if (obj is ITopicHandlerFilter filter)
                    {
                        filter.OnHandlerExecuted(context);
                    }
                }
                catch (Exception ex)
                {
                    context.LastException = ex;
                }

                if (context.IsBreak)
                {
                    goto DONE;
                }
            }
        DONE:
            scope.Dispose();
        }

        /// <summary>
        /// 执行订阅回调
        /// </summary>
        /// <param name="service"></param>
        /// <param name="eventArgs"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual async Task ActionExecuteAsync(IServiceProvider service, MqttApplicationMessageReceivedEventArgs eventArgs, TopicHandlerContext context)
        {
            var topic = GetMqttTopicItem(eventArgs.ApplicationMessage.Topic, eventArgs.ApplicationMessage.TopicAlias);
            if (topic != null)
            {
                try
                {
                    TopicHandler handler = service.GetRequiredService(topic.HandlerInfo.HandlerType) as TopicHandler;
                    handler.ApplicationMessage = context.ApplicationMessage;
                    handler.ClietntId = context.ClientId;
                    handler.Context = context;

                    await CallMethodAsync(handler, topic.HandlerInfo);
                }
                catch (Exception ex)
                {
                    // log
                    m_Logger?.LogError(ex, $"Topic handler [{topic.HandlerInfo.HandlerType.Name}] execute method [{topic.HandlerInfo.HandleMethod.Name}] meet an exception");
                    context.LastException = ex;
                }
            }
            else
            {
                eventArgs.ProcessingFailed = true;
                // 没有找到对应主题
                string str = eventArgs.ApplicationMessage.TopicAlias.HasValue ? eventArgs.ApplicationMessage.TopicAlias.Value.ToString() : "none";
                m_Logger?.LogError($"Topic handler not found. Topic name [{eventArgs.ApplicationMessage.Topic}] alias [{str}]");
            }
        }

        /// <summary>
        /// 根据主题名查找处理器
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="topicAlias"></param>
        /// <returns></returns>
        protected virtual MqttTopicItem GetMqttTopicItem(string topic, ushort? topicAlias)
        {
            MqttTopicItem ret = default;
            if (!string.IsNullOrEmpty(topic))
            {
                foreach (var item in m_Topics)
                {
                    if (item.Topic == topic)
                    {
                        // 更新主题别名
                        if (topicAlias.HasValue && topicAlias > 0)
                        {
                            item.SetTopicAlias(topicAlias.Value);
                        }
                        ret = item;
                        break;
                    }
                }
            }
            else
            {
                if (topicAlias.HasValue)
                {
                    foreach (var item in m_Topics)
                    {
                        if (item.TopicAlias.HasValue && item.TopicAlias.Value == topicAlias.Value)
                        {
                            ret = item;
                            break;
                        }
                    }
                }
            }

            return ret;
        }

        private async Task CallMethodAsync(object handler, MqttTopicHandlerInfo info)
        {
            object[] param = new object[0];

            if (info.IsAwaitable)
            {
                await (Task)info.HandleMethod.Invoke(handler, param);
            }
            else
            {
                info.HandleMethod.Invoke(handler, param);
            }
        }
    }
}
