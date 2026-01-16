# ADR 0005: Vue 3 + TypeScript para Frontend

## Status
Aceito

## Contexto
O frontend precisa de um framework moderno que:
- Seja reativo e performático
- Tenha forte tipagem para reduzir bugs
- Possua boa curva de aprendizado
- Tenha ecosistema maduro

## Decisão
Utilizar Vue 3 com Composition API e TypeScript.

Stack completa:
- Vue 3 (Composition API)
- TypeScript 5
- Vite 7 (build tool)
- Vue Router 4
- Tailwind CSS 4
- Axios para HTTP

## Arquitetura Frontend
Estrutura baseada em features:
```
src/
├── features/
│   └── orders/
│       ├── components/
│       ├── views/
│       ├── composables/
│       ├── services/
│       └── types.ts
└── shared/
    └── components/
```

## Consequências

### Positivas
- Composition API oferece melhor reusabilidade de lógica
- TypeScript previne bugs em tempo de desenvolvimento
- Vite oferece HMR instantâneo
- Feature-based structure facilita escalabilidade
- Tailwind CSS acelera desenvolvimento de UI
- Ecossistema Vue maduro e bem documentado

### Negativas
- Curva de aprendizado de Composition API
- TypeScript adiciona verbosidade inicial
- Necessidade de configuração adicional

## Padrões Adotados
- Composables para lógica reutilizável (`useOrders`)
- Services para chamadas de API
- Types centralizados por feature
- Props tipadas com interfaces
- Reactive refs para estado local

## Alternativas Consideradas
- **React**: Rejeitado por preferência de sintaxe SFC e menor boilerplate
- **Angular**: Rejeitado por complexidade e opinião excessiva
- **Svelte**: Rejeitado por ecossistema menor
- **JavaScript puro**: Rejeitado por falta de type safety

## Melhorias Futuras
- Implementar testes com Vitest
- Adicionar Pinia para estado global se necessário
- Implementar lazy loading de rotas
- PWA capabilities
