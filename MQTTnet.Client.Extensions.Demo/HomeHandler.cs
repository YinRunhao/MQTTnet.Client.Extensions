using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTTnet.Client.Extensions.Demo
{
    public class LightStatus
    {
        public string Id { get; set; }

        public bool SwitchOn { get; set; }
    }

    [MqttTopic("home")]
    public class HomeHandler : TopicHandler
    {
        [MqttTopic("light")]
        public async Task RecvLightStatus()
        {
            var status = GetLightStatus(ApplicationMessage.Payload);
            // save 
            // TBD
            Console.WriteLine($"收到[{ApplicationMessage.Topic}]订阅消息");
            await Task.Delay(100);
        }


        private LightStatus GetLightStatus(byte[] payload)
        {
            // convert bytes to object
            return new LightStatus();
        }
    }
}
