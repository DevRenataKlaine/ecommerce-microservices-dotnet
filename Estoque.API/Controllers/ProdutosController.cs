using Estoque.API.Data;
using Estoque.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Estoque.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutosController : ControllerBase
    {
        private readonly EstoqueDbContext _context;

        public ProdutosController(EstoqueDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}/validar-estoque")]
        public async Task<IActionResult> ValidarEstoque(int id, [FromQuery] int quantidade)
        {
            var produto = await _context.Produtos.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
            if (produto == null) return NotFound();

            return Ok(produto.QuantidadeEmEstoque >= quantidade);
        }

        [HttpGet]
        public async Task<IActionResult> GetProdutos() => Ok(await _context.Produtos.ToListAsync());

        [HttpPost]
        public async Task<IActionResult> CreateProduto(Produto produto)
        {
            _context.Produtos.Add(produto);
            await _context.SaveChangesAsync();
            return Ok(produto);
        }
    }
}