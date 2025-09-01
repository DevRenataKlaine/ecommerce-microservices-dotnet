// Vendas.API/Models/Pedido.cs

namespace Vendas.API.Models
{
    // Representa a tabela 'Pedidos' no banco de dados
    public class Pedido
    {
        // Chave primária do pedido
        public int Id { get; set; }

        // Data em que o pedido foi criado. 'DateTime.UtcNow' garante um padrão global.
        public DateTime DataPedido { get; set; } = DateTime.UtcNow;

        // Relação de Um-para-Muitos. Um Pedido tem muitos Itens.
        // O Entity Framework entende que isso é uma coleção de itens relacionados.
        public List<ItemPedido> Itens { get; set; } = new List<ItemPedido>();
    }

    // Representa a tabela 'ItensPedido' no banco de dados
    public class ItemPedido
    {
        // Chave primária do item
        public int Id { get; set; }

        // Chave estrangeira para ligar este item a um produto no Microserviço de Estoque.
        public int ProdutoId { get; set; }

        // Quantas unidades deste produto foram compradas.
        public int Quantidade { get; set; }

        // O preço do produto no momento da compra.
        // É importante salvar o preço aqui para o histórico, pois o preço no serviço de estoque pode mudar no futuro.
        public decimal PrecoUnitario { get; set; }
    }
}