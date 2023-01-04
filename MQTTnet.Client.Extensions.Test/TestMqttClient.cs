using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.ExtendedAuthenticationExchange;
using MQTTnet.Client.Options;
using MQTTnet.Client.Publishing;
using MQTTnet.Client.Receiving;
using MQTTnet.Client.Subscribing;
using MQTTnet.Client.Unsubscribing;
using MQTTnet.Formatter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTTnet.Client.Extensions.Test
{
    public class TestMqttClientOptions : IMqttClientOptions
    {
        public string ClientId { get; set; }

        public bool CleanSession => throw new NotImplementedException();

        public IMqttClientCredentials Credentials => throw new NotImplementedException();

        public IMqttExtendedAuthenticationExchangeHandler ExtendedAuthenticationExchangeHandler => throw new NotImplementedException();

        public MqttProtocolVersion ProtocolVersion => throw new NotImplementedException();

        public IMqttClientChannelOptions ChannelOptions => throw new NotImplementedException();

        public TimeSpan CommunicationTimeout => throw new NotImplementedException();

        public TimeSpan KeepAlivePeriod => throw new NotImplementedException();

        public TimeSpan? KeepAliveSendInterval => throw new NotImplementedException();

        public MqttApplicationMessage WillMessage => throw new NotImplementedException();

        public uint? WillDelayInterval => throw new NotImplementedException();

        public string AuthenticationMethod => throw new NotImplementedException();

        public byte[] AuthenticationData => throw new NotImplementedException();

        public uint? MaximumPacketSize => throw new NotImplementedException();

        public ushort? ReceiveMaximum => throw new NotImplementedException();

        public bool? RequestProblemInformation => throw new NotImplementedException();

        public bool? RequestResponseInformation => throw new NotImplementedException();

        public uint? SessionExpiryInterval => throw new NotImplementedException();

        public ushort? TopicAliasMaximum => throw new NotImplementedException();
    }

    public class TestMqttClient : IMqttClient
    {
        public bool IsConnected => throw new NotImplementedException();

        public IMqttClientOptions Options => new TestMqttClientOptions();

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
