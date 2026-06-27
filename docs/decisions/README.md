# Decisões Arquiteturais (ADR) — TaxPlanner

Esta seção reúne os **Architecture Decision Records (ADRs)** do projeto. Cada
ADR documenta uma decisão arquitetural relevante, o contexto que a motivou e
suas consequências.

## Índice de ADRs

| ID  | Data       | Título                                                                    | Status      |
| --- | ---------- | ------------------------------------------------------------------------- | ----------- |
| 0001 | 2026-06-10 | Stack Blazor WebAssembly + ASP.NET Core para frontend e backend          | Aceita      |
| 0002 | 2026-06-10 | PostgreSQL como banco de dados (via Npgsql + EF Core)                    | Aceita      |
| 0003 | 2026-06-10 | Render de Markdown no servidor (Markdig) com cache em `ContentHtml`       | Aceita      |
| 0004 | 2026-06-10 | Uploads de imagem em disco (`wwwroot/uploads/posts/`)                     | Aceita      |

## Modelo de ADR

Cada ADR usa o esqueleto abaixo (cabeçalhos estáveis para permitir
indexação):

```md
# ADR NNNN — Título curto

- **Data**: AAAA-MM-DD
- **Status**: Proposto | Aceito | Substituído por ADR-NNNN | Deprecado
- **Contexto**: por que a decisão foi necessária.
- **Decisão**: o que foi decidido.
- **Consequências**: impactos positivos e negativos.
- **Alternativas consideradas**: opções descartadas e por quê.
- **Referências**: código, issues, documentação relacionada.
```

## Como propor um novo ADR

1. Copiar o esqueleto acima para
   `docs/decisions/AAAA-MM-DD-<slug-curto>.md`.
2. Numerar sequencialmente (procurar o maior ID existente + 1).
3. Adicionar linha na tabela de índice.
4. Abrir PR com a proposta para revisão.
