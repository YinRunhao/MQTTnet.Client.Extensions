using Microsoft.Extensions.DependencyInjection;
using MQTTnet.Client.Options;
using MQTTnet.Client.Receiving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTTnet.Client.Extensions.Demo
{
    public class SampleUse
    {
        public async Task Run()
        {
            string ip = "127.0.0.1";
            int port = 1883;
            string clientId = "TestClient";

            // 使用依赖注入
            ServiceCollection collection = new ServiceCollection();
            // 1. 配置扩展处理器
            collection.UseMqttTopicHandler(option => {
                // 2. 添加当前程序集的所有TopicHandler
                option.AddMqttTopicHandlers(this.GetType().Assembly);
            });
            // 构建依赖注入容器
            var service = collection.BuildServiceProvider();

            // 配置客户端
            MqttClientOptionsBuilder option = new MqttClientOptionsBuilder();
            option.WithTcpServer(ip, port)
                .WithProtocolVersion(Formatter.MqttProtocolVersion.V311)
                .WithClientId(clientId);

            MqttFactory mqttFactory = new MqttFactory();
            var client = mqttFactory.CreateMqttClient();
            // 3. 配置扩展后即可取得处理器，然后设置处理器
            var handler = service.GetRequiredService<IMqttApplicationMessageReceivedHandler>();
            client.ApplicationMessageReceivedHandler = handler;

            await client.ConnectAsync(option.Build());

            // 4. 订阅处理器对应的主题
            await client.SubscribeTopicsAsync();

            Console.WriteLine("输入回车退出");
            Console.ReadLine();

            await client.DisconnectAsync();
            service.Dispose();
        }
    }
}
