# ADR 0004: Entity Framework Core como ORM

## Status
Aceito

## Contexto
O sistema precisa de uma solução para persistência de dados que:
- Facilite mapeamento objeto-relacional
- Suporte PostgreSQL
- Permita migrations versionadas
- Seja bem suportada no ecossistema .NET

## Decisão
Utilizar Entity Framework Core 8.0 como ORM principal.

Configurações:
- Code First com Migrations
- Fluent API para configuração de entidades
- Repository Pattern como abstração
- DbContext configurado via Dependency Injection

## Consequências

### Positivas
- Produtividade no desenvolvimento
- Migrations automáticas e versionadas
- LINQ para queries type-safe
- Suporte robusto da Microsoft
- Change tracking automático
- Facilita testes com InMemory provider
- Integração natural com .NET

### Negativas
- Performance inferior a SQL puro em queries complexas
- Possível N+1 queries se não usado corretamente
- Curva de aprendizado para otimizações
- Overhead de memória com tracking

## Boas Práticas Adotadas
- Repository Pattern para abstração
- AsNoTracking para queries read-only
- Eager loading com Include quando necessário
- Migrations sempre versionadas no controle de versão

## Alternativas Consideradas
- **Dapper**: Rejeitado por requerer mais SQL manual
- **ADO.NET puro**: Rejeitado por produtividade baixa
- **NHibernate**: Rejeitado por comunidade menor e complexidade

## Melhorias Futuras
- Implementar especificação pattern para queries complexas
- Adicionar interceptors para logging de queries
- Implementar Unit of Work explícito se necessário
- Query splitting para evitar cartesian explosion
