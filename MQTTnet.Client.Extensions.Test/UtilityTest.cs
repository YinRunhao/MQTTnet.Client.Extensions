using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTTnet.Client.Extensions.Test
{
    public class UtilityTest
    {
        //[Test]
        //public void GetPlaceholderTest()
        //{
        //    string topic = "home/{roomId}/{deviceId}";
        //    var outcome = Utility.GetTopicPlaceholders(topic);
        //    Assert.IsTrue(outcome.Length == 2);
        //    Assert.That(outcome[0], Is.EqualTo("roomId"));
        //    Assert.That(outcome[1], Is.EqualTo("deviceId"));
        //    topic = "home/{test}";
        //    outcome = Utility.GetTopicPlaceholders(topic);
        //    Assert.IsTrue(outcome.Length == 1);
        //    Assert.That(outcome[0], Is.EqualTo("test"));
        //}

        //[Test]
        //public void CheckPlaceholderFormatTest()
        //{
        //    string topic = "a/{device}/{sensor}";
        //    bool ck = Utility.CheckTopicFormat(topic);
        //    Assert.IsTrue(ck);

        //    topic = "a/{test{haha}}";
        //    ck = Utility.CheckTopicFormat(topic);
        //    Assert.IsFalse(ck);

        //    topic = "{haha}{";
        //    ck = Utility.CheckTopicFormat(topic);
        //    Assert.IsFalse(ck);

        //    topic = "{haha}}{";
        //    ck = Utility.CheckTopicFormat(topic);
        //    Assert.IsFalse(ck);

        //    topic = "{abc}/{abc";
        //    ck = Utility.CheckTopicFormat(topic);
        //    Assert.IsFalse(ck);

        //    topic = "{abc}/{abc}}";
        //    ck = Utility.CheckTopicFormat(topic);
        //    Assert.IsFalse(ck);
        //}
    }
}
