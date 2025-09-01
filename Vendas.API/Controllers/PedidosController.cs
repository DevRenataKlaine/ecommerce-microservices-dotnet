using Microsoft.AspNetCore.Mvc;
using Vendas.API.Models; // Adicionar modelos e DTOs aqui

namespace Vendas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PedidosController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public PedidosController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePedido(Pedido pedido)
        {
            var client = _httpClientFactory.CreateClient("Estoque");

            // 1. Validação de Estoque
            foreach (var item in pedido.Itens)
            {
                var response = await client.GetAsync($"/api/produtos/{item.ProdutoId}/validar-estoque?quantidade={item.Quantidade}");
                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode(StatusCodes.Status503ServiceUnavailable, "Serviço de Estoque indisponível.");
                }
                var disponivel = await response.Content.ReadFromJsonAsync<bool>();
                if (!disponivel)
                {
                    return BadRequest($"Produto ID {item.ProdutoId} com estoque insuficiente.");
                }
            }

            // 2. Lógica para salvar o pedido no banco de dados de Vendas (usando o VendasDbContext)

            // 3. Lógica para publicar no RabbitMQ
            // var message = new { Itens = pedido.Itens.Select(i => new { i.ProdutoId, i.Quantidade }) };
            // _rabbitMQProducer.SendMessage(message, "vendas_queue");

            return Created("", pedido); // Retornar o pedido criado
        }
    }
}