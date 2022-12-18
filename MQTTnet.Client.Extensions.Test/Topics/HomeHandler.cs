using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTTnet.Client.Extensions.Test.Topics
{
    /// <summary>
    /// 此测试用例假设使用通配符进行订阅
    /// </summary>
    [MqttTopic("home")]
    public class HomeHandler : TopicHandler
    {
        /// <summary>
        /// 注册 home/tempreture 处理回调，不订阅主题
        /// </summary>
        /// <returns></returns>
        [MqttSubscribeIgnore]
        public async Task Tempreture()
        {
            // meet some exception here
            throw new NotImplementedException("Test exception");
        }

        /// <summary>
        /// 只注册 home/bright 处理回调，不订阅主题
        /// </summary>
        /// <returns></returns>
        [MqttTopic("bright")]
        [MqttSubscribeIgnore]
        public async Task Brightness()
        {
            await Task.Delay(50);
            if (string.IsNullOrEmpty(ApplicationMessage.ResponseTopic))
            {
                ApplicationMessage.ResponseTopic = "-Execute-";
            }
            else
            {
                ApplicationMessage.ResponseTopic += "-Execute-";
            }
        }
    }
}
