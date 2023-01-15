using Microsoft.Extensions.DependencyInjection;
using MQTTnet.Client.Extensions.TestTopics;
using MQTTnet.Client.Receiving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTTnet.Client.Extensions.Test
{
    public class TopicPlaceholderTest
    {
        [Test]
        public async Task SubscribeTest()
        {
            ServiceCollection collection = new ServiceCollection();
            collection.UseMqttTopicHandler(options =>
            {
                var asb = typeof(VehicleHandler).Assembly;
                options.AddMqttTopicHandlers(asb);
            });
            var sp = collection.BuildServiceProvider();

            // 假设从配置中得到了车辆识别码
            string myCarId = "LF12099948372812DG";
            // 订阅之前设置占位符
            var dic = sp.GetRequiredService<ITopicPlaceholderDictionary>();
            dic.SetPlaceholder("carId", myCarId);

            TestMqttClient client = new TestMqttClient();
            var lst = await client.SubscribeTopicsAsync();
            string topic = $"vehicle/{myCarId}/unlock";
            int idx = lst.Items.FindIndex(s => s.TopicFilter.Topic == topic);
            Assert.IsTrue(idx >= 0);

            Clear(sp);
            sp.Dispose();
        }

        [Test]
        public async Task ExecuteTest()
        {
            ServiceCollection collection = new ServiceCollection();
            collection.UseMqttTopicHandler(options =>
            {
                var asb = typeof(VehicleHandler).Assembly;
                options.AddMqttTopicHandlers(asb);
            });
            var sp = collection.BuildServiceProvider();

            // 假设从配置中得到了车辆识别码
            string myCarId = "LF12099948372812DG";
            // 订阅之前设置占位符
            var dic = sp.GetRequiredService<ITopicPlaceholderDictionary>();
            dic.SetPlaceholder("carId", myCarId);

            var handler = sp.GetRequiredService<IMqttApplicationMessageReceivedHandler>();
            if (handler is MqttTopicSubscribeHandler)
            {
                string topic = $"vehicle/{myCarId}/unlock";

                MqttApplicationMessage msg = new MqttApplicationMessage();
                msg.Topic = topic;
                msg.Payload = UTF8Encoding.UTF8.GetBytes("HelloWorld");

                MqttApplicationMessageReceivedEventArgs args = new MqttApplicationMessageReceivedEventArgs("TestClientId", msg);
                await handler.HandleApplicationMessageReceivedAsync(args);
                Assert.That(args.ApplicationMessage.ResponseTopic, Is.EqualTo("ok"));
            }
            else
            {
                Assert.True(false);
            }
            Clear(sp);
            sp.Dispose();
        }

        [Test]
        public void DictionaryTest()
        {
            var dic = MqttTopicPlaceholderDictionary.GetInstance();
            dic.SetPlaceholder("clientA", "test", "A");
            dic.SetPlaceholder("clientB", "test", "B");

            string val = string.Empty;
            Assert.IsFalse(dic.TryGetValue("test", out val));

            dic.TryGetValue("clientA", "test", out val);
            Assert.That(val, Is.EqualTo("A"));

            dic.TryGetValue("clientB", "test", out val);
            Assert.That(val, Is.EqualTo("B"));

            dic.SetPlaceholder("somevalue", "global");
            dic.SetPlaceholder("clientA", "somevalue", "A_value");

            dic.TryGetValue("clientB", "somevalue", out val);
            Assert.That(val, Is.EqualTo("global"));

            dic.TryGetValue("clientA", "somevalue", out val);
            Assert.That(val, Is.EqualTo("A_value"));
        }

        private void Clear(IServiceProvider sp)
        {
            var topics = sp.GetRequiredService<MqttTopicCollection>();
            var filters = sp.GetRequiredService<TopicHandlerFilterCollection>();

            topics.Clear();
            filters.Clear();
        }
    }
}
