using System.Text;
using RabbitMQ.Client;

namespace Routing.Send
{

    public class Sender
    {
        public static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {


                channel.ExchangeDeclare(exchange: "direct_exchange",
                                        type: ExchangeType.Direct);


                int i = 0;
                while (true)
                {

                    string message = args[0] + " " + i;
                    i++;
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "direct_exchange",
                                        routingKey: args[0],
                                        basicProperties: null,
                                        body: body);

                    Console.WriteLine("[x] sent {0}", message);
                    Thread.Sleep(1000);
                }

            }

            Console.WriteLine("Press Enter to Exit");
            Console.ReadLine();

        }
    }
}