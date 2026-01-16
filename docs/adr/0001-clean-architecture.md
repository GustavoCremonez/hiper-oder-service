# ADR 0001: Adoção de Clean Architecture

## Status
Aceito

## Contexto
O sistema de gerenciamento de pedidos precisa de uma arquitetura que seja:
- Testável e manutenível a longo prazo
- Independente de frameworks e tecnologias específicas
- Escalável para crescimento futuro
- Clara na separação de responsabilidades

## Decisão
Adotar Clean Architecture com quatro camadas principais:

1. **Domain**: Entidades de negócio, enums e interfaces de repositório
2. **Application**: Casos de uso, DTOs e lógica de aplicação
3. **Infrastructure**: Implementações de persistência, mensageria e integrações externas
4. **API**: Controllers, middlewares e configuração da aplicação web

## Consequências

### Positivas
- Dependências sempre apontam para dentro (Domain não depende de nada)
- Fácil substituição de tecnologias de infraestrutura
- Testabilidade aumentada através de injeção de dependências
- Separação clara entre regras de negócio e detalhes técnicos
- Facilita trabalho em equipe com responsabilidades bem definidas

### Negativas
- Curva de aprendizado inicial para desenvolvedores não familiarizados
- Mais arquivos e estrutura de pastas comparado a abordagens mais simples
- Possível over-engineering para funcionalidades muito simples

## Alternativas Consideradas
- **MVC tradicional**: Rejeitada por misturar concerns e dificultar testes
- **Arquitetura em camadas simples**: Rejeitada por não fornecer isolamento suficiente
- **DDD completo**: Considerado complexo demais para o escopo atual

## Notas
Esta decisão alinha-se com as melhores práticas da comunidade .NET e facilita futuras migrações tecnológicas.
