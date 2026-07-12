# FCG.PaymentsAPI

Microsserviço de **pagamentos** da plataforma FIAP Cloud Games (FCG).

Consome o evento `OrderPlacedEvent` publicado pelo serviço de catálogo quando um usuário adquire um jogo, simula o processamento do pagamento, persiste o registro e publica o evento `PaymentProcessedEvent` para o serviço de notificações.

## Arquitetura

Clean Architecture / DDD em 4 camadas:

```
src/
├── FCG.PaymentsAPI.Domain          # Entidade Payment, enum PaymentStatus, interfaces
├── FCG.PaymentsAPI.Application     # Consumers, queries (MediatR), eventos
├── FCG.PaymentsAPI.Infrastructure  # EF Core, repositórios
└── FCG.PaymentsAPI.API             # Controllers, configuração, Swagger
```

- **.NET 8** / ASP.NET Core
- **EF Core 8** + SQL Server
- **MassTransit** + RabbitMQ (mensageria)
- **MediatR** (CQRS)
- **JWT Bearer** (valida tokens emitidos pelo FCG.UsersAPI)

## Endpoints

| Método | Rota | Autorização | Descrição |
|---|---|---|---|
| GET | `/api/payments` | Admin | Lista todos os pagamentos processados |

## Eventos

| Evento | Direção | Descrição |
|---|---|---|
| `OrderPlacedEvent` | Consome | Pedido de aquisição de jogo (fila `payments-order-placed`) |
| `PaymentProcessedEvent` | Publica | Resultado do pagamento (PaymentId, OrderId, UserId, GameId, Amount, Approved) |

O processamento é idempotente: pedidos já processados são ignorados (índice único por `OrderId`).

## Variáveis de ambiente

| Variável | Descrição | Exemplo |
|---|---|---|
| `ConnectionStrings__Default` | Connection string do SQL Server | `Server=localhost\SQLEXPRESS;Database=FCG_PaymentsDB;Trusted_Connection=True;TrustServerCertificate=True` |
| `Jwt__Secret` | Mesma chave do FCG.UsersAPI | — |
| `Jwt__Issuer` | `FCG.UsersAPI` | — |
| `Jwt__Audience` | `FCG` | — |
| `RabbitMQ__Host` | Host do RabbitMQ | `localhost` |
| `RabbitMQ__Username` | Usuário do RabbitMQ | `guest` |
| `RabbitMQ__Password` | Senha do RabbitMQ | `guest` |

## Como executar

### Local

Pré-requisitos: .NET 8 SDK, SQL Server, RabbitMQ.

```bash
dotnet run --project src/FCG.PaymentsAPI.API
```

As migrations são aplicadas automaticamente na inicialização.

### Docker

```bash
docker build -t fcg-paymentsapi .
docker run -p 5003:8080 \
  -e ConnectionStrings__Default="..." \
  -e Jwt__Secret="..." \
  -e RabbitMQ__Host="rabbitmq" \
  fcg-paymentsapi
```

### Kubernetes

```bash
kubectl apply -f k8s/
```

Os manifests incluem Deployment, Service, ConfigMap e Secret.

## Testes

Os testes unitários (xUnit + NSubstitute) estão na branch `feature/testes-unitarios`:

```bash
dotnet test
```
