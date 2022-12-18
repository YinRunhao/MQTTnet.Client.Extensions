using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.ExtendedAuthenticationExchange;
using MQTTnet.Client.Options;
using MQTTnet.Client.Publishing;
using MQTTnet.Client.Receiving;
using MQTTnet.Client.Subscribing;
using MQTTnet.Client.Unsubscribing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTTnet.Client.Extensions.Test
{
    public class TestMqttClient : IMqttClient
    {
        public bool IsConnected => throw new NotImplementedException();

        public IMqttClientOptions Options => throw new NotImplementedException();

        public IMqttClientConnectedHandler ConnectedHandler { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IMqttClientDisconnectedHandler DisconnectedHandler { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IMqttApplicationMessageReceivedHandler ApplicationMessageReceivedHandler { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Task<MqttClientAuthenticateResult> ConnectAsync(IMqttClientOptions options, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task DisconnectAsync(MqttClientDisconnectOptions options, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task<MqttClientPublishResult> PublishAsync(MqttApplicationMessage applicationMessage, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SendExtendedAuthenticationExchangeDataAsync(MqttExtendedAuthenticationExchangeData data, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<MqttClientSubscribeResult> SubscribeAsync(MqttClientSubscribeOptions options, CancellationToken cancellationToken)
        {
            MqttClientSubscribeResult ret = new MqttClientSubscribeResult();
            foreach (var item in options.TopicFilters)
            {
                MqttClientSubscribeResultItem temp = new MqttClientSubscribeResultItem(item, MqttClientSubscribeResultCode.GrantedQoS0);
                ret.Items.Add(temp);
            }
            return Task.FromResult(ret);
        }

        public Task<MqttClientUnsubscribeResult> UnsubscribeAsync(MqttClientUnsubscribeOptions options, CancellationToken cancellationToken)
        {
            MqttClientUnsubscribeResult ret = new MqttClientUnsubscribeResult();
            foreach (var item in options.TopicFilters)
            {
                MqttClientUnsubscribeResultItem temp = new MqttClientUnsubscribeResultItem(item, MqttClientUnsubscribeResultCode.Success);
                ret.Items.Add(temp);
            }
            return Task.FromResult(ret);
        }
    }
}
