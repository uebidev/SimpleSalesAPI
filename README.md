# 🛒 SimpleSalesAPI

> ⚠️ **Projeto de estudo, não concluído.** Criado em setembro de 2025 enquanto eu aprendia Clean Architecture. Ficou parado no meio: algumas coisas citadas abaixo (como o rate limiting) não foram terminadas, e a separação de camadas tem pontos que eu faria diferente hoje.

> **API REST robusta para gerenciamento de vendas com Clean Architecture, validações avançadas e patterns enterprise.**

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![Entity Framework](https://img.shields.io/badge/Entity%20Framework-Core%209.0-512BD4?style=flat-square)](https://docs.microsoft.com/en-us/ef/)
[![FluentValidation](https://img.shields.io/badge/FluentValidation-12.0.0-orange?style=flat-square)](https://fluentvalidation.net/)
[![MySQL](https://img.shields.io/badge/MySQL-8.0-blue?style=flat-square&logo=mysql)](https://www.mysql.com/)
[![Serilog](https://img.shields.io/badge/Serilog-9.0.0-green?style=flat-square)](https://serilog.net/)

## 📋 Visão Geral

SimpleSalesAPI implementa um sistema de gestão de vendas seguindo rigorosamente **Clean Architecture** e princípios **SOLID**. O projeto demonstra aplicação prática de patterns enterprise, exception handling centralizado, logging estruturado e validações robustas com FluentValidation.

### Características Técnicas Implementadas

✅ **Clean Architecture** - Separação rigorosa entre Domain, Application, Infrastructure e Presentation  
✅ **Repository Pattern + Unit of Work** - Abstração da camada de dados com controle transacional  
✅ **FluentValidation** - Sistema de validação com regras de negócio estruturadas  
✅ **Exception Handling Centralizado** - Middleware global com Problem Details compliance  
✅ **Structured Logging** - Serilog com enrichers, correlation tracking e performance monitoring  
✅ **Rate Limiting** - Proteção contra abuse configurada  (esqueci de terminar)
✅ **Response Compression** - Otimização de payload  

## 🏗️ Arquitetura

Implementação de Clean Architecture com separação clara de responsabilidades:

```
SimpleSalesAPI/
├── 📁 SimpleSalesAPI (Presentation)
│   ├── Controllers/              # API endpoints com responsabilidade única
│   ├── Middleware/               # Exception handling e request logging
│   ├── Configuration/            # Serilog e logging configuration
│   └── Program.cs               # Application startup e DI configuration
│
├── 📁 Application (Use Cases)
│   ├── Services/                # Business logic implementation
│   ├── Services/Interfaces/     # Service abstractions
│   ├── Dtos/Requests/          # Input data transfer objects
│   ├── Dtos/Responses/         # Output data transfer objects
│   ├── Validators/             # FluentValidation rules
│   └── DependencyInjection.cs # Application layer DI
│
├── 📁 Domain (Core Business)
│   ├── Models/                 # Domain entities
│   ├── Enums/                  # Domain enumerations
│   └── Exceptions/             # Domain-specific exceptions
│
└── 📁 Infrastructure (External Concerns)
    ├── Data/Context/           # EF Core DbContext
    ├── Data/Configurations/    # Entity Framework configurations
    ├── Data/Repositories/      # Repository implementations
    ├── Data/Migrations/        # Database migrations
    └── DependencyInjection.cs  # Infrastructure DI
```

## 🎯 Patterns e Princípios

### Design Patterns Implementados
- **Repository Pattern** - Abstração de acesso a dados com interface genérica
- **Unit of Work** - Controle de transações e contexto unificado
- **Dependency Injection** - Inversão de controle em todas as camadas
- **Middleware Pattern** - Exception handling e request processing
- **DTO Pattern** - Separação entre entidades de domínio e transferência de dados

### SOLID Compliance
- **SRP** - Classes com responsabilidade única (Services, Repositories, Controllers)
- **OCP** - Extensível via interfaces sem modificação de código existente
- **LSP** - Implementações substituíveis via abstrações bem definidas
- **ISP** - Interfaces específicas e focadas (IBaseRepository, IUnitOfWork)
- **DIP** - Dependência de abstrações, não implementações concretas

## 🚀 API Endpoints

### Gerenciamento de Vendas
| Endpoint | Método | Descrição | Implementado |
|----------|--------|-----------|-------------|
| `/api/vendas` | GET | Lista todas as vendas com relacionamentos | ✅ |
| `/api/vendas/{id}` | GET | Recupera venda específica com detalhes completos | ✅ |
| `/api/vendas` | POST | Cria nova venda com validação de estoque | ✅ |
| `/api/vendas/cliente/{clienteId}` | GET | Histórico de vendas por cliente | ✅ |
| `/api/vendas/status/{status}` | GET | Vendas filtradas por status | ✅ |
| `/api/vendas/periodo` | GET | Vendas por período com query parameters | ✅ |
| `/api/vendas/{id}/confirmar` | PATCH | Transição de status para confirmada | ✅ |
| `/api/vendas/{id}/cancelar` | PATCH | Cancelamento com reversão de estoque | ✅ |
| `/api/vendas/{id}/entregar` | PATCH | Marca como entregue | ✅ |
| `/api/vendas/{id}` | DELETE | Exclusão com controle de estado | ✅ |

### Gestão de Produtos
| Endpoint | Método | Descrição | Implementado |
|----------|--------|-----------|-------------|
| `/api/produtos` | GET | Lista produtos ativos com categorias | ✅ |
| `/api/produtos/{id}` | GET | Detalhes do produto com categoria | ✅ |
| `/api/produtos` | POST | Criação com validação de categoria | ✅ |
| `/api/produtos/{id}` | PUT | Atualização completa do produto | ✅ |
| `/api/produtos/categoria/{categoriaId}` | GET | Produtos filtrados por categoria | ✅ |
| `/api/produtos/search` | GET | Busca com filtros de nome e preço | ✅ |
| `/api/produtos/baixo-estoque` | GET | Relatório de produtos com estoque crítico | ✅ |
| `/api/produtos/{id}/ativar` | PATCH | Ativação de produto | ✅ |
| `/api/produtos/{id}/desativar` | PATCH | Desativação de produto | ✅ |
| `/api/produtos/{id}` | DELETE | Exclusão permanente | ✅ |

### Gerenciamento de Clientes e Categorias
| Endpoint | Método | Descrição | Status |
|----------|--------|-----------|--------|
| `/api/clientes` | * | CRUD completo de clientes | 🟡 Service implementado |
| `/api/categorias` | * | CRUD completo de categorias | 🟡 Service implementado |

*Controllers restantes requerem implementação trivial baseada nos Services existentes.*

## 🛠️ Stack Tecnológica

### Core Framework
- **.NET 8** - Framework com performance e features mais recentes
- **ASP.NET Core** - Web framework para APIs REST
- **Entity Framework Core 9.0** - ORM com Fluent API configurations

### Persistence & Data
- **MySQL 8.0** - Sistema de gerenciamento de banco relacional
- **Pomelo.EntityFrameworkCore.MySql** - Provider MySQL otimizado
- **Entity Framework Migrations** - Controle de versionamento de schema

### Validation & Quality
- **FluentValidation 12.0** - Validação declarativa com rules complexas
- **Serilog 9.0** - Structured logging com enrichers e sinks
- **Rate Limiting** - Proteção contra abuse com System.Threading.RateLimiting

### Documentation & Tools
- **Swagger/OpenAPI** - Documentação interativa automática
- **Problem Details** - RFC 7807 compliance para error responses

## 🔧 Setup e Configuração

### Pré-requisitos
```bash
.NET 8 SDK
MySQL Server 8.0+
IDE: Visual Studio 2022 / JetBrains Rider / VS Code
```

### Instalação
```bash
# Clone do repositório
git clone https://github.com/seu-usuario/SimpleSalesAPI.git
cd SimpleSalesAPI

# Restore de dependências
dotnet restore

# Configuração de connection string (appsettings.json)
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=SimpleSales;Uid=root;Pwd=sua_senha;"
  }
}

# Execução de migrations
dotnet ef database update --project SimpleSalesAPI.Infrastructure --startup-project SimpleSalesAPI

# Execução da aplicação
dotnet run --project SimpleSalesAPI
```

### Verificação
```bash
# Swagger UI
https://localhost:7066/swagger

# Health check endpoint
curl -X GET "https://localhost:7066/health"

# Teste de endpoint
curl -X GET "https://localhost:7066/api/produtos" -H "accept: application/json"
```

## 📊 Exemplos de Uso

### Criação de Venda com Validação
```json
POST /api/vendas
Content-Type: application/json

{
  "clienteId": 1,
  "itens": [
    {
      "produtoId": 1,
      "quantidade": 2
    },
    {
      "produtoId": 5,
      "quantidade": 1
    }
  ]
}
```

### Response com Relacionamentos
```json
{
  "id": 1,
  "cliente": {
    "id": 1,
    "nome": "Ana Carolina Silva",
    "email": "ana.carolina@gmail.com"
  },
  "dataVenda": "2024-09-06T15:30:00Z",
  "valorTotal": 7629.98,
  "status": "Pendente",
  "itens": [
    {
      "id": 1,
      "produtoId": 1,
      "produtoNome": "iPhone 15 128GB",
      "quantidade": 2,
      "precoUnitario": 7499.99,
      "subtotal": 14999.98
    }
  ]
}
```

### Error Response Estruturado
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Dados de entrada inválidos",
  "status": 400,
  "detail": "Um ou mais campos possuem valores inválidos",
  "instance": "/api/vendas",
  "traceId": "0HN7GKQR7VK1L",
  "timestamp": "2024-09-06T15:30:00Z",
  "errors": {
    "clienteId": [
      "ID do cliente deve ser um valor positivo válido"
    ],
    "itens": [
      "Venda deve conter pelo menos um item"
    ]
  }
}
```

## 🧪 Qualidade e Confiabilidade

### Validação Implementada
- **Input Validation** - FluentValidation em todos os request DTOs
- **Business Rules** - Validação de estoque, status transitions, foreign keys
- **Data Integrity** - EF Core constraints e validações de modelo

### Exception Handling
- **NotFoundException** - Resources não localizados com metadata
- **BusinessException** - Violação de regras de negócio com error codes
- **ValidationException** - Dados inválidos com field-level errors
- **InsufficientStockException** - Estoque insuficiente com detalhes específicos

### Observability
- **Structured Logging** - Correlation IDs, performance tracking, enrichers
- **Request Logging** - HTTP requests com timing e status codes
- **Error Tracking** - Exception logging com stack traces e context

## 🔐 Segurança e Performance

### Segurança Implementada
- **Input Sanitization** - Validação rigorosa de entrada
- **SQL Injection Prevention** - Entity Framework parametrizado
- **Rate Limiting** - Proteção contra abuse de endpoints

### Otimizações de Performance
- **Async/Await** - Operações assíncronas em toda a stack
- **AsNoTracking** - Queries read-only otimizadas
- **Response Compression** - Compressão automática de payloads
- **Connection Pooling** - Gerenciamento eficiente de conexões MySQL

## 📈 Status do Projeto

### Implementado ✅
- Clean Architecture com separação de camadas
- CRUD completo para Vendas e Produtos
- Services para Clientes e Categorias
- Exception handling centralizado
- Structured logging com Serilog
- FluentValidation em todos os inputs
- Rate limiting e response compression
- Data seeding com dados realistas

### Pendente 🟡
- API versioning
- Authentication/Authorization
- Caching layer
- Integration tests
- CI/CD pipeline

### Futuro 🔄
- Real-time notifications (SignalR)
- Background jobs (Hangfire)
- Distributed caching (Redis)
- Monitoring e APM integration

## 🤝 Contribuição

### Standards
- Seguir Clean Architecture principles
- Manter cobertura de validação completa
- Implementar tests para novos services
- Documentar endpoints com XML comments
- Usar Conventional Commits

### Processo
1. Fork e clone do repositório
2. Create feature branch (`feature/nova-funcionalidade`)
3. Implementar com testes adequados
4. Commit seguindo padrões convencionais
5. Submit pull request com descrição detalhada

## 📄 Licença

MIT License - Consulte [LICENSE](LICENSE) para detalhes completos.

---

<div align="center">

**API construída com foco em arquitetura limpa, qualidade de código e maintainability**

[**Swagger UI**](https://localhost:7066/swagger) • [**Health Check**](https://localhost:7066/health) • [**Logs**](./logs/)

</div>
