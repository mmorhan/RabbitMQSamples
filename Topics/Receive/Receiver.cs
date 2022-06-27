using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Topics.Receive
{
    public class MyReceiver
    {
        public static void Main(string[] args)
        {

            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {

                Console.WriteLine("Exchange Declared");
                channel.ExchangeDeclare(exchange: "topic_log",
                                         type: ExchangeType.Topic);

                if (args.Length < 1)
                {
                    Console.Error.WriteLine("Usage: {0} [binding_key...]",
                                    Environment.GetCommandLineArgs()[0]);
                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                    Environment.ExitCode = 1;
                    return;
                }

                var queueName = channel.QueueDeclare().QueueName;
                foreach (var bindingKey in args)
                {

                    channel.QueueBind(queue: queueName,
                                    exchange: "topic_log",
                                    routingKey: bindingKey);
                }


                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine("[x] Received {0} ", message);

                };

                while (true)
                {
                    channel.BasicConsume(queue: queueName,
                                        autoAck: true,
                                        consumer: consumer);
                }

            }
            Console.WriteLine("Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}