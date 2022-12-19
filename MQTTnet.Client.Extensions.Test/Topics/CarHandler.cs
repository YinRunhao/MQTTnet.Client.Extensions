using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MQTTnet.Protocol;

namespace MQTTnet.Client.Extensions.Test.Topics
{
    [MqttTopic("geely/car")]
    public class CarHandler:TopicHandler
    {
        [MqttTopic("speed")]
        public void SpeedSubscribe()
        {
        }

        [MqttTopic("fuel")]
        public async Task FuelSubscribe()
        {
            Console.WriteLine("call CarHandler FuelSubscribe method");
            await Task.Delay(50);
            ApplicationMessage.ResponseTopic = "testok";

        }

        [MqttTopic("seatbelts",MqttQualityOfServiceLevel.AtLeastOnce)]
        public void SeatbeltsSubscribe()
        {
        }
    }
}
