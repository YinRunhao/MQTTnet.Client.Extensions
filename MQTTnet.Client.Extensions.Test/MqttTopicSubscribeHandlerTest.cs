using Microsoft.Extensions.DependencyInjection;
using MQTTnet.Client.Extensions.Test.Filters;
using MQTTnet.Client.Receiving;
using System.Text;

namespace MQTTnet.Client.Extensions.Test
{
    public class MqttTopicSubscribeHandlerTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task TestException()
        {
            ServiceCollection collection = new ServiceCollection();
            collection.UseMqttTopicHandler(options =>
            {
                var asb = this.GetType().Assembly;
                options.AddMqttTopicHandlers(asb)
                    .AddAsyncHandlerFilter<ExcetionLoggingFilter>();
            });
            var sp = collection.BuildServiceProvider();

            var handler = sp.GetRequiredService<IMqttApplicationMessageReceivedHandler>();
            if (handler is MqttTopicSubscribeHandler)
            {
                MqttApplicationMessage msg = new MqttApplicationMessage();
                msg.Topic = "home/tempreture";
                msg.Payload = UTF8Encoding.UTF8.GetBytes("HelloWorld");

                MqttApplicationMessageReceivedEventArgs args = new MqttApplicationMessageReceivedEventArgs("TestClientId", msg);
                await handler.HandleApplicationMessageReceivedAsync(args);
                Assert.That(args.ApplicationMessage.ResponseTopic, Is.EqualTo("get last exception"));
            }
            else
            {
                Assert.True(false);
            }
            Clear(sp);
            sp.Dispose();
        }

        [Test]
        public async Task TestAsyncExecute()
        {
            ServiceCollection collection = new ServiceCollection();
            collection.UseMqttTopicHandler(options =>
            {
                var asb = this.GetType().Assembly;
                options.AddMqttTopicHandlers(asb);
            });
            var sp = collection.BuildServiceProvider();

            var handler = sp.GetRequiredService<IMqttApplicationMessageReceivedHandler>();
            if (handler is MqttTopicSubscribeHandler)
            {
                MqttApplicationMessage msg = new MqttApplicationMessage();
                msg.Topic = "yrh/car/fuel";
                msg.Payload = UTF8Encoding.UTF8.GetBytes("HelloWorld");

                MqttApplicationMessageReceivedEventArgs args = new MqttApplicationMessageReceivedEventArgs("TestClientId", msg);
                await handler.HandleApplicationMessageReceivedAsync(args);
                Assert.That(args.ApplicationMessage.ResponseTopic, Is.EqualTo("testok"));
            }
            else
            {
                Assert.True(false);
            }
            Clear(sp);
            sp.Dispose();
        }

        [Test]
        public async Task TestFilterBreak()
        {
            ServiceCollection collection = new ServiceCollection();
            collection.UseMqttTopicHandler(options =>
            {
                var asb = this.GetType().Assembly;
                options.AddMqttTopicHandlers(asb)
                    .AddHandlerFilter<TestFilter>()
                    .AddHandlerFilter<BreakFilter>();
            });
            var sp = collection.BuildServiceProvider();

            var handler = sp.GetRequiredService<IMqttApplicationMessageReceivedHandler>();
            if (handler is MqttTopicSubscribeHandler)
            {
                MqttApplicationMessage msg = new MqttApplicationMessage();
                msg.Topic = "home/bright";
                msg.Payload = UTF8Encoding.UTF8.GetBytes("HelloWorld");

                MqttApplicationMessageReceivedEventArgs args = new MqttApplicationMessageReceivedEventArgs("TestClientId", msg);
                await handler.HandleApplicationMessageReceivedAsync(args);
                Assert.That(args.ApplicationMessage.ResponseTopic, Is.EqualTo("-A--Break-"));
            }
            else
            {
                Assert.True(false);
            }
            Clear(sp);
            sp.Dispose();
        }

        [Test]
        public async Task TestFilter()
        {
            ServiceCollection collection = new ServiceCollection();
            collection.UseMqttTopicHandler(options =>
            {
                var asb = this.GetType().Assembly;
                options.AddMqttTopicHandlers(asb)
                    .AddHandlerFilter<TestFilter>()
                    .AddAsyncHandlerFilter<TestAsyncFilter>();
            });
            var sp = collection.BuildServiceProvider();

            var handler = sp.GetRequiredService<IMqttApplicationMessageReceivedHandler>();
            if (handler is MqttTopicSubscribeHandler)
            {
                MqttApplicationMessage msg = new MqttApplicationMessage();
                msg.Topic = "home/bright";
                msg.Payload = UTF8Encoding.UTF8.GetBytes("HelloWorld");

                MqttApplicationMessageReceivedEventArgs args = new MqttApplicationMessageReceivedEventArgs("TestClientId", msg);
                await handler.HandleApplicationMessageReceivedAsync(args);
                Assert.That(args.ApplicationMessage.ResponseTopic, Is.EqualTo("-A--B--Execute--B--A-"));
            }
            else
            {
                Assert.True(false);
            }
            Clear(sp);
            sp.Dispose();
        }

        [Test]
        public async Task TestExecute()
        {
            ServiceCollection collection = new ServiceCollection();
            collection.UseMqttTopicHandler(options =>
            {
                var asb = this.GetType().Assembly;
                options.AddMqttTopicHandlers(asb);
            });
            var sp = collection.BuildServiceProvider();

            var handler = sp.GetRequiredService<IMqttApplicationMessageReceivedHandler>();
            if (handler is MqttTopicSubscribeHandler)
            {
                MqttApplicationMessage msg = new MqttApplicationMessage();
                msg.Topic = "vms/display";
                msg.Payload = UTF8Encoding.UTF8.GetBytes("HelloWorld");

                MqttApplicationMessageReceivedEventArgs args = new MqttApplicationMessageReceivedEventArgs("TestClientId", msg);
                await handler.HandleApplicationMessageReceivedAsync(args);
                Assert.That(args.ApplicationMessage.ResponseTopic, Is.EqualTo("testok"));
            }
            else
            {
                Assert.True(false);
            }
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