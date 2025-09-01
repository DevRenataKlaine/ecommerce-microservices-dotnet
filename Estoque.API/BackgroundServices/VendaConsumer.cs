using Estoque.API.Data;
using Estoque.API.DTOs;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Estoque.API.BackgroundServices
{
    public class VendaConsumer : BackgroundService
    {
        private readonly IConnection _connection;
    private readonly RabbitMQ.Client.IModel _channel;
        private readonly IServiceProvider _serviceProvider;
        private const string QueueName = "vendas_queue";

        public VendaConsumer(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            var factory = new ConnectionFactory { HostName = configuration["RabbitMQ:HostName"] };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel(); // RabbitMQ.Client.IModel
            _channel.QueueDeclare(queue: QueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new RabbitMQ.Client.Events.EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var vendaEvent = JsonSerializer.Deserialize<VendaRealizadaEvent>(message);

                if (vendaEvent != null)
                {
                    await AtualizarEstoque(vendaEvent);
                }
            };

            _channel.BasicConsume(queue: QueueName, autoAck: true, consumer: consumer);
            return Task.CompletedTask;
        }

        private async Task AtualizarEstoque(VendaRealizadaEvent vendaEvent)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<EstoqueDbContext>();

            foreach (var item in vendaEvent.Itens)
            {
                var produto = await dbContext.Produtos.FindAsync(item.ProdutoId);
                if (produto != null && produto.QuantidadeEmEstoque >= item.Quantidade)
                {
                    produto.QuantidadeEmEstoque -= item.Quantidade;
                }
            }
            await dbContext.SaveChangesAsync();
        }
    }
}