# ADR 0003: RabbitMQ para Mensageria Assíncrona

## Status
Aceito

## Contexto
O sistema precisa processar eventos de forma assíncrona, especificamente:
- Confirmação automática de pedidos após criação
- Possível integração futura com outros sistemas
- Desacoplamento de operações que não precisam ser síncronas

## Decisão
Utilizar RabbitMQ como message broker para comunicação assíncrona entre componentes.

Fluxo implementado:
1. Pedido criado com status `Pending`
2. Evento `OrderCreatedEvent` publicado no RabbitMQ
3. Consumer recebe evento e atualiza status para `Confirmed`

## Consequências

### Positivas
- Desacoplamento entre produtor e consumidor de eventos
- Resiliência através de filas persistentes
- Possibilidade de retry automático
- Escalabilidade horizontal dos consumidores
- Audit trail através das mensagens
- Preparado para arquitetura de microsserviços futura

### Negativas
- Complexidade adicional de infraestrutura
- Necessidade de monitoramento da fila
- Eventual consistency no lugar de consistência imediata
- Mais componentes para gerenciar (RabbitMQ server)

## Configuração
- Exchange: Direct
- Queue: `order.created`
- Durabilidade: Habilitada
- Auto-delete: Desabilitada
- Consumer: Background service com delay de 15s para aguardar RabbitMQ subir

## Alternativas Consideradas
- **Azure Service Bus**: Rejeitado por custo e lock-in com Azure
- **Apache Kafka**: Rejeitado por ser over-engineering para o volume atual
- **Redis Pub/Sub**: Rejeitado por não ter garantias de entrega
- **Processamento síncrono**: Rejeitado por acoplamento e falta de escalabilidade

## Melhorias Futuras
- Dead letter queue para mensagens com falha
- Retry policy configurável
- Múltiplos consumers para paralelização
- Telemetria e observabilidade das mensagens
