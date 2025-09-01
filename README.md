O sistema foi decomposto em dois serviços principais e independentes: um para a **Gestão de Estoque** de produtos e outro para a **Gestão de Vendas**. A comunicação entre o cliente e os serviços foi centralizada através de um **API Gateway**, e a comunicação entre os serviços utilizou uma abordagem híbrida (síncrona e assíncrona) para garantir performance e resiliência.
O objetivo foi simular um ambiente corporativo robusto e escalável, aplicando boas práticas de design de software e desacoplamento de responsabilidades.

### **Tecnologias Utilizadas**

- **Backend:** .NET e C#
- **Framework de API:** ASP.NET Core Web API
- **Acesso a Dados (ORM):** Entity Framework Core
- **Banco de Dados:** SQL Server (executado via Docker)
- **Mensageria (Message Broker):** RabbitMQ (executado via Docker)
- **API Gateway:** Ocelot
- **Containerização:** Docker (para orquestrar o ambiente de desenvolvimento)
- **Arquitetura:** Microserviços, API RESTful, Comunicação Orientada a Eventos.

### **Funcionalidades**

- **Estrutura de Microserviços:**
    - Criação de projetos separados e independentes para `Estoque.API`, `Vendas.API` e `APIGateway`.
- **Microserviço de Estoque:**
    - Endpoints para cadastro e consulta de produtos.
    - Um endpoint específico para validação de estoque, a ser consumido pelo serviço de Vendas.
    - Um serviço em background (`Consumer`) que escuta eventos do RabbitMQ para dar baixa no estoque de forma assíncrona e segura.
- **Microserviço de Vendas:**
    - Endpoint para criação de pedidos.
    - Implementação da **comunicação síncrona** (via `HttpClient`) para validar a disponibilidade de produtos no serviço de Estoque antes de confirmar uma venda.
    - Implementação da **comunicação assíncrona** (via `Producer`), publicando uma mensagem no RabbitMQ após a venda ser concluída.
- **API Gateway:**
    - Configuração do Ocelot para atuar como um ponto de entrada único, roteando as requisições para os microserviços corretos.
- **Persistência de Dados:**
    - Aplicação do padrão **"Database per Service"**, com cada microserviço gerenciando seu próprio banco de dados (`EstoqueDB` e `VendasDB`), garantindo total desacoplamento.
