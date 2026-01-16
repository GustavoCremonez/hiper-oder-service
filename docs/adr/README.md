# Architectural Decision Records (ADRs)

Este diretório contém os registros de decisões arquiteturais (ADRs) do Hiper Order System.

## O que são ADRs?

ADRs documentam decisões arquiteturais importantes tomadas durante o desenvolvimento do projeto, incluindo:
- Contexto da decisão
- Alternativas consideradas
- Decisão tomada
- Consequências (positivas e negativas)

## Lista de ADRs

- [ADR-0001](0001-clean-architecture.md) - Adoção de Clean Architecture
- [ADR-0002](0002-result-pattern.md) - Result Pattern para Tratamento de Erros
- [ADR-0003](0003-rabbitmq-mensageria.md) - RabbitMQ para Mensageria Assíncrona
- [ADR-0004](0004-entity-framework-core.md) - Entity Framework Core como ORM
- [ADR-0005](0005-vue3-typescript-frontend.md) - Vue 3 + TypeScript para Frontend

## Status dos ADRs

- **Proposto**: Decisão em discussão
- **Aceito**: Decisão aprovada e implementada
- **Depreciado**: Decisão substituída por outra
- **Superseded**: Substituída por ADR mais recente

## Como criar um novo ADR

1. Copie o template abaixo
2. Crie um arquivo com numeração sequencial: `XXXX-titulo-descritivo.md`
3. Preencha todas as seções
4. Adicione à lista acima

### Template

```markdown
# ADR XXXX: Título da Decisão

## Status
[Proposto | Aceito | Depreciado | Superseded]

## Contexto
Descreva o problema ou situação que motiva esta decisão.

## Decisão
Descreva a decisão tomada.

## Consequências

### Positivas
- Lista de consequências positivas

### Negativas
- Lista de consequências negativas

## Alternativas Consideradas
- Alternativa 1: Por que foi rejeitada
- Alternativa 2: Por que foi rejeitada

## Referências
- Links relevantes
```
