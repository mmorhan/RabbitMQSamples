using System.Text;
using RabbitMQ.Client;

namespace WorkQueues.NewTask
{
    public class TaskQueue
    {
        public static void Main(string[] args)
        {

            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                
                channel.QueueDeclare(queue:"task_queue",
                                    durable:false,
                                    exclusive:false,
                                    autoDelete:false,
                                    arguments:null);

                while(true){

                var message = GetMessage(args);

                var body = Encoding.UTF8.GetBytes(message);
                var properties = channel.CreateBasicProperties();
                properties.Persistent=true;

                channel.BasicPublish(exchange:"",
                                    routingKey:"task_queue",
                                    basicProperties:properties,
                                    body:body);

                Console.WriteLine(" [x] Sent {0}",message);
                Thread.Sleep(1000);

                }
            
            }


            Console.WriteLine("Press [Enter] to Exit");
            Console.ReadLine();
        }

        public static string GetMessage(string[] args)
        {
            return ((args.Length > 0) ? string.Join(" ", args) : "Hello World!");
        }
    }
}