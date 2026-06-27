# ADR 0003 — Render de Markdown no servidor (Markdig) com cache em `ContentHtml`

- **Data**: 2026-06-10
- **Status**: Aceito
- **Contexto**: posts do blog são escritos em Markdown, mas precisam
  ser entregues ao frontend já renderizados em HTML para evitar
  custo de parsing no cliente e permitir highlighting/sanitização
  controlada no servidor.
- **Decisão**:
  - Usar a biblioteca **`Markdig` 1.2** com
    `MarkdownPipelineBuilder().UseAdvancedExtensions().Build()`.
  - Render acontece **no servidor**, no momento de `CreateAsync` /
    `UpdateAsync` do `BlogService`.
  - O resultado é persistido em **`Post.ContentHtml`** como cache, e
    o Markdown original fica em `Post.ContentMarkdown` (fonte de
    verdade).
- **Consequências**:
  - **Positivas**: payload de leitura (público) é leve (HTML pronto);
    sem dependência de JS de Markdown no cliente; re-render sob
    demanda é barato.
  - **Negativas**: a coluna `ContentHtml` precisa ser atualizada
    sempre que o Markdown muda; aumento de uso de disco
    (Markdown + HTML por post).
- **Alternativas consideradas**:
  - **Render no cliente** — simplificaria o backend, mas aumentaria
    o bundle do Wasm e quebraria o requisito de privacidade
    (cliente precisa de mais JS).
  - **Sem cache de HTML, re-render a cada leitura** — desperdiça CPU
    em posts populares.
- **Referências**:
  - `TaxPlanner.Server/Services/BlogService.cs`
    (`RenderMarkdown`, `CreateAsync`, `UpdateAsync`)
  - `TaxPlanner.Server/Models/Post.cs` (`ContentMarkdown`, `ContentHtml`)
  - [`../architecture.md`](../architecture.md)
