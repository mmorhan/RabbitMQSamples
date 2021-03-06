using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace WorkQueues.NewTask{

    public class Worker{
        public static void Main(string[] args){
            
            var factory= new ConnectionFactory(){HostName="localhost"};
            using (var connection=factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {


                channel.QueueDeclare(queue:"task_queue",
                                    durable:false,
                                    exclusive:false,
                                    autoDelete:false,
                                    arguments:null);

                var consumer=  new EventingBasicConsumer(channel);
                
                consumer.Received += (model,ea)=>{
                    var body=ea.Body.ToArray();
                    var message=Encoding.UTF8.GetString(body);
                    Console.WriteLine(" [x] Received {0} ",message);
                    
                    int dots= message.Split(".").Length-1;
                    Thread.Sleep(dots*1000);

                    Console.WriteLine(" [x] Done");

                };
                while (true){

                channel.BasicConsume(queue:"task_queue",autoAck:true,consumer:consumer);

                }
            }

            Console.WriteLine("Press [Enter] to Exit");
            Console.ReadLine();

        }
    }
}