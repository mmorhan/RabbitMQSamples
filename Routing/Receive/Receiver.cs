using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Routing.Receive
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
                channel.ExchangeDeclare(exchange: "direct_exchange",
                                         type: ExchangeType.Direct);

                var queueName = channel.QueueDeclare().QueueName;

                foreach (var severity in args){

                channel.QueueBind(queue:queueName,
                                exchange: "direct_exchange",
                                routingKey: severity);
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