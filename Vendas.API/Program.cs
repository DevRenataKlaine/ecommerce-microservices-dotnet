using Microsoft.EntityFrameworkCore;
using Vendas.API.Data;
using Vendas.API.Services;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

// ===================================================================
// 1. ADICIONAR SERVIÇOS AO CONTAINER (INJEÇÃO DE DEPENDÊNCIA)
// TODAS as chamadas builder.Services.Add... DEVEM vir aqui.
// ===================================================================

// Configura o DbContext para o Entity Framework Core, usando a connection string do appsettings.json
builder.Services.AddDbContext<VendasDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configura o HttpClient para se comunicar com o Microserviço de Estoque
builder.Services.AddHttpClient("Estoque", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:EstoqueUrl"]);
});

// Registra nossa classe que envia mensagens para o RabbitMQ
builder.Services.AddSingleton<IRabbitMQProducer, RabbitMQProducer>();

// Adiciona os serviços essenciais para que os Controllers de API funcionem
builder.Services.AddControllers();

// Adiciona os serviços do Swagger/OpenAPI para documentação da API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// ===================================================================
// 2. CONSTRUIR A APLICAÇÃO
// Esta linha "fecha" a configuração dos serviços e cria o app.
// ===================================================================
var app = builder.Build();


// ===================================================================
// 3. CONFIGURAR O PIPELINE DE REQUISIÇÕES HTTP
// A ordem das chamadas app.Use... é importante.
// ===================================================================

// Habilita o Swagger apenas em ambiente de desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Redireciona requisições HTTP para HTTPS
app.UseHttpsRedirection();

// Adiciona autorização (se você for usar o atributo [Authorize])
app.UseAuthorization();

// Mapeia as rotas definidas nos seus Controllers (ex: PedidosController)
app.MapControllers();


// ===================================================================
// 4. EXECUTAR A APLICAÇÃO
// ===================================================================
app.Run();