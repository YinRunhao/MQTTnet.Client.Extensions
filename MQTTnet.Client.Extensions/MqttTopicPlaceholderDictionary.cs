using System;
using System.Collections.Generic;
using System.Text;

namespace MQTTnet.Client.Extensions
{
    /// <summary>
    /// 默认占位符变量字典
    /// </summary>
    public class MqttTopicPlaceholderDictionary : ITopicPlaceholderDictionary
    {
        private Dictionary<string, string> m_Placeholders = new Dictionary<string, string>();

        private static MqttTopicPlaceholderDictionary s_Instance = new MqttTopicPlaceholderDictionary();

        private MqttTopicPlaceholderDictionary() { }

        public static MqttTopicPlaceholderDictionary GetInstance()
        {
            return s_Instance;
        }

        /// <summary>
        /// 设置占位符变量的值
        /// </summary>
        /// <param name="placeholder">占位符</param>
        /// <param name="value">值</param>
        /// <remarks>占位符不能包含'{'或'}'</remarks>
        /// <exception cref="InvalidOperationException"></exception>
        public void SetPlaceholder(string placeholder, string value)
        {
            if (placeholder.IndexOf('{') >= 0 || placeholder.IndexOf('}') >= 0)
            {
                throw new InvalidOperationException("placeholder can not contains '{' or '}'");
            }
            if (m_Placeholders.ContainsKey(placeholder))
            {
                m_Placeholders[placeholder] = value;
            }
            else
            {
                m_Placeholders.Add(placeholder, value);
            }
        }

        /// <summary>
        /// 获取占位符变量值
        /// </summary>
        /// <param name="placeholder"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(string placeholder, out string value)
        {
            string val = string.Empty;
            bool ok = m_Placeholders.TryGetValue(placeholder, out val);
            value = val;
            return ok;
        }

        public void SetPlaceholder(string clientId, string placeholder, string value)
        {
            if (string.IsNullOrEmpty(clientId))
            {
                clientId = string.Empty;
            }
            string key = $"{clientId}:{placeholder}";
            SetPlaceholder(key, value);
        }

        public bool TryGetValue(string clientId, string placeholder, out string value)
        {
            if (string.IsNullOrEmpty(clientId))
            {
                clientId = string.Empty;
            }
            string key = $"{clientId}:{placeholder}";
            if (TryGetValue(key, out string val))
            {
                // 先取指定客户端
                value = val;
                return true;
            }
            else
            {
                // 再取全局
                return TryGetValue(placeholder, out value);
            }
        }
    }
}
