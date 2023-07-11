using MQTTnet;
using MQTTnet.Client;

namespace MqttService.MqttNet;

public class MqttNetService : IDisposable
{   
    private readonly IMqttClient _mqttClient;
    private readonly MqttFactory _mqttFactory;

    public MqttNetService()
    {
        _mqttFactory = new MqttFactory();
        _mqttClient = _mqttFactory.CreateMqttClient();
    }

    public async Task<MqttClientConnectResult> ConnectAsync(Config config)
    {
        var options = new MqttClientOptionsBuilder()
            .WithTcpServer(config.Broker, config.Port)
            .WithClientId(config.ClientId)
            .WithCleanSession()
            .WithTls()
            .Build();

        return await _mqttClient.ConnectAsync(options);
    }

    public async Task<MqttClientPublishResult> PublishAsync(string topic, string payload)
    {
        var message = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(payload)
            .Build();

        return await _mqttClient.PublishAsync(message);
    }

    public void OnMessageReceivedAsync(Action<string> callback)
    {
        _mqttClient.ApplicationMessageReceivedAsync += e =>
        {
            return Task.Run(() => callback(e.ApplicationMessage.ConvertPayloadToString()));
        };
    }

    public async Task<MqttClientSubscribeResult> SubscribeAsync(string topic)
    {
        _mqttClient.ApplicationMessageReceivedAsync += e =>
        {
            return Task.CompletedTask;
        };

        var mqttSubscribeOptions = _mqttFactory.CreateSubscribeOptionsBuilder()
                .WithTopicFilter(
                    f =>
                    {
                        f.WithTopic(topic);
                    })
                .Build();

        return await _mqttClient.SubscribeAsync(topic);
    }

    public async Task<MqttClientUnsubscribeResult> UnsubscribeAsync(string topic)
    {
        return await _mqttClient.UnsubscribeAsync(topic);
    }

    public async Task DisconnectAsync()
    {
        await _mqttClient.DisconnectAsync(new MqttClientDisconnectOptionsBuilder().WithReason(MqttClientDisconnectOptionsReason.NormalDisconnection).Build());
    }

    public void Dispose()
    {
        _mqttClient.Dispose();
    }
}
