using RabbitMQ.Client;
using RabbitMQ.Client.Events;

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

var consumer = new AsyncEventingBasicConsumer(channel);

consumer.ReceivedAsync += async (model, args) =>
{
    var body = args.Body.ToArray();
    var message = System.Text.Encoding.UTF8.GetString(body);
    Console.WriteLine($"Received message: {message}");
    
    // Acknowledge the message
    await channel.BasicAckAsync(args.DeliveryTag, false);
};

var consumerTag = await channel.BasicConsumeAsync(queueName,false,consumer);

Console.ReadLine();

await channel.BasicCancelAsync(consumerTag);

await channel.CloseAsync();
await cnn.CloseAsync();