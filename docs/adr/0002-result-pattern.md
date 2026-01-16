# ADR 0002: Result Pattern para Tratamento de Erros

## Status
Aceito

## Contexto
O tratamento de erros em aplicações pode ser feito de várias formas:
- Exceções para controle de fluxo
- Códigos de erro
- Objetos de resultado
- Unions types

Exceções são custosas em performance e podem tornar o fluxo de controle implícito e difícil de rastrear.

## Decisão
Implementar o Result Pattern com as classes:
- `Result`: Para operações que não retornam valor
- `Result<T>`: Para operações que retornam valor

```csharp
public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string Error { get; }
}

public class Result<T> : Result
{
    public T Value { get; }
}
```

## Consequências

### Positivas
- Erros explícitos no fluxo de código
- Força o desenvolvedor a lidar com casos de erro
- Melhor performance comparado a exceções
- Facilita testes unitários
- Railway-oriented programming possível
- Código mais previsível e legível

### Negativas
- Requer disciplina da equipe para usar consistentemente
- Pode aumentar verbosidade em alguns casos
- Não compatível com código que espera exceções

## Uso
Usado em:
- Métodos factory das entidades (`Order.Create`, `OrderItem.Create`)
- Casos de uso da camada Application
- Validações de domínio

Exceções ainda são usadas para:
- Erros técnicos irrecuperáveis (banco de dados indisponível)
- Violações de contrato (argumentos nulos)

## Alternativas Consideradas
- **Exceções para tudo**: Rejeitado por performance e fluxo implícito
- **OneOf/Union Types**: Rejeitado por falta de suporte nativo em C#
- **Códigos de erro**: Rejeitado por não serem type-safe

## Referências
- Railway Oriented Programming - Scott Wlaschin
- Domain Modeling Made Functional
