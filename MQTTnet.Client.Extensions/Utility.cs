using System;
using System.Collections.Generic;
using System.Text;

namespace MQTTnet.Client.Extensions
{
    internal static class Utility
    {
        public static string[] GetTopicPlaceholders(string topic)
        {
            List<int> begs = new List<int>();
            List<int> ends = new List<int>();

            for (int i = 0; i < topic.Length; i++)
            {
                if (topic[i] == '{')
                {
                    begs.Add(i);
                }
                else if (topic[i] == '}')
                {
                    ends.Add(i);
                }
            }

            if (begs.Count > 0)
            {
                string[] ret = new string[begs.Count];

                int beg, end;
                for (int i = 0; i < begs.Count; i++)
                {
                    beg = begs[i] + 1;
                    end = ends[i];
                    ret[i] = topic.Substring(beg, end - beg);
                }

                return ret;
            }
            else
            {
                return new string[0];
            }
        }

        public static bool CheckTopicFormat(string topic)
        {
            bool ret = true;
            int ck = 0;
            for (int i = 0; i < topic.Length; i++)
            {
                if (topic[i] == '{')
                {
                    ck++;
                }
                else if (topic[i] == '}')
                {
                    ck--;
                }
                if (ck > 1 || ck < 0)
                {
                    // > 1 说明有嵌套
                    // < 0 说明右括号在左括号前
                    ret = false;
                    break;
                }
            }

            if (ck != 0)
            {
                ret = false;
            }

            return ret;
        }

        /// <summary>
        /// 根据主题表达式和占位符字典获取实际主题字符串
        /// </summary>
        /// <param name="clientId">客户端ID</param>
        /// <param name="topicExp">主题表达式</param>
        /// <param name="dic">占位符字典</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static string GetTopic(string clientId, string topicExp, ITopicPlaceholderDictionary dic)
        {
            var placeholders = GetTopicPlaceholders(topicExp);
            if (placeholders.Length > 0)
            {
                string ret = topicExp;
                string val = string.Empty;
                foreach (var placeholder in placeholders)
                {
                    if (dic.TryGetValue(clientId, placeholder, out val))
                    {
                        ret = ret.Replace("{" + placeholder + "}", val);
                    }
                    else
                    {
                        throw new InvalidOperationException($"topic [{topicExp}] placeholder [{placeholder}] not found in MqttTopicPlaceholderDictionary");
                    }
                }

                return ret;
            }
            else
            {
                return topicExp;
            }
        }
    }
}
