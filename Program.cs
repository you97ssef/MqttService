using MqttService;
using MqttService.MqttNet;
using MqttService.M2;

var topic = "fizzbuzz/hundred";
var config = new Config
{
    Broker = "broker.hivemq.com",
    Port = 8883,
    ClientId = "fizzbuzz"
};

// MqttNet ---------------------------------------------------------------------
Console.WriteLine("MqttNet running");
var netService = new MqttNetService();

await netService.ConnectAsync(config);
await netService.SubscribeAsync(topic);

netService.OnMessageReceivedAsync(checkReceivedMessageAsync);

for (int i = 0; i < 100; i++)
{
    await netService.PublishAsync(topic, i.ToString());
    await Task.Delay(100);
}

await netService.UnsubscribeAsync(topic);
await netService.DisconnectAsync();
netService.Dispose();
// -----------------------------------------------------------------------------


// M2 --------------------------------------------------------------------------
Console.WriteLine("M2 running");
var m2Service = new M2MqttService();

m2Service.Connect(config);
m2Service.SubscribeAsync(topic);

m2Service.OnMessageReceivedAsync(checkReceivedMessageAsync);

for (int i = 0; i < 100; i++)
{
    m2Service.PublishAsync(topic, i.ToString());
    await Task.Delay(100);
}

m2Service.UnsubscribeAsync(topic);
m2Service.DisconnectAsync();
// -----------------------------------------------------------------------------

Console.WriteLine("Press any button to exit.");
Console.ReadLine();

// This is the callback that is called when a message is received.
void checkReceivedMessageAsync(string message)
{
    int.TryParse(message, out var result);
    if (result % 3 == 0 && result % 5 == 0)
    {
        Console.WriteLine("FizzBuzz");
    }
    else if (result % 3 == 0)
    {
        Console.WriteLine("Fizz");
    }
    else if (result % 5 == 0)
    {
        Console.WriteLine("Buzz");
    }
    else
    {
        Console.WriteLine(result);
    }
}
