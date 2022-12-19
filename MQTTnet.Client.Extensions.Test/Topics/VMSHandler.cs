using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTTnet.Client.Extensions.Test.Topics
{
    public class VMSHandler : TopicHandler
    {
        public async Task Temperature()
        {
        }

        public void Display()
        {
            Console.WriteLine("call VMSHandler Display method");
            ApplicationMessage.ResponseTopic = "testok";
        }

        public async Task Brightness()
        {
        }
    }
}
