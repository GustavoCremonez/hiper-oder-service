# Hiper Order System

Sistema de gerenciamento de pedidos desenvolvido com .NET 8 e Clean Architecture, implementado como desafio técnico para a vaga de Engenheiro de Software PL na Hiper Software SA.

## Stack Tecnológica

### Backend
- .NET 8.0
- C# 12
- Entity Framework Core 8.0
- PostgreSQL 16
- RabbitMQ 3.x
- Clean Architecture

### Frontend
- Vue.js 3
- TypeScript 5
- Vite
- Tailwind CSS 3

### DevOps
- Docker
- Docker Compose

## Arquitetura

O projeto segue os princípios de Clean Architecture, separado em camadas:

```
src/
├── Hiper.Domain          # Entidades, Value Objects, Interfaces
├── Hiper.Application     # Use Cases, Commands, Queries, DTOs
├── Hiper.Infrastructure  # Repositórios, EF Core, RabbitMQ
└── Hiper.API            # Controllers, Middlewares, Program.cs
```

### Principais Padrões
- **Clean Architecture**: Separação clara de responsabilidades
- **Result Pattern**: Tratamento de erros sem exceções
- **Factory Methods**: Criação de entidades com validação de invariantes
- **Repository Pattern**: Abstração de persistência
- **CQRS Simplificado**: Separação de Commands e Queries
- **Domain-Driven Design**: Modelagem rica do domínio

## Pré-requisitos

- Docker e Docker Compose
- .NET 8 SDK (opcional, para desenvolvimento local)
- Node.js 18+ (opcional, para desenvolvimento do frontend)

## Executando o Projeto

### Com Docker (Recomendado)

```bash
# Subir backend (API + PostgreSQL + RabbitMQ)
cd hiper-oder-service
docker-compose up -d

# Ver logs
docker-compose logs -f api

# Parar todos os serviços
docker-compose down
```

### Frontend (Localmente)

```bash
cd hiper-web
npm install
npm run dev
```

### Backend Localmente (Desenvolvimento)

```bash
cd hiper-oder-service

# Executar API
dotnet run --project src/Hiper.API

# Executar testes
dotnet test
```

## Acessos

- **Frontend**: http://localhost:5173
- **API**: http://localhost:5000
- **Swagger**: http://localhost:5000/swagger
- **RabbitMQ Management**: http://localhost:15672 (guest/guest)
- **PostgreSQL**: localhost:5432 (postgres/postgres)

## Endpoints da API

### Pedidos

| Método | Rota | Descrição |
|--------|------|-----------|
| POST | /api/orders | Criar novo pedido |
| GET | /api/orders | Listar todos os pedidos |
| GET | /api/orders/{id} | Buscar pedido por ID |
| PUT | /api/orders/{id}/status | Atualizar status do pedido |
| DELETE | /api/orders/{id} | Cancelar pedido |

### Exemplo de Request - Criar Pedido

```json
{
  "customerName": "João Silva",
  "customerEmail": "joao@example.com",
  "items": [
    {
      "productName": "Notebook Dell",
      "quantity": 1,
      "unitPrice": 3500.00
    },
    {
      "productName": "Mouse Logitech",
      "quantity": 2,
      "unitPrice": 150.00
    }
  ]
}
```

## Fluxo de Criação de Pedido

1. Cliente envia POST /api/orders
2. API valida dados e cria pedido com status `Pending`
3. Pedido é persistido no PostgreSQL
4. Evento `OrderCreatedEvent` é publicado no RabbitMQ
5. Consumer recebe evento assincronamente
6. Status do pedido é atualizado para `Confirmed`
7. Cliente pode consultar o pedido atualizado

## Status de Pedidos

- **Pending**: Criado, aguardando confirmação
- **Confirmed**: Confirmado via mensageria
- **Processing**: Em processamento
- **Completed**: Finalizado
- **Cancelled**: Cancelado

### Transições Permitidas
- Pending → Confirmed (automático via RabbitMQ)
- Confirmed → Processing
- Processing → Completed
- Qualquer status → Cancelled (exceto Completed)

## Decisões Técnicas

### Result Pattern
Evita o uso de exceções para controle de fluxo de negócio, tornando os erros explícitos e facilitando o tratamento.

### Factory Methods
Centraliza validações na criação de entidades, garantindo que objetos inválidos nunca sejam criados.

### Mensageria Assíncrona
Desacopla a confirmação do pedido do fluxo principal, demonstrando arquitetura orientada a eventos.

### Validações no Domínio
Regras de negócio ficam encapsuladas nas entidades, não nos controllers ou handlers.

## Melhorias Futuras

- [ ] Testes de integração com Testcontainers
- [ ] Logging estruturado com Serilog
- [ ] Health checks para monitoramento
- [ ] API versioning
- [ ] Paginação na listagem de pedidos
- [ ] Filtros e ordenação
- [ ] Autenticação e autorização
- [ ] Validações avançadas com FluentValidation
- [ ] CI/CD com GitHub Actions

## Autor

Desenvolvido como desafio técnico para Hiper Software SA.
