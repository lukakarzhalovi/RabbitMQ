using System.Text;
using RabbitMQ.Client;

ConnectionFactory factory = new()
{
    Uri = new Uri("amqp://guest:guest@localhost:5672"),
    ClientProvidedName = "Rabbit Sender App"
};

var cnn =  await factory.CreateConnectionAsync(cancellationToken: CancellationToken.None);

var channel =  await cnn.CreateChannelAsync();

var exchangeName = "DemoExchange";
var routeKey = "demo-route-key";
var queueName = "DemoQueue";

await channel.ExchangeDeclareAsync(exchangeName, ExchangeType.Direct);
await channel.QueueDeclareAsync(queueName, false,false,false);
await channel.QueueBindAsync(queueName, exchangeName, routeKey);


var message = Encoding.UTF8.GetBytes("Hello Luka!");
await channel.BasicPublishAsync(exchangeName,routeKey,message);

await channel.CloseAsync();
await cnn.CloseAsync();