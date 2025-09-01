namespace Estoque.API.DTOs
{
    public class VendaRealizadaEvent
    {
        // Solução: Inicialize a lista para garantir que ela nunca seja nula.
        public List<ItemVenda> Itens { get; set; } = new List<ItemVenda>();
    }

    public class ItemVenda
    {
        public int ProdutoId { get; set; }
        public int Quantidade { get; set; }
    }
}