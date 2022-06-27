using System.Text;
using RabbitMQ.Client;

namespace PublishSender.Send
{

    public class Sender
    {
        public static void Main()
        {

            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {


                channel.ExchangeDeclare(exchange: "Logs",
                                        type: ExchangeType.Fanout);

                int i = 0;
                while (true)
                {

                    string message = "Logs " + i;
                    i++;
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "Logs",
                                        routingKey: "hello",
                                        basicProperties: null,
                                        body: body);

                    Console.WriteLine("[x] sent {0}", message);
                }

            }

            Console.WriteLine("Press Enter to Exit");
            Console.ReadLine();

        }
    }
}