<div align="center">
  
# ?? Cinema Ticket System

### Sistema Distribuído de Venda de Ingressos

[![.NET 8.0](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-12.0-239120?logo=c-sharp)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![Docker](https://img.shields.io/badge/Docker-Enabled-2496ED?logo=docker)](https://www.docker.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

*Sistema de alta concorrência para reserva e venda de ingressos de cinema com arquitetura distribuída*

[Visão Geral](#-visão-geral) •
[Tecnologias](#-tecnologias) •
[Arquitetura](#-arquitetura) •
[Como Executar](#-como-executar) •
[API Endpoints](#-api-endpoints) •
[Roadmap](#-roadmap)

</div>

---

## ?? Sobre o Projeto

Este projeto foi desenvolvido como solução para um desafio técnico de sistemas distribuídos, focado em resolver problemas reais de **alta concorrência** em ambientes de produção.

### ?? Desafio

Desenvolver um sistema robusto de venda de ingressos para redes de cinema, capaz de lidar com **milhares de requisições simultâneas** garantindo consistência e integridade dos dados.

#### ?? Cenário Crítico

```
?? Situação:
   +- 1 Sala de Cinema com 2 assentos disponíveis
   +- 10 Usuários tentando comprar simultaneamente
   +- Múltiplas instâncias da aplicação (distribuída)
   +- Reservas temporárias com expiração (30s)
   +- Zero margem para double-booking
```

#### ? Problemas a Resolver

| Desafio | Descrição |
|---------|-----------|
| **Race Condition** | 2+ usuários clicando no último assento no mesmo milissegundo |
| **Deadlock** | Múltiplos usuários reservando assentos conflitantes |
| **Idempotência** | Cliente reenvia requisição por timeout de rede |
| **Expiração Automática** | Liberar assentos de reservas não confirmadas |
| **Coordenação Distribuída** | Sincronizar estado entre múltiplas instâncias |

---

## ?? Tecnologias

### Core Stack

| Tecnologia | Versão | Finalidade |
|------------|--------|------------|
| **C# / .NET** | 8.0 | Backend API & Business Logic |
| **PostgreSQL** | 15+ | Banco de dados relacional principal |
| **Redis** | 7.0+ | Cache distribuído & Locks |
| **RabbitMQ** | 3.12+ | Sistema de mensageria assíncrona |
| **Docker** | 24+ | Containerização & Orquestração |

### Bibliotecas & Frameworks

- **Entity Framework Core** - ORM para acesso a dados
- **MassTransit** - Abstração para mensageria
- **Serilog** - Logging estruturado
- **Swagger/OpenAPI** - Documentação da API
- **xUnit** - Framework de testes

### Justificativa das Escolhas

<details>
<summary><b>?? Por que PostgreSQL?</b></summary>

- Suporte nativo a transações ACID
- Isolamento de transações configurável
- Row-level locking para controle fino de concorrência
- Extensões como `pg_locks` para diagnóstico
</details>

<details>
<summary><b>?? Por que Redis?</b></summary>

- Latência ultra-baixa (< 1ms)
- Distributed Locks com Redlock
- TTL automático para expiração de reservas
- Pub/Sub para eventos em tempo real
</details>

<details>
<summary><b>?? Por que RabbitMQ?</b></summary>

- Garantias de entrega (at-least-once)
- Dead Letter Queues nativas
- Padrões Exchange/Queue flexíveis
- Retry com backoff exponencial
</details>

---

## ?? Arquitetura

### Clean Architecture

```
+-----------------------------------------------------+
¦                    API Layer                        ¦
¦  +--------------+  +--------------+                ¦
¦  ¦ Controllers  ¦  ¦  Middlewares ¦                ¦
¦  +--------------+  +--------------+                ¦
+-----------------------------------------------------+
                         ¦
+-----------------------------------------------------+
¦               Application Layer                     ¦
¦  +--------------+  +--------------+                ¦
¦  ¦  Use Cases   ¦  ¦  DTOs/Models ¦                ¦
¦  +--------------+  +--------------+                ¦
+-----------------------------------------------------+
                         ¦
+-----------------------------------------------------+
¦                 Domain Layer                        ¦
¦  +--------------+  +--------------+                ¦
¦  ¦   Entities   ¦  ¦  Interfaces  ¦                ¦
¦  +--------------+  +--------------+                ¦
+-----------------------------------------------------+
                         ¦
+-----------------------------------------------------+
¦            Infrastructure Layer                     ¦
¦  +--------+  +--------+  +--------+                ¦
¦  ¦Database¦  ¦ Redis  ¦  ¦RabbitMQ¦                ¦
¦  +--------+  +--------+  +--------+                ¦
+-----------------------------------------------------+
```

### Princípios Aplicados

- ? **SOLID** - Separação de responsabilidades
- ? **DDD** - Domain-Driven Design
- ? **Repository Pattern** - Abstração de persistência
- ? **CQRS** - Separação de comandos e consultas
- ? **Event-Driven** - Comunicação assíncrona via eventos

---

## ?? Como Executar

### Pré-requisitos

```bash
# Ferramentas necessárias
? Docker Desktop 24.0+
? Docker Compose 2.0+
? .NET SDK 8.0+ (opcional, para desenvolvimento)
? Git 2.0+
```

### Instalação & Execução

```bash
# 1. Clone o repositório
git clone https://github.com/developerviana/Cinena-Ticket.git
cd Cinena-Ticket

# 2. Inicie todos os serviços (1 comando!)
docker-compose up -d

# 3. Aguarde inicialização (~30s)
# Verificar saúde dos containers:
docker-compose ps

# 4. Acesse a API
# Swagger UI: http://localhost:5000/swagger
# API Base:   http://localhost:5000/api
```

### Serviços Disponíveis

| Serviço | Porta | Credenciais |
|---------|-------|-------------|
| **API** | 5000 | - |
| **PostgreSQL** | 5432 | `postgres:postgres` |
| **Redis** | 6379 | - |
| **RabbitMQ Admin** | 15672 | `guest:guest` |

### Popular Dados Iniciais

```bash
# Executar seed de dados
docker-compose exec api dotnet run --seed

# Ou via script SQL
docker-compose exec postgres psql -U postgres -d cinema -f /scripts/seed.sql
```

---

## ?? API Endpoints


### ?? Gestão de Sessões

<details>
<summary><b>POST</b> <code>/api/sessions</code> - Criar Sessão</summary>

```json
// Request
{
  "movieTitle": "Oppenheimer",
  "startTime": "2024-02-15T19:00:00Z",
  "roomNumber": "A1",
  "totalSeats": 16,
  "ticketPrice": 25.00
}

// Response 201 Created
{
  "sessionId": "550e8400-e29b-41d4-a716-446655440000",
  "movieTitle": "Oppenheimer",
  "availableSeats": 16,
  "createdAt": "2024-02-10T10:30:00Z"
}
```
</details>

<details>
<summary><b>GET</b> <code>/api/sessions/{id}/seats</code> - Disponibilidade em Tempo Real</summary>

```json
// Response 200 OK
{
  "sessionId": "550e8400-e29b-41d4-a716-446655440000",
  "totalSeats": 16,
  "availableSeats": 8,
  "seats": [
    { "number": "A1", "status": "available" },
    { "number": "A2", "status": "reserved", "expiresAt": "2024-02-15T19:00:30Z" },
    { "number": "A3", "status": "sold" }
  ]
}
```
</details>

### ?? Reserva de Assentos

<details>
<summary><b>POST</b> <code>/api/reservations</code> - Criar Reserva (30s TTL)</summary>

```json
// Request
{
  "sessionId": "550e8400-e29b-41d4-a716-446655440000",
  "userId": "user-123",
  "seatNumbers": ["A1", "A2"]
}

// Response 201 Created
{
  "reservationId": "res-789",
  "status": "pending",
  "expiresAt": "2024-02-15T19:00:30Z",
  "seats": ["A1", "A2"],
  "totalAmount": 50.00
}

// Response 409 Conflict (assento já reservado)
{
  "error": "SEAT_ALREADY_RESERVED",
  "message": "Assentos [A1] não disponíveis",
  "conflictingSeats": ["A1"]
}
```
</details>

### ?? Confirmação de Pagamento

<details>
<summary><b>POST</b> <code>/api/reservations/{id}/confirm</code> - Confirmar Pagamento</summary>

```json
// Request
{
  "paymentMethod": "credit_card",
  "transactionId": "tx-456"
}

// Response 200 OK
{
  "saleId": "sale-999",
  "status": "confirmed",
  "seats": ["A1", "A2"],
  "paidAt": "2024-02-15T19:00:25Z"
}

// Response 410 Gone (reserva expirada)
{
  "error": "RESERVATION_EXPIRED",
  "message": "Reserva expirou em 2024-02-15T19:00:30Z"
}
```
</details>

### ?? Consultas

<details>
<summary><b>GET</b> <code>/api/users/{userId}/purchases</code> - Histórico de Compras</summary>

```json
// Response 200 OK
{
  "userId": "user-123",
  "purchases": [
    {
      "saleId": "sale-999",
      "sessionId": "550e8400-e29b-41d4-a716-446655440000",
      "movieTitle": "Oppenheimer",
      "seats": ["A1", "A2"],
      "totalAmount": 50.00,
      "purchasedAt": "2024-02-15T19:00:25Z"
    }
  ]
}
```
</details>

---

## ?? Estratégias de Concorrência Implementadas

### 1?? Controle de Race Conditions

**Problema:** 2 usuários clicam no último assento no mesmo milissegundo.

**Solução:**
```csharp
// Distributed Lock com Redis (Redlock)
using var @lock = await _redisLock.AcquireAsync($"seat:{seatId}", TimeSpan.FromSeconds(5));
if (@lock == null) 
    throw new ConflictException("Assento em processo de reserva");

// Dentro do lock: verificar + reservar atomicamente
var seat = await _dbContext.Seats
    .Where(s => s.Id == seatId && s.Status == SeatStatus.Available)
    .FirstOrDefaultAsync();

if (seat == null)
    throw new ConflictException("Assento indisponível");

seat.Status = SeatStatus.Reserved;
await _dbContext.SaveChangesAsync();
```

**Resultado:** ? Apenas 1 requisição consegue o lock, outras aguardam ou falham gracefully.

---

### 2?? Prevenção de Deadlocks

**Problema:** User A reserva [1,3], User B reserva [3,1] - ambos aguardam liberação.

**Solução:**
```csharp
// Sempre ordenar IDs para lock consistente
var sortedSeatIds = seatIds.OrderBy(id => id).ToList();

foreach (var seatId in sortedSeatIds)
{
    using var @lock = await _redisLock.AcquireAsync($"seat:{seatId}");
    // Processar sequencialmente em ordem crescente
}
```

**Resultado:** ? Ordem determinística evita ciclos de espera.

---

### 3?? Idempotência

**Problema:** Cliente reenvia requisição por timeout.

**Solução:**
```csharp
// Idempotency Key via header
[HttpPost]
public async Task<IActionResult> CreateReservation(
    [FromBody] ReservationRequest request,
    [FromHeader(Name = "Idempotency-Key")] string idempotencyKey)
{
    // Verificar cache Redis
    var cached = await _redis.GetAsync<Reservation>($"idempotency:{idempotencyKey}");
    if (cached != null)
        return Ok(cached); // Retorna resultado anterior

    // Processar normalmente e cachear resultado
    var reservation = await _service.CreateReservation(request);
    await _redis.SetAsync($"idempotency:{idempotencyKey}", reservation, TimeSpan.FromMinutes(5));
    return CreatedAtAction(nameof(GetReservation), new { id = reservation.Id }, reservation);
}
```

**Resultado:** ? Requisições duplicadas retornam o mesmo resultado sem processar novamente.

---

### 4?? Expiração Automática de Reservas

**Solução 1: Redis TTL**
```csharp
// Ao criar reserva
await _redis.SetAsync($"reservation:{reservationId}", reservation, TimeSpan.FromSeconds(30));
```

**Solução 2: Background Worker**
```csharp
public class ReservationExpirationWorker : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var expiredReservations = await _dbContext.Reservations
                .Where(r => r.Status == ReservationStatus.Pending 
                         && r.ExpiresAt < DateTime.UtcNow)
                .ToListAsync();

            foreach (var reservation in expiredReservations)
            {
                reservation.Status = ReservationStatus.Expired;
                await _eventBus.Publish(new ReservationExpiredEvent(reservation.Id));
            }

            await _dbContext.SaveChangesAsync();
            await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
        }
    }
}
```

---

### 5?? Coordenação entre Múltiplas Instâncias

**Desafio:** 3 instâncias da API rodando simultaneamente.

**Solução:**
```yaml
# docker-compose.yml
services:
  api:
    image: cinema-ticket-api
    deploy:
      replicas: 3  # Múltiplas instâncias
    environment:
      - REDIS_CONNECTION=redis:6379
      - USE_DISTRIBUTED_LOCK=true
```

```csharp
// Todas as instâncias compartilham:
// 1. Mesmo banco de dados PostgreSQL (estado central)
// 2. Mesmo Redis (cache + locks distribuídos)
// 3. Mesma fila RabbitMQ (processamento assíncrono)
```

---

## ?? Sistema de Mensageria (Event-Driven)

### Eventos Publicados

| Evento | Quando | Consumidores |
|--------|--------|--------------|
| `ReservationCreated` | Assento reservado | Email Worker, Analytics |
| `PaymentConfirmed` | Pagamento aprovado | Email Worker, Invoice Generator |
| `ReservationExpired` | Reserva expira (30s) | Seat Releaser, Notification |
| `SeatReleased` | Assento liberado | Cache Invalidator |

### Exemplo de Publicação

```csharp
public class ReservationService
{
    private readonly IEventBus _eventBus;

    public async Task<Reservation> CreateReservationAsync(CreateReservationCommand command)
    {
        // ... lógica de reserva ...

        // Publicar evento
        await _eventBus.PublishAsync(new ReservationCreatedEvent
        {
            ReservationId = reservation.Id,
            UserId = command.UserId,
            Seats = command.SeatNumbers,
            ExpiresAt = DateTime.UtcNow.AddSeconds(30)
        });

        return reservation;
    }
}
```

### Retry com Backoff Exponencial

```csharp
services.AddMassTransit(x =>
{
    x.AddConsumer<ReservationCreatedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.UseMessageRetry(r => r.Exponential(
            retryLimit: 5,
            minInterval: TimeSpan.FromSeconds(1),
            maxInterval: TimeSpan.FromMinutes(5),
            intervalDelta: TimeSpan.FromSeconds(2)
        ));

        // Dead Letter Queue
        cfg.ReceiveEndpoint("reservation-created", e =>
        {
            e.ConfigureConsumer<ReservationCreatedConsumer>(context);
            e.UseMessageRetry(r => r.Intervals(100, 500, 1000));
            
            // Mensagens que falharam 5x vão para DLQ
            e.BindDeadLetterQueue("reservation-created-error");
        });
    });
});
```

---

## ?? Logging Estruturado

```csharp
// Serilog configurado com contexto enriquecido
Log.Information("Reserva criada {ReservationId} para usuário {UserId} - Assentos: {@Seats}",
    reservation.Id,
    userId,
    seatNumbers);

// Output estruturado (JSON):
{
  "timestamp": "2024-02-15T19:00:00.123Z",
  "level": "Information",
  "messageTemplate": "Reserva criada {ReservationId} para usuário {UserId}",
  "properties": {
    "ReservationId": "res-789",
    "UserId": "user-123",
    "Seats": ["A1", "A2"],
    "SourceContext": "CinemaTicket.Application.ReservationService"
  }
}
```

---

## ?? Testes

### Executar Testes

```bash
# Testes de unidade
dotnet test --filter Category=Unit

# Testes de integração
dotnet test --filter Category=Integration

# Testes de concorrência
dotnet test --filter Category=Concurrency

# Cobertura de código
dotnet test /p:CollectCoverage=true /p:CoverageReporter=html
```

### Teste de Concorrência (Exemplo)

```csharp
[Fact]
public async Task Should_Handle_Race_Condition_For_Last_Seat()
{
    // Arrange
    var sessionId = await CreateSessionWithSeats(totalSeats: 1);
    var tasks = new List<Task<HttpResponseMessage>>();

    // Act: 10 usuários tentam reservar o último assento
    for (int i = 0; i < 10; i++)
    {
        var userId = $"user-{i}";
        tasks.Add(_client.PostAsJsonAsync("/api/reservations", new
        {
            sessionId,
            userId,
            seatNumbers = new[] { "A1" }
        }));
    }

    var responses = await Task.WhenAll(tasks);

    // Assert
    var successCount = responses.Count(r => r.StatusCode == HttpStatusCode.Created);
    var conflictCount = responses.Count(r => r.StatusCode == HttpStatusCode.Conflict);

    Assert.Equal(1, successCount);  // Apenas 1 sucesso
    Assert.Equal(9, conflictCount); // 9 conflitos
}
```

---

## ?? Decisões Técnicas

### Transações Database vs Cache

**Decisão:** Redis para locks, PostgreSQL para estado persistente.

**Motivo:**
- Redis: Latência < 1ms para locks rápidos
- PostgreSQL: ACID para garantias de consistência
- **Padrão:** Lock no Redis ? Operação no DB ? Libera Lock

### Pessimistic vs Optimistic Locking

**Decisão:** **Pessimistic Locking** via Redis Distributed Lock.

**Motivo:**
- Alta contenção (muitos usuários, poucos assentos)
- Optimistic falharia frequentemente (overhead de retries)
- Feedback imediato ao usuário ("assento sendo processado")

### Message Broker: RabbitMQ vs Kafka

**Decisão:** RabbitMQ

**Motivo:**
| Critério | RabbitMQ | Kafka |
|----------|----------|-------|
| Latência | Baixa (~ms) | Média (~10ms) |
| Complexidade | Simples | Alta |
| Garantias | At-least-once | Exactly-once (complexo) |
| Use Case | Processamento de eventos | Stream processing |

Para este cenário (eventos transacionais, baixo volume), RabbitMQ é suficiente.

---

## ?? Limitações Conhecidas

1. **Escalabilidade Horizontal:** Redis como single point of failure
   - **Solução Futura:** Redis Cluster ou Redis Sentinel

2. **Sem Saga Pattern:** Rollback manual em caso de falha de pagamento
   - **Solução Futura:** Implementar Choreography Saga

3. **Rate Limiting:** Não implementado
   - **Solução Futura:** AspNetCoreRateLimit ou NGINX

4. **Observabilidade:** Logs locais, sem agregação
   - **Solução Futura:** ELK Stack ou Seq

---

## ?? Melhorias Futuras

### Curto Prazo (1-2 sprints)

- [ ] **Testes de Carga:** Simular 10.000 req/s com k6
- [ ] **Circuit Breaker:** Polly para resiliência
- [ ] **Health Checks:** Endpoints `/health` e `/ready`
- [ ] **Observabilidade:** OpenTelemetry + Jaeger

### Médio Prazo (3-6 meses)

- [ ] **CQRS Completo:** Separar read/write models
- [ ] **Event Sourcing:** Auditoria completa de eventos
- [ ] **GraphQL:** Queries flexíveis para frontend
- [ ] **WebSockets:** Atualização de disponibilidade em tempo real

### Longo Prazo (6-12 meses)

- [ ] **Kubernetes:** Deploy em cluster K8s
- [ ] **Multi-Region:** Replicação geográfica
- [ ] **Machine Learning:** Predição de demanda
- [ ] **Blockchain:** Ingressos NFT (experimentação)

---

## ?? Referências & Estudos

- [Designing Data-Intensive Applications - Martin Kleppmann](https://dataintensive.net/)
- [Building Microservices - Sam Newman](https://samnewman.io/books/building_microservices/)
- [Redis Distributed Locks (Redlock)](https://redis.io/docs/manual/patterns/distributed-locks/)
- [MassTransit Documentation](https://masstransit-project.com/)

---

## ?? Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

---

## ????? Autor

**Desenvolvido por:** [developerviana](https://github.com/developerviana)

?? **Contato:** flood.com@hotmail.com

?? **LinkedIn:** [linkedin.com/in/developerviana](https://linkedin.com/in/developerviana)

---

<div align="center">

### ? Se este projeto foi útil, considere dar uma estrela!

**Qualidade > Quantidade** • **Clean Code** • **SOLID** • **DDD**

</div>
