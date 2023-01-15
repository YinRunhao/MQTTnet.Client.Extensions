using System;
using System.Collections.Generic;
using System.Text;

namespace MQTTnet.Client.Extensions
{
    /// <summary>
    /// 主题占位符变量字典
    /// </summary>
    public interface ITopicPlaceholderDictionary
    {
        /// <summary>
        /// 全局设置主题占位符变量的值
        /// </summary>
        /// <param name="placeholder">占位符</param>
        /// <param name="value">值</param>
        void SetPlaceholder(string placeholder, string value);

        /// <summary>
        /// 多客户端情景下设置指定客户端主题占位符变量的值
        /// </summary>
        /// <param name="clientId">客户端ID</param>
        /// <param name="placeholder">占位符</param>
        /// <param name="value">值</param>
        void SetPlaceholder(string clientId, string placeholder, string value);

        /// <summary>
        /// 获取占位符变量值
        /// </summary>
        /// <param name="placeholder">占位符</param>
        /// <param name="value">值</param>
        /// <returns>是否存在该占位符</returns>
        bool TryGetValue(string placeholder, out string value);

        /// <summary>
        /// 多客户端情景下获取指定客户端主题占位符变量的值
        /// </summary>
        /// <param name="clientId">客户端ID</param>
        /// <param name="placeholder">占位符</param>
        /// <param name="value">值</param>
        /// <returns>是否存在该占位符</returns>
        bool TryGetValue(string clientId, string placeholder, out string value);
    }
}
