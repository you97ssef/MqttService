using System.Text;
using uPLibrary.Networking.M2Mqtt;
using MqttService;

namespace MqttService.M2;

public class M2MqttService
{
    private uPLibrary.Networking.M2Mqtt.MqttClient? _mqttClient;

    public void Connect(Config config)
    {
        _mqttClient = new uPLibrary.Networking.M2Mqtt.MqttClient(config.Broker, config.Port, true, null, null, uPLibrary.Networking.M2Mqtt.MqttSslProtocols.None, null, null);
        _mqttClient.Connect(config.ClientId);
    }

    public void PublishAsync(string topic, string payload)
    {
        _mqttClient?.Publish(topic, Encoding.UTF8.GetBytes(payload));
    }

    public void OnMessageReceivedAsync(Action<string> callback)
    {
        _mqttClient!.MqttMsgPublishReceived += (sender, e) =>
        {
            callback(Encoding.UTF8.GetString(e.Message));
        };
    }
    
    public void SubscribeAsync(string topic)
    {
        _mqttClient!.Subscribe(new[] { topic }, new[] { uPLibrary.Networking.M2Mqtt.Messages.MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });
    }

    public void UnsubscribeAsync(string topic)
    {
        _mqttClient!.Unsubscribe(new[] { topic });
    }

    public void DisconnectAsync()
    {
        _mqttClient!.Disconnect();
    }
}