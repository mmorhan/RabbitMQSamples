using System.Text;
using RabbitMQ.Client;

namespace Topics.Send
{

    public class Sender
    {
        public static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {


                channel.ExchangeDeclare(exchange: "topic_log",
                                        type: ExchangeType.Topic);


                var routingKey=(args.Length>0)?args[0]:"anonymous.info";
                int i = 0;
                while (true)
                {

                    string message = routingKey + " " + i;
                    i++;
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "topic_log",
                                        routingKey: routingKey,
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