using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MQTTnet.Client.Extensions.TestTopics;
using MQTTnet.Client.Options;
using MQTTnet.Client.Receiving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTTnet.Client.Extensions.Demo
{
    /// <summary>
    /// 使用主题占位符
    /// </summary>
    public class UseTopicPlaceholder
    {
        public async Task Run()
        {
            string ip = "127.0.0.1";
            int port = 1883;
            string clientId = "TestClient";

            // 需要读配置，这里引入配置文件
            var cfgBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true);

            IConfiguration config = cfgBuilder.Build();

            // 使用依赖注入
            ServiceCollection collection = new ServiceCollection();
            collection.AddSingleton<IConfiguration>(config);
            // 1. 配置扩展处理器
            collection.UseMqttTopicHandler(option => {
                // 2. 添加指定程序集的所有TopicHandler
                option.AddMqttTopicHandlers(typeof(VehicleHandler).Assembly);
            });
            // 构建依赖注入容器
            var service = collection.BuildServiceProvider();

            // 读配置拿标识符
            var cfg = service.GetRequiredService<IConfiguration>();
            string carId = cfg.GetSection("CarId").Value;

            var dic = service.GetRequiredService<ITopicPlaceholderDictionary>();
            // 3. 使用前先设置占位符的值
            dic.SetPlaceholder("carId", carId);

            // 配置客户端
            MqttClientOptionsBuilder option = new MqttClientOptionsBuilder();
            option.WithTcpServer(ip, port)
                .WithProtocolVersion(Formatter.MqttProtocolVersion.V311)
                .WithClientId(clientId);

            MqttFactory mqttFactory = new MqttFactory();
            var client = mqttFactory.CreateMqttClient();
            // 4. 配置扩展后即可取得处理器，然后设置处理器
            var handler = service.GetRequiredService<IMqttApplicationMessageReceivedHandler>();
            client.ApplicationMessageReceivedHandler = handler;

            await client.ConnectAsync(option.Build());

            // 5. 订阅处理器对应的主题
            await client.SubscribeTopicsAsync();

            Console.WriteLine("输入回车退出");
            Console.ReadLine();

            await client.DisconnectAsync();
            service.Dispose();
        }
    }
}
