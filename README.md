# 🛒 SimpleSalesAPI

> *Uma API REST robusta para gerenciamento de vendas, construída seguindo princípios de Clean Architecture e boas práticas de desenvolvimento.*

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![Entity Framework](https://img.shields.io/badge/Entity%20Framework-Core-512BD4?style=flat-square)](https://docs.microsoft.com/en-us/ef/)
[![FluentValidation](https://img.shields.io/badge/FluentValidation-11.9.0-orange?style=flat-square)](https://fluentvalidation.net/)
[![MySQL](https://img.shields.io/badge/MySQL-8.0-blue?style=flat-square&logo=mysql)](https://www.mysql.com/)
[![License](https://img.shields.io/badge/License-MIT-green?style=flat-square)](LICENSE)

## 📋 **Visão Geral**

SimpleSalesAPI é uma API REST desenvolvida em .NET 8 que implementa um sistema completo de gestão de vendas, seguindo rigorosamente os princípios de **Clean Architecture** e **SOLID**. O projeto demonstra a aplicação prática de padrões de design enterprise, validações robustas e separação adequada de responsabilidades.

### **Características Técnicas Principais**

✅ **Clean Architecture** - Separação clara entre camadas de domínio, aplicação e infraestrutura  
✅ **Repository Pattern + Unit of Work** - Abstração adequada da camada de dados  
✅ **FluentValidation** - Sistema robusto de validação com mensagens estruturadas  
✅ **DTOs especializados** - Separação entre entidades de domínio e objetos de transferência  
✅ **Exception Handling** - Tratamento centralizado de erros com tipos específicos  
✅ **Dependency Injection** - Inversão de controle e injeção de dependências  
✅ **Async/Await** - Operações assíncronas para melhor performance  

## 🏗️ **Arquitetura**

O projeto segue uma arquitetura em camadas bem definida, implementando os princípios de Clean Architecture:

```
SimpleSalesAPI/
├── 📁 SimpleSalesAPI (Presentation Layer)
│   ├── Controllers/                  # Controllers lean com responsabilidade única
│   ├── Middleware/                   # Middleware personalizado
│   └── Program.cs                    # Configuração da aplicação
│
├── 📁 SimpleSalesAPI.Application (Application Layer)
│   ├── Services/                     # Services com lógica de negócio
│   ├── Services.Interfaces/          # Abstrações dos services
│   ├── DTOs/                        # Data Transfer Objects
│   ├── Validators/                   # Validações com FluentValidation
│   └── DependencyInjection.cs       # Configuração de DI da camada
│
├── 📁 SimpleSalesAPI.Domain (Domain Layer)
│   ├── Models/                       # Entidades de domínio
│   ├── Enums/                       # Enumerações do domínio
│   └── ValueObjects/                # Value Objects (futuro)
│
└── 📁 SimpleSalesAPI.Infrastructure (Infrastructure Layer)
    ├── Data/Context/                # DbContext e configurações
    ├── Data/Repositories/           # Implementação de repositórios
    └── DependencyInjection.cs       # Configuração de DI da camada
```

## 🎯 **Padrões e Princípios Implementados**

### **Design Patterns**
- **Repository Pattern** - Abstração da camada de acesso a dados
- **Unit of Work** - Controle de transações e contexto unificado
- **Dependency Injection** - Inversão de controle e desacoplamento
- **DTO Pattern** - Separação entre entidades e objetos de transferência

### **Princípios SOLID**
- **Single Responsibility** - Cada classe possui uma única responsabilidade
- **Open/Closed** - Extensível para novos comportamentos sem modificar código existente
- **Liskov Substitution** - Interfaces bem definidas e substituíveis
- **Interface Segregation** - Interfaces específicas e focadas
- **Dependency Inversion** - Dependência de abstrações, não de implementações

### **Clean Architecture Compliance**
- **Independence of Frameworks** - Core domain independente de tecnologias
- **Testability** - Arquitetura permite testes unitários eficazes
- **Independence of UI** - Lógica não acoplada à camada de apresentação
- **Independence of Database** - Domain independente de tecnologia de persistência

## 🚀 **Recursos da API**

### **Gerenciamento de Vendas**
| Endpoint | Método | Descrição | Status |
|----------|--------|-----------|--------|
| `/api/vendas` | GET | Lista todas as vendas com relacionamentos | ✅ |
| `/api/vendas/{id}` | GET | Recupera venda específica com detalhes | ✅ |
| `/api/vendas` | POST | Cria nova venda com validação robusta | ✅ |
| `/api/vendas/{id}/confirmar` | PATCH | Confirma venda pendente | ✅ |
| `/api/vendas/{id}/cancelar` | PATCH | Cancela venda com reversão de estoque | ✅ |
| `/api/vendas/{id}/entregar` | PATCH | Marca venda como entregue | ✅ |

### **Gestão de Produtos**
| Endpoint | Método | Descrição | Status |
|----------|--------|-----------|--------|
| `/api/produtos` | GET | Lista produtos ativos com categorias | ✅ |
| `/api/produtos/{id}` | GET | Detalhes do produto | ✅ |
| `/api/produtos` | POST | Cria produto com validação | ✅ |
| `/api/produtos/{id}` | PUT | Atualiza produto existente | ✅ |
| `/api/produtos/search` | GET | Busca produtos com filtros | ✅ |
| `/api/produtos/baixo-estoque` | GET | Relatório de produtos com estoque baixo | ✅ |

### **Relacionamento Cliente-Venda**
| Endpoint | Método | Descrição | Status |
|----------|--------|-----------|--------|
| `/api/clientes` | GET | Lista todos os clientes | ✅ |
| `/api/clientes/{id}/vendas` | GET | Histórico de vendas do cliente | ✅ |
| `/api/categorias/{id}/produtos` | GET | Produtos por categoria | ✅ |

## 🛠️ **Stack Tecnológica**

### **Core Framework**
- **.NET 8** - Framework principal com performance otimizada
- **ASP.NET Core** - Framework web para APIs REST
- **Entity Framework Core** - ORM para acesso a dados

### **Database & Storage**
- **MySQL 8.0** - Sistema de gerenciamento de banco de dados
- **Entity Framework Migrations** - Controle de versão do schema

### **Validation & Serialization**
- **FluentValidation** - Validação robusta com mensagens customizadas
- **System.Text.Json** - Serialização JSON de alta performance

### **Documentation & Testing**
- **Swagger/OpenAPI** - Documentação interativa da API
- **Postman Collections** - Coleções de testes disponíveis

## 🔧 **Configuração e Execução**

### **Pré-requisitos**
```bash
- .NET 8 SDK
- MySQL Server 8.0+
- Visual Studio 2022 / VS Code
- Git
```

### **Instalação**
```bash
# Clone o repositório
git clone https://github.com/seuusuario/SimpleSalesAPI.git
cd SimpleSalesAPI

# Restore packages
dotnet restore

# Configure a connection string no appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=SimpleSalesDB;Uid=seu_usuario;Pwd=sua_senha;"
  }
}

# Execute as migrations
dotnet ef database update --project SimpleSalesAPI.Infrastructure

# Execute a aplicação
dotnet run --project SimpleSalesAPI
```

### **Verificação da Instalação**
```bash
# Health Check
curl -X GET "https://localhost:5001/swagger"

# Teste básico da API
curl -X GET "https://localhost:5001/api/categorias" -H "accept: application/json"
```

## 📊 **Exemplos de Uso**

### **Criação de Venda Completa**
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
      "produtoId": 3,
      "quantidade": 1
    }
  ]
}
```

### **Response Estruturado**
```json
{
  "id": 1,
  "cliente": {
    "id": 1,
    "nome": "João Silva",
    "email": "joao@email.com"
  },
  "dataVenda": "2024-09-05T14:30:00Z",
  "valorTotal": 299.90,
  "status": "Pendente",
  "itens": [
    {
      "id": 1,
      "produtoId": 1,
      "produtoNome": "Smartphone Galaxy",
      "quantidade": 2,
      "precoUnitario": 99.95,
      "subtotal": 199.90
    }
  ]
}
```

### **Validação de Entrada**
```json
// Request inválido
POST /api/vendas
{
  "clienteId": -1,
  "itens": []
}

// Response de validação
{
  "type": "ValidationError",
  "title": "Dados inválidos",
  "status": 400,
  "errors": {
    "clienteId": [
      {
        "message": "ID do cliente deve ser um valor positivo válido",
        "code": "INVALID_CLIENTE_ID"
      }
    ],
    "itens": [
      {
        "message": "Venda deve conter pelo menos um item"
      }
    ]
  }
}
```

## 🧪 **Qualidade e Testes**

### **Validações Implementadas**
- **Entrada de dados** - FluentValidation em todos os DTOs
- **Regras de negócio** - Validações de estoque, status e relacionamentos
- **Integridade referencial** - Verificação de foreign keys antes de operações

### **Exception Handling**
- **NotFoundException** - Recursos não encontrados
- **BusinessException** - Regras de negócio violadas
- **ValidationException** - Dados de entrada inválidos
- **InsufficientStockException** - Estoque insuficiente para operação

### **Logging e Monitoramento**
- **Structured Logging** - Preparado para implementação
- **Performance Monitoring** - Queries otimizadas com AsNoTracking
- **Error Tracking** - Exception handling centralizado

## 🔐 **Segurança e Performance**

### **Validação de Entrada**
- **Input Sanitization** - Limpeza automática de dados
- **SQL Injection Prevention** - Entity Framework parametrizado
- **Data Type Validation** - Validação rigorosa de tipos e formatos

### **Performance Optimizations**
- **Async Operations** - Todas as operações de I/O são assíncronas
- **Query Optimization** - Includes explícitos e AsNoTracking quando adequado
- **Connection Pooling** - Gerenciamento eficiente de conexões

## 📈 **Roadmap e Expansões**

### **Próximas Implementações**
- [ ] **Autenticação JWT** - Sistema de login e autorização
- [ ] **Rate Limiting** - Proteção contra abuse da API
- [ ] **Caching** - Redis para otimização de consultas
- [ ] **Logging Estruturado** - Serilog com enrichers
- [ ] **Health Checks** - Monitoramento de dependências
- [ ] **API Versioning** - Suporte a múltiplas versões

### **Integrações Futuras**
- [ ] **AI-Powered Analytics** - Análise inteligente de vendas
- [ ] **Payment Integration** - Gateway de pagamentos
- [ ] **Email Notifications** - Sistema de notificações
- [ ] **Real-time Updates** - SignalR para atualizações em tempo real

## 🤝 **Contribuições**

### **Padrões de Código**
- Seguir princípios SOLID e Clean Architecture
- Manter cobertura de validação em 100% dos inputs
- Implementar testes unitários para novos services
- Documentar APIs com XML comments para Swagger

### **Process de Contribuição**
1. Fork do repositório
2. Criar branch feature (`git checkout -b feature/nova-funcionalidade`)
3. Commit das mudanças seguindo Conventional Commits
4. Push para a branch (`git push origin feature/nova-funcionalidade`)
5. Abrir Pull Request com descrição detalhada

## 📄 **Licença**

Este projeto está licenciado sob a MIT License - veja o arquivo [LICENSE](LICENSE) para detalhes.

---

<div align="center">

**Desenvolvido com foco em qualidade, performance e manutenibilidade**

*Uma API que demonstra a aplicação prática de padrões enterprise em .NET*

**[Documentação da API](https://localhost:5001/swagger)** | **[Relatório de Cobertura](#)** | **[Performance Benchmarks](#)**

</div>
