using DAL;
using DTO;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace QueueListener
{
    class Program
    {
        private static INewsRepository newsRepository = new NewsRepository(new MongoConfig()
        {
            ConnectionString = "mongodb://localhost",
            Database = "news"
        });

        static void Main(string[] args)
        {
            ConnectionFactory factory = new ConnectionFactory();

            factory.UserName = "guest";
            factory.Password = "guest";
            factory.VirtualHost = "/";
            factory.HostName = "localhost";

            using (IConnection conn = factory.CreateConnection())
            using (var model = conn.CreateModel())
            {
                model.QueueDeclare("news", false, false, false, null);

                var consumer = new EventingBasicConsumer(model);

                consumer.Received += Consumer_Received;

                model.BasicConsume(queue: "news",
                                     autoAck: true,
                                     consumer: consumer);


                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }

        private static void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            var body = e.Body;
                        
            NewsDTO n = BinaryConverter.ByteArrayToObject<NewsDTO>(body);

            Console.WriteLine(n);

            newsRepository.AddNews(n);
        }

    }
}

   
