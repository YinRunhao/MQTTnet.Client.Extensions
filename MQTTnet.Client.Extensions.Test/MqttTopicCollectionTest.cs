using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTTnet.Client.Extensions.Test
{
    public class MqttTopicCollectionTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void AddTest()
        {
            MqttTopicCollection topicCollection = MqttTopicCollection.GetInstance();
            topicCollection.AddTopics(this.GetType().Assembly);

            var lst = topicCollection.ToList();

            Assert.That(lst.Count, Is.EqualTo(Consts.TotalTopics));
            Assert.True(lst.FindIndex(s => s.Topic == "vms/display") >= 0);
            Assert.True(lst.FindIndex(s => s.Topic == "yrh/car/seatbelts") >= 0);
            topicCollection.Clear();
        }

        [Test]
        public void RepeatAddTest()
        {
            MqttTopicCollection topicCollection = MqttTopicCollection.GetInstance();
            topicCollection.AddTopics(this.GetType().Assembly);
            topicCollection.AddTopics(this.GetType().Assembly);

            var lst = topicCollection.ToList();

            Assert.That(lst.Count, Is.EqualTo(Consts.TotalTopics));
            topicCollection.Clear();
        }
    }
}
