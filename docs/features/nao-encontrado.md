# Feature: Página Não Encontrada (404)

- **Rota**: `/not-found` (também capturada por `*` quando configurada no
  `App.razor`).
- **Componente**: `TaxPlanner.Wasm/Pages/NotFound.razor`
- **Status**: Estável (placeholder textual — recomenda-se evoluir)

## Propósito

Exibir uma página amigável quando o usuário acessa uma rota inexistente.
No backend, o pipeline do `TaxPlanner.Server` configura
`app.UseStatusCodePagesWithReExecute("/not-found", ...)` para reexecutar
a rota `/not-found` em respostas com status code (ex.: 404).

## Componente

- `Pages/NotFound.razor`:
  - `@page "/not-found"`
  - `@layout MainLayout` — usa o layout raiz (AppBar + drawer + footer).
  - Markup: `<h3>Not Found</h3>` + parágrafo curto.

> ⚠️ O conteúdo atual está em **inglês** ("Not Found", "Sorry, the
> content you are looking for does not exist."). Recomenda-se
> localizar para **pt-BR** ("Página não encontrada", "O conteúdo
> procurado não existe.") em iteração futura, mantendo consistência
> com o idioma do projeto (ver `CLAUDE.md`).

## Pontos de entrada

- Acesso direto: `/not-found`.
- Reexecução automática pelo ASP.NET Core via
  `UseStatusCodePagesWithReExecute` em
  `TaxPlanner.Server/Program.cs`.
- Rota curinga `*` (recomendado adicionar no `App.razor` do Wasm para
  cobrir SPA routes não encontradas).

## Estado

- Sem `@code`. Componente puramente declarativo.

## Acessibilidade

- Hierarquia semântica: `<h3>` para o título.
- Texto alternativo: descrição no `<p>`.
- Recomenda-se adicionar link para `/` ("Voltar ao início") em evolução.

## Referências

- `TaxPlanner.Wasm/Pages/NotFound.razor`
- `TaxPlanner.Server/Program.cs` (`UseStatusCodePagesWithReExecute`)
- `TaxPlanner.Wasm/App.razor` (roteador principal)
