using Microsoft.Extensions.DependencyInjection;
using MQTTnet.Client.Extensions.Test.Topics;
using MQTTnet.Client.Receiving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTTnet.Client.Extensions.Test
{
    public class ExtensionsTest
    {
        [Test]
        public async Task AutoSubscribeTest()
        {
            ServiceCollection collection = new ServiceCollection();
            collection.UseMqttTopicHandler(options =>
            {
                var asb = this.GetType().Assembly;
                options.AddMqttTopicHandlers(asb);
            });
            var sp = collection.BuildServiceProvider();

            TestMqttClient client = new TestMqttClient();
            var lst = await client.SubscribeTopicsAsync();
            int cnt = lst.Items.Count;
            Assert.That(cnt, Is.EqualTo(Consts.TotalSubscribeTopics));

            var test = sp.GetRequiredService<MqttTopicCollection>();
            int handlerCnt = test.Count();

            Assert.That(handlerCnt, Is.EqualTo(Consts.TotalTopics));
            Clear(sp);
            sp.Dispose();
        }

        [Test]
        public async Task AutoUnSubscribeTest()
        {
            ServiceCollection collection = new ServiceCollection();
            collection.UseMqttTopicHandler(options =>
            {
                var asb = this.GetType().Assembly;
                options.AddMqttTopicHandlers(asb);
            });
            var sp = collection.BuildServiceProvider();

            TestMqttClient client = new TestMqttClient();
            var lst = await client.SubscribeTopicsAsync();
            int cnt = lst.Items.Count;
            Assert.That(cnt, Is.EqualTo(Consts.TotalSubscribeTopics));

            var ulst = await client.UnSubscribeTopicsAsync();
            cnt = ulst.Items.Count;
            Assert.That(cnt, Is.EqualTo(Consts.TotalSubscribeTopics));
            Clear(sp);
            sp.Dispose();
        }

        [Test]
        public void UseMqttTopicHandlerTest()
        {
            ServiceCollection collection = new ServiceCollection();
            collection.UseMqttTopicHandler(options =>
            {
                var asb = this.GetType().Assembly;
                options.AddMqttTopicHandlers(asb);
            });
            var sp = collection.BuildServiceProvider();

            var topics = sp.GetRequiredService<MqttTopicCollection>();
            sp.GetRequiredService<CarHandler>();
            sp.GetRequiredService<VMSHandler>();
            var handler = sp.GetRequiredService<IMqttApplicationMessageReceivedHandler>();
            Assert.That(handler.GetType().Name, Is.EqualTo(typeof(MqttTopicSubscribeHandler).Name));
            var cnt = topics.Count();
            Assert.That(cnt, Is.EqualTo(Consts.TotalTopics));

            Clear(sp);
            sp.Dispose();
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
