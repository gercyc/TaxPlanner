# Módulo: Blog (administração)

- **Projeto**: `TaxPlanner.Server`
- **Controller**: `Controllers/AdminController.cs` (`AdminPostsController`)
- **Serviço**: `Services/BlogService.cs` (parte de CRUD)
- **Status**: Funcional — **dívida crítica de segurança pendente**

## Propósito

API REST **administrativa** para gestão completa dos posts do blog
(CRUD), publicação/despublicação e gestão de imagens. Consumida pelo
painel admin (Blazor Server em `TaxPlanner.Server/Components`).

> ⚠️ **Atenção**: no estado atual do código, **nenhum endpoint deste
> controller exige autenticação**. Antes de expor publicamente, adicionar
> `[Authorize]` (ou policy restritiva) e configurar a autenticação por
> cookie do `Identity` no `Program.cs` (já parcialmente configurado).

## Endpoints

| Método | Rota                                       | Descrição                                  |
| ------ | ------------------------------------------ | ------------------------------------------ |
| GET    | `/api/admin/posts`                         | Lista **todos** os posts (rascunhos + publicados) com `thumbnailUrl`/`thumbnailAlt`. |
| GET    | `/api/admin/posts/{id}`                    | Retorna post por id (com `ContentMarkdown` e `thumbnailUrl`/`thumbnailAlt`).  |
| POST   | `/api/admin/posts`                         | Cria post (rascunho). Aceita `thumbnailUrl`/`thumbnailAlt` opcionais. |
| PUT    | `/api/admin/posts/{id}`                    | Atualiza post. Aceita `thumbnailUrl`/`thumbnailAlt`; `null` remove. |
| DELETE | `/api/admin/posts/{id}`                    | Remove post.                                |
| PUT    | `/api/admin/posts/{id}/publish`            | Publica o post.                             |
| PUT    | `/api/admin/posts/{id}/unpublish`          | Despublica o post.                          |
| GET    | `/api/admin/posts/images`                  | Lista imagens em `wwwroot/uploads/posts/`. |
| DELETE | `/api/admin/posts/images/{fileName}`       | Remove imagem por nome.                    |

## Contratos

### `POST /api/admin/posts` (criar)

```json
{
  "title": "Planejamento Tributário 2026",
  "summary": "Resumo curto",
  "contentMarkdown": "# Conteúdo em Markdown",
  "tags": "tributos,fiscal,2026",
  "slug": "planejamento-tributario-2026",
  "thumbnailUrl": "https://exemplo.com/capa.jpg",
  "thumbnailAlt": "Mulher analisando planilha"
}
```

- `thumbnailUrl` e `thumbnailAlt` são opcionais.
- Quando `thumbnailUrl` é enviado e não-vazio, o backend exige scheme
  `http` ou `https` (case-insensitive). Esquemas como `javascript:`,
  `data:`, `file:`, `vbscript:` ou ausência de scheme retornam
  `400 Bad Request` com
  `{ "error": "thumbnailUrl deve iniciar com http:// ou https://" }` e
  nada é persistido.
- A imagem de capa é um campo **editorial** independente do bloco de
  upload inline (`POST /api/posts/upload-image`) que insere
  `![alt](url)` no markdown.

Resposta: `201 Created` com `Location: /api/admin/posts/{id}` e o post
completo (inclui `id`, `contentHtml` renderizado, `isPublished: false`,
`createdAt`, `thumbnailUrl`, `thumbnailAlt`).

### `PUT /api/admin/posts/{id}` (atualizar)

Mesma forma de `Create`, mais `isPublished: bool`, e os mesmos campos
opcionais `thumbnailUrl`/`thumbnailAlt`. A regra de validação de scheme
é idêntica ao `Create` — `null` ou string vazia **remove** a capa. Se
`IsPublished=true` e `PublishedAt` ainda era `null`, é setado para
`DateTime.UtcNow`.

### `GET /api/admin/posts` (lista) e `GET /api/admin/posts/{id}`

A resposta inclui `thumbnailUrl` e `thumbnailAlt` em ambos endpoints.

### `POST /api/admin/posts/{id}/publish` e `/unpublish`

- `publish`: seta `IsPublished=true` e `PublishedAt=UtcNow`.
- `unpublish`: seta `IsPublished=false`.
- Resposta: `204 No Content` (ou `404` se não encontrado).

## Dependências

