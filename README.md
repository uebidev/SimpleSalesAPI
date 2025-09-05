# 🛒 SimpleSalesAPI

> *Uma API de vendas que segue boas práticas de verdade (não aquelas que você vê no YouTube)*

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![Entity Framework](https://img.shields.io/badge/Entity%20Framework-Core-512BD4?style=flat-square)](https://docs.microsoft.com/en-us/ef/)
[![License](https://img.shields.io/badge/License-MIT-green?style=flat-square)](LICENSE)
[![Code Quality](https://img.shields.io/badge/Code%20Quality-Actually%20Good-brightgreen?style=flat-square)]()

## 🎯 **O que é isso?**

Uma API REST para gerenciamento de vendas construída com .NET 8 que **realmente** segue os princípios de Clean Architecture. Não é mais uma dessas APIs onde o Controller faz tudo e a lógica de negócio vive espalhada como confete em carnaval.

### **Por que essa API não é uma merda?**

✅ **Service Layer separado** - Porque lógica de negócio no Controller é coisa de estagiário  
✅ **DTOs adequados** - Não expõe entidades como um animal  
✅ **Configuração decimal correta** - Porque `float` para dinheiro é suicídio profissional  
✅ **Async/Await em tudo** - Performance que não faz o usuário chorar  
✅ **Relacionamentos do EF configurados** - Foreign keys órfãs são crime contra a humanidade  
✅ **Separation of Concerns** - Cada classe tem UMA responsabilidade (que conceito!)  

## 🏗️ **Arquitetura**

```
SimpleSalesAPI/
├── 📁 Controllers/          # Só controla (como deveria ser)
├── 📁 Services/            # Lógica de negócio (não no Controller!)
├── 📁 Models/
│   ├── Entities/           # Entidades do domínio
│   ├── DTOs/              # Para não expor entidades como amador
│   └── Enums/             # Melhor que strings mágicas
├── 📁 Data/
│   ├── Context/           # DbContext configurado direito
│   └── Repositories/      # Se precisar (spoiler: talvez não)
└── 📁 Extensions/         # DI e outras mágicas
```

## 🚀 **Features**

| Endpoint | Método | Descrição | Status |
|----------|--------|-----------|--------|
| `/api/vendas` | GET | Lista todas as vendas | ✅ |
| `/api/vendas/{id}` | GET | Busca venda específica | ✅ |
| `/api/vendas` | POST | Cria nova venda | ✅ |
| `/api/vendas/{id}` | PUT | Atualiza venda | 🚧 |
| `/api/vendas/{id}/cancel` | PATCH | Cancela venda | 🚧 |
| `/api/produtos` | GET | Lista produtos | 🚧 |
| `/api/clientes` | GET | Lista clientes | 🚧 |

## 🛠️ **Stack Técnica**

- **.NET 8** - Porque é 2024, não 2010
- **Entity Framework Core** - ORM que funciona (quando configurado direito)
- **SQL Server** - Database que não quebra quando você olha torto
- **Swagger/OpenAPI** - Documentação automática (dev preguiçoso approved)
- **AutoMapper** - Para mapear DTOs sem morrer de tédio

## 🔧 **Como rodar (sem quebrar tudo)**

### **Pré-requisitos**
```bash
# Você precisa ter instalado (óbvio):
- .NET 8 SDK
- SQL Server (LocalDB serve)
- Visual Studio / VS Code (ou qualquer editor que não seja Notepad)
```

### **Configuração**
```bash
# Clone (espero que saiba fazer isso)
git clone https://github.com/seuusuario/SimpleSalesAPI.git
cd SimpleSalesAPI

# Restore packages
dotnet restore

# Configure a connection string no appsettings.json
# (não commite senha no Git, pelo amor de Turing!)

# Roda as migrations
dotnet ef database update

# Roda a aplicação
dotnet run
```

### **Testando**
```bash
# Swagger UI disponível em:
https://localhost:5001/swagger

# Ou se preferir ser hardcore:
curl -X GET "https://localhost:5001/api/vendas" -H "accept: application/json"
```

## 📊 **Exemplos de Uso**

### **Criar uma venda (finalmente!)**
```json
POST /api/vendas
{
  "clienteId": 1,
  "itens": [
    {
      "produtoId": 1,
      "quantidade": 2
    },
    {
      "produtoId": 2,
      "quantidade": 1
    }
  ]
}
```

### **Response que você vai receber:**
```json
{
  "id": 1,
  "nomeCliente": "João Silva",
  "dataVenda": "2024-09-05T10:30:00Z",
  "valorTotal": 150.50,
  "status": "Pendente",
  "itens": [
    {
      "produtoNome": "Produto A",
      "quantidade": 2,
      "precoUnitario": 50.25,
      "subtotal": 100.50
    }
  ]
}
```

## 🧪 **Testes**

*Em breve... porque dev que não testa merece sofrer com bug em produção* 🚧

## 📝 **TODO (para não esquecer)**

- [ ] Testes unitários (porque YOLO não é estratégia)
- [ ] Validações com FluentValidation
- [ ] Logging estruturado (Serilog)
- [ ] Cache com Redis (quando fizer sentido)
- [ ] Health checks
- [ ] Rate limiting (para proteger dos usuários)
- [ ] Autenticação JWT (quando crescer)

## 🤝 **Contribuindo**

Quer contribuir? Ótimo! Mas segue as regras:

1. **Não faça Controller gordo** - Senão eu rejeito o PR e te julgo
2. **Use async/await** - Blocking calls são pecado capital
3. **Configure decimal direito** - Float para dinheiro = ban automático
4. **Teste antes de commitar** - "Funciona na minha máquina" não é argumento
5. **Commit messages descentes** - "fix stuff" não é commit message

## 📄 **Licença**

MIT License - Porque código bom deve ser livre (ao contrário das suas escolhas arquiteturais anteriores)

---

<div align="center">

**Feito com ❤️ e muito ☕ por um dev que realmente entende de arquitetura**

*"Se você está fazendo lógica de negócio no Controller, nós precisamos conversar."*

</div>
