using System.Text;
using RabbitMQ.Client;

ConnectionFactory factory = new()
{
    Uri = new Uri("amqp://guest:guest@localhost:5672"),
    ClientProvidedName = "Rabbit Sender App"
};

var cnn =  await factory.CreateConnectionAsync(cancellationToken: CancellationToken.None);

var channel =  await cnn.CreateChannelAsync();

const string exchangeName = "DemoExchange";
const string routeKey = "demo-route-key";
const string queueName = "DemoQueue";

await channel.ExchangeDeclareAsync(exchangeName, ExchangeType.Direct);
await channel.QueueDeclareAsync(queueName, false,false,false);
await channel.QueueBindAsync(queueName, exchangeName, routeKey);


for (var i = 0; i < 60; i++)
{
    Thread.Sleep(500); // Simulate some delay between messages
    var message = Encoding.UTF8.GetBytes($"Message Send {i}");
    await channel.BasicPublishAsync(exchangeName,routeKey,message);
}

await channel.CloseAsync();
await cnn.CloseAsync();