- `ApplicationDbContext` (EF Core, PostgreSQL).
- `BlogService` (CRUD, render Markdown, gestão de imagens):
  - `GetAllPostsAsync`, `GetByIdAsync`.
  - `CreateAsync(title, summary, contentMarkdown, tags, slug,
    thumbnailUrl = null, thumbnailAlt = null)`.
  - `UpdateAsync(id, title, summary, contentMarkdown, tags, slug,
    isPublished, thumbnailUrl = null, thumbnailAlt = null)`.
  - `DeleteAsync`, `PublishAsync`, `UnpublishAsync`.
  - `UploadImageAsync`, `GetImages`, `DeleteImage`.
  - `RenderMarkdown` (Markdig com `UseAdvancedExtensions`).
  - `GenerateSlug` — slug em kebab-case com normalização de acentos.
  - `NormalizeCoverUrl` — helper estático que faz `Trim`, devolve `null`
    para string vazia, e exige scheme `http`/`https` (lança
    `ArgumentException` em pt-BR caso contrário).

## Comportamentos importantes

- **Slug**:
  - Se vazio, gerado a partir do título (`GenerateSlug`).
  - Substituições: remove acentos, troca espaços por `-`, colapsa `-`
    duplicados, remove caracteres não alfanuméricos/`-`.
- **Markdown → HTML**:
  - `ContentMarkdown` é a **fonte de verdade**.
  - `ContentHtml` é o **cache renderizado** pelo `Markdig` (pipeline
    `UseAdvancedExtensions()`).
  - `UpdateAsync` **sempre** recomputa o `ContentHtml`.
- **Imagem de capa**:
  - Pipeline: reusa o `POST /api/posts/upload-image` (limite 5 MB,
    `BlogService.AllowedImageExtensions`, `Guid.NewGuid():N` como nome,
    armazenamento em `wwwroot/uploads/posts/`). Não há pipeline paralelo
    — a capa **compartilha** o pipeline de upload de imagens inline.
    Ver [ADR 0004](../decisions/0004-uploads-de-imagem-em-disco-wwwroot-uploads-posts.md)
    para as regras de armazenamento.
  - URL pública (`http`/`https`) é persistida **como está** — sem
    download, sem espelhamento, sem rewrite para HTTPS. A URL é
    validada pelo `BlogService.NormalizeCoverUrl` no `CreateAsync`/
    `UpdateAsync`; `ArgumentException` é mapeada para `400 Bad Request`
    pelo controller.
  - `null` ou string vazia **remove** a capa.
  - Sem HEAD request ao recurso remoto (evita latência no `Save`).
- **Imagens**:
  - Limite: 5 MB; extensões: `jpg, jpeg, png, gif, webp, svg`.
  - Nome no disco: `Guid.NewGuid():N + extensão`.
  - **Validação de path traversal** (`..`, `/`, `\\`) em `DeleteImage`.
  - Constantes `BlogService.AllowedImageExtensions` e
    `BlogService.MaxImageSize` ficaram `internal static` para reuso pelo
    frontend admin (`NovoPost.razor`/`EditarPost.razor`) sem precisar
    duplicar a lista.

## Falhas conhecidas e dívidas técnicas

1. **Autorização ausente** — endpoint público com capacidade de
   criar/editar/deletar posts. Adicionar `[Authorize(Roles="Admin")]`
   ou policy própria antes de produção.
2. **Sem rate limiting** no upload de imagem.
3. **Slug único não é garantido** — `GenerateSlug` pode colidir;
   considerar retry com sufixo numérico.
4. **Sem auditoria** — quem publicou/despublicou não é rastreado.

## Referências

- `TaxPlanner.Server/Controllers/AdminController.cs`
- `TaxPlanner.Server/Services/BlogService.cs`
- `TaxPlanner.Server/Models/Post.cs`
- `TaxPlanner.Server/Migrations/20260608185223_CreatePostsTable.cs`
- `TaxPlanner.Server/Migrations/20260611020405_AddPostThumbnail.cs`
- `TaxPlanner.Server/Components/Pages/Admin/NovoPost.razor` — bloco "Imagem de capa"
- `TaxPlanner.Server/Components/Pages/Admin/EditarPost.razor` — bloco "Imagem de capa"
- [ADR 0004](../decisions/0004-uploads-de-imagem-em-disco-wwwroot-uploads-posts.md) — pipeline de upload reutilizado pela capa
- [`../integrations.md`](../integrations.md) — contrato da API
- [`../runbooks/uploads-de-imagens.md`](../runbooks/uploads-de-imagens.md)
