using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RPC.RPCServer
{
    public static class RPCServer
    {

        public static void Main(string[] args)
        {

            // Console.WriteLine(fib(3));
            // Console.WriteLine(fib(8));
            // Console.ReadLine();
            var factory = new ConnectionFactory() { HostName = "localhost" };


            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(
                    "rpc_queue", 
                    durable: false, 
                    exclusive: false, 
                    autoDelete: false, 
                    arguments: null);

                channel.BasicQos(0, 1, false);

                var consumer = new EventingBasicConsumer(channel);
                channel.BasicConsume(
                        queue: "rpc_queue",
                        autoAck: false,
                        consumer: consumer
                );
                Console.WriteLine(" [x] Waiting for RPC Request");

                consumer.Received += (model, ea) =>
                {

                    string response = null;

                    var body = ea.Body.ToArray();
                    var props = ea.BasicProperties;
                    var replyProps = channel.CreateBasicProperties();
                    replyProps.CorrelationId = props.CorrelationId;

                    try
                    {
                        var message = Encoding.UTF8.GetString(body);
                        int n = int.Parse(message);
                        Console.WriteLine(" [.] fib{0} ", message);
                        response = fib(n).ToString();
                    }

                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        Console.WriteLine(" [.] " + e.Message);
                        response = "";
                    }

                    finally
                    {
                        var responseBytes = Encoding.UTF8.GetBytes(response);

                        channel.BasicPublish(
                            exchange:"",
                            routingKey:props.ReplyTo,
                            basicProperties:replyProps,
                            body:responseBytes
                        );

                        channel.BasicAck(
                            deliveryTag:ea.DeliveryTag,
                            multiple:false
                        );
                    }

                };
                Console.WriteLine("Press [Enter] to Exit ");
                Console.ReadLine();
            }

        }

        public static int fib(int n)
        {
            return n == 0 || n == 1 ? n : fib(n-1)  + fib(n-2);
        }
    }
}