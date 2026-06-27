# Integrações — TaxPlanner

Catálogo de integrações **internas e externas** do projeto **TaxPlanner**,
com seus contratos, autenticação e políticas de falha.

## Visão geral

| Integração                         | Tipo                | Autenticação                              | Direção            | Status    |
| ---------------------------------- | ------------------- | ----------------------------------------- | ------------------ | --------- |
| `TaxPlanner.Wasm` ↔ `TaxPlanner.Server` | HTTP/REST interna   | Pública (leitura) / Cookie (admin)        | Cliente → Servidor | Ativa     |
| PostgreSQL                         | Banco de dados      | Connection string (`DefaultConnection`)   | Servidor ↔ DB      | Ativa     |
| Google Fonts (Plus Jakarta Sans, Inter, JetBrains Mono) | CDN externa | Pública (carregamento client-side) | Cliente → Externa   | Ativa     |
| MudBlazor (componentes UI)         | Pacote NuGet        | n/a                                       | Build-time         | Ativa     |
| Markdig (render Markdown)          | Pacote NuGet        | n/a                                       | Server-side        | Ativa     |
| RSS feed                           | Feed público        | Pública                                   | Servidor → Externo | Ativa     |

## Integração interna: Wasm ↔ Server

### Endpoints públicos (`PostsController`)

Base: `/api/posts`

| Método | Rota                | Descrição                                                                                  | Autenticação |
| ------ | ------------------- | ------------------------------------------------------------------------------------------ | ------------ |
| GET    | `/api/posts`        | Lista posts publicados (paginado: `?page=1&pageSize=10`).                                  | Pública      |
| GET    | `/api/posts/{slug}` | Retorna post publicado por slug (Markdown + HTML).                                         | Pública      |
| POST   | `/api/posts/upload-image` | Upload de imagem (multipart, 5 MB máx, extensões de imagem).                          | Pública no código atual — **proteger** antes de produção |
| GET    | `/api/posts/rss`    | Feed RSS 2.0 com últimos 30 posts publicados.                                              | Pública      |

**Contrato de listagem** (`GET /api/posts`):

```json
{
  "posts": [
    {
      "slug": "planejamento-tributario-2026",
      "title": "Planejamento Tributário 2026",
      "summary": "Resumo curto do post",
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

**Contrato de detalhe** (`GET /api/posts/{slug}`):

```json
{
  "slug": "planejamento-tributario-2026",
  "title": "Planejamento Tributário 2026",
  "summary": "Resumo curto do post",
  "contentHtml": "<h1>...</h1>",
  "contentMarkdown": "# ...",
  "tags": "tributos,fiscal,2026",
  "publishedAt": "2026-06-01T12:00:00Z",
  "updatedAt": null,
  "thumbnailUrl": "https://exemplo.com/capa.jpg",
  "thumbnailAlt": "Mulher analisando planilha"
}
```

**Upload de imagem** (`POST /api/posts/upload-image`, multipart):

- Campo: `file`.
- Limite: 5 MB (`RequestSizeLimit`).
- Extensões permitidas: `.jpg`, `.jpeg`, `.png`, `.gif`, `.webp`, `.svg`.
- Resposta: `{ "url": "/uploads/posts/<guid>.png", "fileName": "<guid>.png" }`.

**Feed RSS** (`GET /api/posts/rss`):

- Tipo: `application/rss+xml`.
- Inclui os últimos 30 posts publicados.
- `<atom:link rel="self">` aponta para a própria URL do feed.
- `<language>pt-BR</language>`.

### Endpoints administrativos (`AdminPostsController`)

Base: `/api/admin/posts`

> ⚠️ **Atenção**: no estado atual do código, os endpoints abaixo **não
> exigem autenticação** no `Program.cs`. Antes de expor para a internet,
> adicionar `[Authorize]` (ou policy) e proteger com Identity/cookies.

| Método | Rota                              | Descrição                                   |
| ------ | --------------------------------- | ------------------------------------------- |
| GET    | `/api/admin/posts`                | Lista todos os posts (rascunhos + publicados). |
| GET    | `/api/admin/posts/{id}`           | Retorna post por id (com `ContentMarkdown`).  |
| POST   | `/api/admin/posts`                | Cria post.                                   |
| PUT    | `/api/admin/posts/{id}`           | Atualiza post.                               |
| DELETE | `/api/admin/posts/{id}`           | Remove post.                                 |
| PUT    | `/api/admin/posts/{id}/publish`   | Publica o post.                              |
| PUT    | `/api/admin/posts/{id}/unpublish` | Despublica o post.                           |
| GET    | `/api/admin/posts/images`         | Lista imagens em `wwwroot/uploads/posts/`.   |
| DELETE | `/api/admin/posts/images/{name}`  | Remove imagem por nome (com validação de path traversal). |

**Contrato de criação** (`POST /api/admin/posts`):

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

Campos `thumbnailUrl`/`thumbnailAlt` são opcionais. Quando `thumbnailUrl`
é enviado e não-vazio, o backend exige scheme `http` ou `https`
(case-insensitive); qualquer outro valor retorna
`400 Bad Request` com `{ "error": "thumbnailUrl deve iniciar com http:// ou https://" }`
e nada é persistido. O `PUT /api/admin/posts/{id}` aceita a mesma
forma, mais o campo `isPublished`, e a regra de validação é idêntica.

Resposta: `201 Created` com o post completo (incluindo `id`,
`contentHtml` renderizado pelo `Markdig`, `createdAt`, `isPublished: false`).

## Banco de dados: PostgreSQL

- **Provider**: `Npgsql.EntityFrameworkCore.PostgreSQL 10.0.2`.
- **Connection string**: chave `DefaultConnection` em
  `appsettings.json` (ou user secrets em dev).
- **Schema**:
  - Tabelas `AspNet*` (Identity): `Users`, `Roles`, `UserRoles`, `UserClaims`,
    `UserLogins`, `UserTokens`, `RoleClaims`, `Roles`.
  - Tabela `Posts` (ver `Models/Post.cs`):
    - `Id` (PK, int, identity).
    - `Slug` (varchar 200, **unique** recomendado).
    - `Title` (varchar 300).
    - `Summary` (varchar 500, nullable).
    - `ContentMarkdown` (text).
    - `ContentHtml` (text, cache derivado).
    - `Tags` (varchar 500, nullable, comma-separated).
    - `ThumbnailUrl` (varchar 500, nullable) — URL da imagem de capa; aceita upload local `/uploads/posts/...` ou `http(s)://...` externos.
    - `ThumbnailAlt` (varchar 200, nullable) — texto alternativo da capa.
    - `CreatedAt` (timestamp, default `utcnow`).
    - `UpdatedAt` (timestamp, nullable).
    - `PublishedAt` (timestamp, nullable).
    - `IsPublished` (bool).
