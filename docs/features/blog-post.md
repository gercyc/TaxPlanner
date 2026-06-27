# Feature: Blog (detalhe do post)

- **Rota**: `/post/{Slug}`
- **Componente**: `TaxPlanner.Wasm/Pages/Post.razor`
- **Status**: Estável

## Propósito

Exibe um **post individual** (HTML renderizado a partir de Markdown no
servidor). Inclui título, data de publicação formatada em pt-BR, tags
e o conteúdo HTML.

## Pontos de entrada

- Navegação interna: links a partir de [`blog-listagem.md`](./blog-listagem.md).
- Link direto via URL (compartilhamento).
- Feed RSS em `/api/posts/rss` aponta para a mesma URL.

## Componente e dependências

- `Pages/Post.razor`:
  - `[Parameter] string Slug` — slug do post (vindo da rota).
  - `inject BlogService BlogService` — busca o detalhe.
  - `inject IJSRuntime JS` — invoca `Prism.highlightAll` para
    syntax highlighting de blocos de código após o render.
- `Services/BlogService.cs` — método `GetPostAsync(slug)`.
- Tipo: `BlogService.PostDetail { Slug, Title, Summary, ContentHtml,
  ContentMarkdown, Tags, PublishedAt, UpdatedAt, ThumbnailUrl,
  ThumbnailAlt }`.

## Estado

```csharp
private BlogService.PostDetail? _post;
private bool _loading = true;
private bool _highlighted;
```

- `OnInitializedAsync` busca o post e marca `_loading = false`.
- `OnAfterRenderAsync(firstRender)`: quando o post já carregou e ainda
  não foi destacado, chama `Prism.highlightAll` e marca `_highlighted`
  para evitar chamadas repetidas.

## Integrações

- `GET /api/posts/{slug}` no `TaxPlanner.Server` (público, sem auth).
- **404 handling**: quando o slug não existe, o backend retorna
  `Not Found`; o `BlogService` propaga a falha e a UI exibe
  `MudAlert Severity.Error` + botão "Voltar ao Blog".

## Renderização

- **Hero de capa**: quando `_post.ThumbnailUrl` é não-vazio, o componente
  renderiza um `<figure class="tp-article-hero">` com `<img>` antes do
  conteúdo. Quando ausente, apenas o título + metadados aparecem (sem
  bloco vazio). O atributo `alt` usa `ThumbnailAlt` quando presente ou
  cai no fallback `"Imagem de capa: {title}"`. O `loading="lazy"`
  mantém o custo de fetch fora da primeira pintura.
- `ContentHtml` é injetado via `@((MarkupString)_post.ContentHtml)`.
  ⚠️ Por confiar no HTML gerado pelo `Markdig` no servidor, certifique-se
  de que o conteúdo do post é confiável (apenas autores autenticados
  publicam). Caso abra para autores externos, sanitizar o HTML antes
  de persistir.
- Blocos de código são destacados via Prism.js (carregado em
  `wwwroot/index.html`).

## UX

- **Loading**: `MudProgressCircular Indeterminate`.
- **Erro**: `MudAlert Severity.Error` + CTA "Voltar ao Blog".
- **Data**: formato `"dd 'de' MMMM 'de' yyyy"` em pt-BR.
- **Tags**: chips outlined (mesmo padrão da listagem).

## Acessibilidade

- Hierarquia semântica: `<h3>` no título.
- `MudDivider` separa metadados do conteúdo.
- `MudButton` com `StartIcon` no "Voltar".

## Referências

- `TaxPlanner.Wasm/Pages/Post.razor`
- `TaxPlanner.Wasm/Services/BlogService.cs`
- `TaxPlanner.Server/Controllers/PostsController.cs`
- `TaxPlanner.Server/Services/BlogService.cs` (server-side)
