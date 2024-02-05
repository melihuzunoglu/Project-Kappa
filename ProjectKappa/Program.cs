using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

ConnectionFactory connectionFactory = new();
connectionFactory.Uri = new Uri(uriString: "amqp://guest:guest@192.168.16.1:5672");
connectionFactory.ClientProvidedName = "ProjectKappa";

IConnection connection = connectionFactory.CreateConnection();
IModel channel = connection.CreateModel();

string exchangeName = "zeta-exchange";
string routingKey = "zeta-routing-key";
string queueName = "zeta-queue";

channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
channel.QueueDeclare(queueName, false, false, false, null);
channel.QueueBind(queueName, exchangeName, routingKey, null);
channel.BasicQos(0, 1, false);

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (sender, args) =>
{ 
    System.Threading.Thread.Sleep(10000); //This line added for observe the que

    var body = args.Body.ToArray();
    string message = Encoding.UTF8.GetString(body);
    Console.WriteLine("Message Received: " + message);
    channel.BasicAck(args.DeliveryTag, false);
};

string consumerTag = channel.BasicConsume(queueName, false, consumer);
Console.ReadLine();
channel.BasicCancel(consumerTag);
channel.Close();
connection.Close();