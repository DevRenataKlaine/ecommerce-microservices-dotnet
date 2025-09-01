using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Vendas.API.Services
{
    public class RabbitMQProducer : IRabbitMQProducer
    {
        private readonly IConfiguration _configuration;

        public RabbitMQProducer(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SendMessage<T>(T message, string queueName)
        {
            var factory = new ConnectionFactory { HostName = _configuration["RabbitMQ:HostName"] };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);
        }
    }
}

namespace Vendas.API.Services
{
    public interface IRabbitMQProducer
    {
        void SendMessage<T>(T message, string queueName);
    }
}