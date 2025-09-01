// Vendas.API/Data/VendasDbContext.cs

using Microsoft.EntityFrameworkCore;
using Vendas.API.Models; // Importante: referenciar seus modelos

namespace Vendas.API.Data
{
    public class VendasDbContext : DbContext
    {
        public VendasDbContext(DbContextOptions<VendasDbContext> options) : base(options) { }

        // Mapeia a classe 'Pedido' para uma tabela chamada 'Pedidos'
        public DbSet<Pedido> Pedidos { get; set; }

        // Mapeia a classe 'ItemPedido' para uma tabela chamada 'ItensPedido'
        public DbSet<ItemPedido> ItensPedido { get; set; }
    }
}