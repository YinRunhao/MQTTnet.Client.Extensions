# MQTTnet.Client.Extensions

一个基于 **MQTTnet** 的 **客户端** 扩展，使你的主题订阅消息回调像 ASP.net 的Controller一样编码使用。

## 先决条件

使用该扩展之前需要一些先决条件

1. [必要] 程序中的对象管理需使用**依赖注入**

2. [必要] MQTTnet版本需在 3.0.3 以上

3. [必要] 目前能支持的目标框架仅有 .net standard 2.0，使用前可用以下地址确认你的项目框架能否使用 .net standard 2.0的程序集

   [.NET Standard | Microsoft Learn](https://learn.microsoft.com/zh-cn/dotnet/standard/net-standard?tabs=net-standard-2-0)

4. [可选] 在某些异常情况会产生一些日志，日志器使用的是依赖注入容器获取的 ILogger< MqttTopicSubscribeHandler > 若获取不到，则不会产生日志。

## 使用示例

### 简单使用

1. 定义订阅处理类

   ```c#
   [MqttTopic("home")]
   public class HomeHandler : TopicHandler
   {
       [MqttTopic("light")]
       public async Task RecvLightStatus()
       {
           // home/light
           Console.WriteLine($"收到[{ApplicationMessage.Topic}]订阅消息");
           await Task.Delay(100);
       }
   }
   ```

   

2. 配置依赖注入容器并配置客户端的应用消息处理器

   ```c#
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
   
   // MQTTnet配置客户端
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
   ```

3. 按订阅处理器自动对应的主题(也可以自行手动订阅)

   ```c#
   // 4. 订阅处理器对应的主题
   await client.SubscribeTopicsAsync();
   ```

### 添加过滤器

1. 实现过滤器接口(可在构造方法中使用依赖注入获取需要的对象)

   ```c#
   public class LoggingFilter : ITopicHandlerFilter
   {
       public void OnHandlerExecuted(TopicHandlerContext context)
       {
           if (context.LastException != null)
           {
               // 遇到异常，记录
           }
       }
   
       public void OnHandlerExecuting(TopicHandlerContext context)
       {
           // 记录收到的消息
       }
   }
   ```

   

2. 按顺序添加过滤器

   ```c#
   string ip = "127.0.0.1";
   int port = 1883;
   string clientId = "TestClient";
   
   // 使用依赖注入
   ServiceCollection collection = new ServiceCollection();
   // 1. 配置扩展处理器
   collection.UseMqttTopicHandler(option => {
       // 2. 添加当前程序集的所有TopicHandler
       option.AddMqttTopicHandlers(this.GetType().Assembly)
           // 2.1 按顺序添加过滤器
           .AddAsyncHandlerFilter<AuthFilter>()    // 异步执行的过滤器
           .AddHandlerFilter<LoggingFilter>();     // 同步执行的过滤器
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
   ```


## 一些概念

### TopicHandler

类似于 ASP.net 中的Controller，任何继承于它的类中的公共实例方法将被识别为是一个订阅的回调。

#### 可用属性

| 属性名             | 类型                   | 说明                       |
| ------------------ | ---------------------- | -------------------------- |
| ClientId           | string                 | 自己的ID                   |
| ApplicationMessage | MqttApplicationMessage | 收到的MQTT应用消息         |
| Context            | TopicHandlerContext    | 订阅处理上下文对象(见下文) |

#### 回调方法识别规则

初始化时会扫描类中所有自行定义的公共实例方法，并且这些实例方法不能带有参数与返回值。因为MQTT并不是一个 请求-回送 协议，所以这里认为订阅的处理不应该产生返回值；另外，所有的参数已经在 ApplicationMessage 属性中了，所以目前就不做参数解析工作了，故要求方法不能带有参数。

*注：每次接收到订阅消息都会new一个对应的TopicHandler并调用其方法，在方法中使用ApplicationMessage或Context无需考虑并行问题*

**按类名+方法名识别**

继承于TopicHandler后无需配置任何东西，初始化时将自动扫描该类中定义的公共实例方法作为订阅回调。类名将自动去除末尾的'Handler'字符，并拼接方法名。所有字符均转为小写。

```c#
public class VMSHandler : TopicHandler
{
    public async Task Temperature()
    {
    }

    public void Brightness()
    {
    }
}
```

在该类中的两个方法将被识别为以下两个订阅的回调

> vms/temperature
>
> vms/brightness

**使用MqttTopic特性指定**

对继承于TopicHandler的类或其公共实例方法可添加 **MqttTopic** 特性以指定主题名或服务质量。

```c#
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
        }

        [MqttTopic("seatbelts", MqttQualityOfServiceLevel.AtLeastOnce)]
        public void SeatbeltsSubscribe()
        {
        }
    }
```

在该类中的三个方法将被识别为以下三个订阅的回调

> geely/car/speed
>
> geely/car/fuel
>
> geely/car/seatbelts

### MqttTopic

该特性可用于类或方法，用于指定回调方法是属于哪个主题，同时，如果使用自动订阅功能，将可以用该特性指定订阅的主题和服务质量。

### 过滤器

订阅回调的执行除了会执行对应TopicHandler的方法外还可以通过配置过滤器来增加一些额外的操作，例如记录日志，判断权限等。过滤器中包含 *OnHandlerExecuting* 和 *OnHandlerExecuted* 两个方法，若按顺序配置了A, B两个过滤器，则执行顺序如下。

A(OnHandlerExecuting) -> B(OnHandlerExecuting) -> [对应Handler的方法] -> B(OnHandlerExecuted) -> A(OnHandlerExecuted)

过滤器接口有两个：同步过滤器(ITopicHandlerFilter)，异步过滤器(ITopicHandlerAsyncFilter)。两者使用方法一致，具体使用哪个取决于你的操作是否是异步方法。

### TopicHandlerContext

各过滤器和TopicHandler将传递同一个TopicHandlerContext对象。

#### 可用属性

| 属性名             | 类型                   | 说明                                                         |
| ------------------ | ---------------------- | ------------------------------------------------------------ |
| ClientId           | string                 | 自己的ID                                                     |
| ApplicationMessage | MqttApplicationMessage | 收到的MQTT应用消息                                           |
| LastException      | Exception              | 过滤器或执行中遇到的最后一个异常，在过滤器中手动赋值将传递给下层过滤器 |
| IsBreak            | bool                   | 是否中止执行其余过滤器，赋值为 true 后将终止后续执行         |

## 通配符订阅

### 跳过自动订阅

若客户端使用了通配符订阅，那么在使用自动订阅功能时可通过向方法添加 *MqttSubscribeIgnore* 特性使自动订阅时跳过订阅该主题，但收到对应主题消息时依然能调用其处理方法。

例如需要使用 *home/#* 进行了订阅，那么再订阅 *home/tempreature* 或 *home/bright* 将没有意义，故可以加上 *MqttSubscribeIgnore* 特性跳过这两个订阅。

### 包含通配符的订阅处理

Q: 订阅处理的方法中使用 *MqttTopic* 特性能否包含通配符

A: 目前不支持，因为MQTT协议中规定发布的消息必须包含明确的主题，所以订阅者接收发布消息时，发布消息的主题不会包含通配符。后续版本可能会支持。

Q: 通过通配符订阅接收到了未设置 *MqttTopic* 特性的主题会怎样？要如何处理？

A: 会产生无法处理的错误日志。这里有一个设计思想就是**我只做我能做的事情**，若只定义了 *home/tempreature* 的处理方法，但使用了 *home/#* 进行订阅，导致收到了 *home/bright* 的消息，这里将认为是尚未定义这类消息的处理方案，即**我不能处理这个消息**。虽然不能映射到对应的处理方法，但过滤器对这种消息仍然有效。

## 主题占位符变量

主题占位符变量是用于在订阅主题中包含某些特定标识符的解决方案。例如现有一个车机应用，车机的唯一识别码是"testcar001"，它需要订阅主题**vehicle/testcar001/unlock**实现远程解锁功能，那么此时"testcar001"就是主题占位符变量。此时我们将"testcar001"理解为一个变量，变量名为**carId**，变量值是"testcar001"，当然，变量值应该是可以是任何值，可以从配置文件、数据库等地方读取。

### 使用主题占位符变量

在上述例子中，我们可以理解为需要订阅主题**vehicle/{carId}/unlock**以实现远程解锁功能，其中{carId}是一个占位符。

1. 在**MqttTopic**特性中声明主题占位符

   在**MqttTopic**特性中将需要抽象为变量的地方使用“{}”包裹起来，"{}"中的就是占位符变量

   ```c#
   [MqttTopic("vehicle")]
   public class VehicleHandler : TopicHandler
   {
       [MqttTopic("{carId}/unlock")]
       public async Task Unlock()
       {
           // 远程解锁指定车架号的车子
       }
   }
   ```

2. 设置主题占位符变量的值

   在依赖注入容器构建完成后获取**ITopicPlaceholderDictionary**，调用**SetPlaceholder**方法设置占位符变量的值。

   若有使用自动订阅TopicHandler对应的主题，请在调用**SubscribeTopicsAsync**之前就把占位符变量设置好。

   ```c#
   ...
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
   string carId = cfg.GetSection("MyCarId").Value;
   // 从容器中获取占位符字典
   var dic = service.GetRequiredService<ITopicPlaceholderDictionary>();
   // 3. 设置占位符的值
   dic.SetPlaceholder("carId", carId);
   ...
   ```

   *注意：这里的占位符是大小写敏感的。*

   *完整示例可参考Demo中的UseTopicPlaceholder.cs*

