# Módulo: Blog (público)

- **Projeto**: `TaxPlanner.Server`
- **Controller**: `Controllers/PostsController.cs`
- **Serviço**: `Services/BlogService.cs` (parte de leitura)
- **Status**: Estável

## Propósito

API REST **pública** do blog. Fornece listagem paginada, detalhe por
slug, upload de imagem e feed RSS. Consumida pelo frontend Blazor
WebAssembly (`TaxPlanner.Wasm/Pages/Blog.razor`,
`TaxPlanner.Wasm/Pages/Post.razor`).

## Endpoints

| Método | Rota                          | Descrição                                                                                          | Autenticação |
| ------ | ----------------------------- | -------------------------------------------------------------------------------------------------- | ------------ |
| GET    | `/api/posts`                  | Lista posts publicados, paginados.                                                                 | Pública      |
| GET    | `/api/posts/{slug}`           | Detalhe (HTML + Markdown).                                                                          | Pública      |
| POST   | `/api/posts/upload-image`     | Upload de imagem (multipart, 5 MB máx).                                                            | Pública no código — **proteger** antes de produção |
| GET    | `/api/posts/rss`              | Feed RSS 2.0 com últimos 30 posts.                                                                 | Pública      |

## Contratos

### `GET /api/posts?page=1&pageSize=10`

```json
{
  "posts": [
    {
      "slug": "...",
      "title": "...",
      "summary": "...",
      "tags": "tributos,fiscal,2026",
      "publishedAt": "2026-06-01T12:00:00Z",
      "updatedAt": null,
      "thumbnailUrl": "https://exemplo.com/capa.jpg",
      "thumbnailAlt": "Mulher analisando planilha"
    }
  ],
  "page": 1,
  "pageSize": 10,
  "totalCount": 42
}
```

### `GET /api/posts/{slug}`

```json
{
  "slug": "...",
  "title": "...",
  "summary": "...",
  "contentHtml": "<h1>...</h1>",
  "contentMarkdown": "# ...",
  "tags": "...",
  "publishedAt": "...",
  "updatedAt": null,
  "thumbnailUrl": "https://exemplo.com/capa.jpg",
  "thumbnailAlt": "Mulher analisando planilha"
}
```

Os campos `thumbnailUrl`/`thumbnailAlt` são opcionais e refletem o que o
admin definiu na criação/edição (ver [`blog-admin.md`](./blog-admin.md)).
Quando `thumbnailUrl` é `null` ou vazio, o frontend público exibe o
placeholder editorial no card/hero. O frontend Wasm já consome esses
campos sem mudanças adicionais (binding JSON absorve a adição de chaves
em contratos já existentes).

`404` quando o slug não existe ou post não está publicado.

### `POST /api/posts/upload-image`

- `Content-Type: multipart/form-data`, campo `file`.
- Validações: tamanho ≤ 5 MB, extensão em
  `AllowedImageExtensions = { jpg, jpeg, png, gif, webp, svg }`.
- Resposta: `{ "url": "/uploads/posts/<guid>.<ext>", "fileName": "..." }`.
- O arquivo é gravado em
  `TaxPlanner.Server/wwwroot/uploads/posts/<guid>.<ext>`.

### `GET /api/posts/rss`

- `Content-Type: application/rss+xml`.
- Inclui `<channel>` com `title=TaxPlanner Blog`, `language=pt-BR`, e
  `<atom:link rel="self">` apontando para o próprio feed.
- Itens: últimos 30 posts publicados, ordenados por `PublishedAt DESC`.

## Dependências

- `ApplicationDbContext` (EF Core, PostgreSQL).
- `BlogService`:
  - `GetPublishedPostsAsync(page, pageSize)`.
  - `GetPublishedCountAsync()`.
  - `GetBySlugAsync(slug)`.
- `Markdig` para renderizar `ContentMarkdown` em `ContentHtml` no
  momento de criar/editar (ver [`blog-admin.md`](./blog-admin.md)).

## Falhas conhecidas e dívidas técnicas

1. **Upload de imagem é público** — `PostsController.UploadImage` não
   tem `[Authorize]`. Antes de expor para a internet, proteger com
   política de admin ou JWT interno.
2. **Sem sanitização de HTML** — `ContentHtml` é cache derivado do
   Markdown via `Markdig`. Aceitável enquanto apenas admins publicam.
3. **Sem paginação máxima** — `pageSize` é aceito sem limite, o que
   pode degradar performance em chamadas com `pageSize=10000`.

## Referências

- `TaxPlanner.Server/Controllers/PostsController.cs`
- `TaxPlanner.Server/Services/BlogService.cs`
- `TaxPlanner.Server/Models/Post.cs`
- `TaxPlanner.Wasm/Services/BlogService.cs` (cliente HTTP)
- `TaxPlanner.Wasm/Components/BlogThumbnail.razor` — renderiza a capa em featured/card
- `TaxPlanner.Wasm/Pages/Post.razor` — renderiza a hero no detalhe
- [`../integrations.md`](../integrations.md)
- [`../runbooks/uploads-de-imagens.md`](../runbooks/uploads-de-imagens.md)
