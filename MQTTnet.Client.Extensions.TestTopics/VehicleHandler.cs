using System;
using System.Threading.Tasks;

namespace MQTTnet.Client.Extensions.TestTopics
{
    [MqttTopic("vehicle")]
    public class VehicleHandler : TopicHandler
    {
        [MqttTopic("{carId}/unlock")]
        public async Task Unlock()
        {
            // 远程解锁指定车架号的车子
            ApplicationMessage.ResponseTopic = "ok";
        }


    }
}
