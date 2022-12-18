namespace MQTTnet.Client.Extensions.Demo
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // 普通使用示例
            SampleUse sampleUse = new SampleUse();
            await sampleUse.Run();

            // 使用过滤器示例
            //UseFilter useFilter = new UseFilter();
            //await useFilter.Run();
        }
    }
}