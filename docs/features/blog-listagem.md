# Feature: Blog (listagem)

- **Rota**: `/blog`
- **Componente**: `TaxPlanner.Wasm/Pages/Blog.razor`
- **Status**: Estável

## Propósito

Lista os **posts publicados** do blog, ordenados por data de publicação
decrescente. Cada card mostra título, summary, tags (chips) e data;
clicar no título ou no botão "Ler" navega para a página de detalhe
(`/post/{slug}`).

## Pontos de entrada

- AppBar (desktop e drawer mobile): `Href="/blog"`.
- Home: card "Artigos" no footer.
- Cards de cada postagem com `Href="/post/{slug}"`.

## Componente e dependências

- `Pages/Blog.razor`:
  - `inject BlogService BlogService` — cliente HTTP para a API pública.
- `Services/BlogService.cs` — wrapper tipado sobre `HttpClient`.
- `Components/BlogThumbnail.razor` — renderiza a imagem de capa do post
  nas variants `featured` (primeiro card da página) e `card` (demais).
  Quando `ThumbnailUrl` é `null` ou vazio, exibe o placeholder editorial
  com ícone de imagem e marca "TaxPlanner".
- Tipos: `BlogService.PostListResponse { Posts, Page, PageSize, TotalCount }`
  com `BlogService.PostSummary { Slug, Title, Summary, Tags, PublishedAt,
  UpdatedAt, ThumbnailUrl, ThumbnailAlt }`.

## Estado

```csharp
private BlogService.PostListResponse? _posts;
private int _currentPage = 1;
private int _pageSize = 9;
```

- `OnInitializedAsync` carrega a primeira página.
- `OnPageChanged(int page)` recarrega ao paginar, com **guard contra
  recarga duplicada** (`if (page == _currentPage) return;`).
- Após cada carga, faz **clamp** da página atual caso o total de páginas
  tenha diminuído (evita ficar em página inexistente após exclusão de
  posts).

## Integrações

- `GET /api/posts?page={n}&pageSize={9}` no `TaxPlanner.Server`.
- Sem autenticação.
- Paginação server-side: `_posts.TotalCount` é usado para calcular
  `Count` do `MudPagination` via fórmula
  `(TotalCount + PageSize - 1) / PageSize` (ceil).

## UX

- **Loading**: `MudProgressCircular Indeterminate` enquanto `_posts` é
  `null`.
- **Empty state**: `MudAlert Severity.Info` quando não há posts.
- **Lista**: `MudGrid` com `MudItem xs=12 md=6 lg=4` (1, 2 ou 3 colunas).
- **Capa**: o `BlogThumbnail` (variants `featured` e `card`) exibe a
  imagem quando o post tem `ThumbnailUrl` definido; sem capa, mostra o
  placeholder editorial.
- **Tags**: chips outlined, quebrados por vírgula.
- **Data**: `dd/MM/yyyy` quando `PublishedAt` é não-nulo.

## Acessibilidade

- `MudNavLink` no título do card preserva navegação por teclado.
- `MudButton EndIcon` no "Ler" reforça affordance.
- Paginação completa via `MudPagination` (não é `<select>` custom).

## Referências

- `TaxPlanner.Wasm/Pages/Blog.razor`
- `TaxPlanner.Wasm/Services/BlogService.cs`
- `TaxPlanner.Server/Controllers/PostsController.cs`
- [`../../integrations.md`](../../integrations.md) — contrato da API.