- **Migrações**:
  - Inicial: `20260608185223_CreatePostsTable` (Postgres).
  - Capa: `20260611020405_AddPostThumbnail` (Postgres) — adiciona `ThumbnailUrl`/`ThumbnailAlt` (nullable, sem backfill).
  - Comando: `dotnet ef migrations add <Nome> --project TaxPlanner.Server`.

## CDN externa: Google Fonts

- **Uso**: tipografia do frontend (`Plus Jakarta Sans` + `Inter` + `JetBrains
  Mono`).
- **Origem**: `<link>` no `TaxPlanner.Wasm/wwwroot/index.html`.
- **Falhas**: o navegador faz fallback para a stack do sistema
  (`-apple-system, BlinkMacSystemFont, "Segoe UI", ...`) caso a CDN esteja
  inacessível.

## Limites e notas de propriedade

- **Frontend Wasm** é cliente do contrato REST do `TaxPlanner.Server`.
  Mudanças de contrato devem atualizar ambos os lados no mesmo PR.
- **Uploads** ficam no disco do `TaxPlanner.Server` (`wwwroot/uploads/posts/`).
  Em ambientes efêmeros, é necessário volume persistente ou storage externo
  (S3, Blob Storage etc.).
- **Identity** usa schema padrão v3 (`IdentitySchemaVersions.Version3`).

## Tratamento de falhas

| Cenário                                  | Comportamento atual                                                                 |
| ---------------------------------------- | ----------------------------------------------------------------------------------- |
| DB inacessível                           | Aplicação aborta no startup (InvalidOperationException em `Program.cs`).            |
| Slug inexistente em `GET /api/posts/{slug}` | `404 Not Found` com `{ "error": "Post not found" }`.                              |
| Arquivo inválido no upload               | `400 Bad Request` com mensagem explicando extensão ou tamanho.                      |
| CORS em produção                         | Política atual é permissiva — restringir antes de expor publicamente.                |
| Path traversal em `DELETE /api/admin/posts/images/{name}` | `400 Bad Request` (verifica `..`, `/`, `\\`). |
| `thumbnailUrl` com scheme inválido (POST/PUT) | `400 Bad Request` com `{ "error": "thumbnailUrl deve iniciar com http:// ou https://" }`. Nada é persistido. |
