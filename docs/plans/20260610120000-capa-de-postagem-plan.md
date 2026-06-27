---
title: "Imagem de capa na criação/edição de postagens (upload ou URL pública)"
type: enhancement
status: active
date: 2026-06-10
phased: true
---

# Imagem de capa na criação/edição de postagens (upload ou URL pública)

## Overview

O painel admin (`TaxPlanner.Server/Components/Pages/Admin/NovoPost.razor` e
`EditarPost.razor`) permite enviar imagens que são embutidas no markdown do
post, mas **não há um campo dedicado para a imagem de capa (thumbnail/hero)**.
Por outro lado, o front público (`TaxPlanner.Wasm/Components/BlogThumbnail.razor`,
`Pages/Post.razor`, `Pages/Blog.razor`) **já espera** os campos
`ThumbnailUrl` e `ThumbnailAlt` nos contratos `PostSummary` e `PostDetail`
(`TaxPlanner.Wasm/Services/BlogService.cs:14-15`) e já renderiza uma hero
quando `ThumbnailUrl` está presente (`Post.razor:62-69`). Hoje a capa nunca
aparece porque o **backend não envia** esses campos nas respostas.

**O que vamos construir:**

- Um campo "Imagem de capa" nos formulários de nova postagem e edição.
- O admin pode escolher entre:
  1. **Upload** de um arquivo local (reutiliza o pipeline existente do
     `BlogService.UploadImageAsync` e o endpoint `/api/posts/upload-image`).
  2. **URL pública** (http/https) — armazenada como está, sem espelhamento.
- A URL da capa e o texto alternativo (`alt`) serão persistidos em duas
  novas colunas na tabela `Posts`.
- O backend passa a devolver `thumbnailUrl`/`thumbnailAlt` em todos os
  contratos públicos (`GET /api/posts` e `GET /api/posts/{slug}`) e nos
  contratos administrativos (`GET/POST/PUT /api/admin/posts`).
- O front público já consome esses campos — sem alterações necessárias,
  exceto o `HttpClient` base do Wasm se o host for diferente.

**Público-alvo:** o próprio autor/admin do blog, que hoje precisa inserir a
capa via markdown no meio do conteúdo, sem preview editorial na listagem.

## Scope / Work Breakdown

| Grupo | Requisitos | Camada |
| --- | --- | --- |
| G1 — Modelo de dados | 2 colunas novas em `Posts`: `ThumbnailUrl` (varchar 500) e `ThumbnailAlt` (varchar 200) + migration EF Core | Backend (data) |
| G2 — Serviço + Controllers | Aceitar/atualizar/expor `ThumbnailUrl`/`ThumbnailAlt` em `BlogService.CreateAsync`, `UpdateAsync`, `PostsController.List/Get`, `AdminPostsController.Create/Get/Update` | Backend (service + API) |
| G3 — UI admin (Server) | Bloco "Imagem de capa" no topo de `NovoPost.razor` e `EditarPost.razor` com 2 botões (Upload / Usar URL), preview e alt text | Frontend Server (Blazor) |
| G4 — UI pública (Wasm) | Sem mudança obrigatória (já consome `ThumbnailUrl`/`ThumbnailAlt`); verificar alinhamento da `BaseAddress` e da paginação | Frontend Wasm (verificação) |
| G5 — Documentação | Atualizar `integrations.md`, `modules/blog-admin.md`, `modules/blog-publico.md` e `features/blog-post.md`/`features/blog-listagem.md`; novo ADR para o limite de tamanho do upload (ou referenciar o 0004) | Documentação |

## Proposed Solution

### Decisões de design (registradas neste plano)

1. **Persistência de URL pública como string.** A URL é gravada no banco
   exatamente como o admin a digitou, sem download, sem espelhamento e sem
   reescrita para HTTPS. Isso evita custo de banda, evita amplificar SSRF
   e mantém o caminho de upgrade para o `IFileStorageService` descrito no
   ADR 0004 focado em arquivos enviados.
2. **Validação no servidor:** o backend exige `http` ou `https` como
   scheme. Qualquer outro scheme (`javascript:`, `data:`, `file:`, `vbscript:`,
   `blob:` etc.) é rejeitado com `400 Bad Request` quando o post é criado
   ou atualizado. Comprimento máximo: **500 caracteres** (alinhado com
   o tamanho da coluna).
3. **Upload segue ADR 0004.** Mesmo endpoint, mesmas regras (5 MB, lista
   de extensões, `Guid.NewGuid():N` no nome, diretório
   `wwwroot/uploads/posts/`).
4. **Coexistência com o bloco de imagens inline.** O bloco de upload
   existente em `NovoPost.razor`/`EditarPost.razor` (que insere
   `![alt](url)` no markdown) **continua existindo** sem mudanças. O
   campo de capa é um bloco separado, no topo, com semântica diferente
   ("capa do post" ≠ "imagem dentro do conteúdo").
5. **`ThumbnailAlt` é opcional**, mas recomendado. Quando ausente, o
   `BlogThumbnail.razor` já cai num fallback (`"Imagem de capa: <título>"`).
6. **Sem validação de existência do recurso remoto.** Não fazemos HEAD
   request para a URL pública — isso evita custo extra, latência no
   `Save` e falsos negativos por trás de CDNs com auth. A imagem pode
   quebrar visualmente; o admin ajusta se necessário.
7. **Sem migração de posts antigos.** Colunas são nullable; posts
   existentes ficam com `null` e o `BlogThumbnail` mostra o placeholder
   editorial. Sem backfill automático.

### Modelo de dados

Novas colunas em `Posts` (Postgres, via EF Core migration):

| Coluna | Tipo | Nullable | Default | Restrição |
| --- | --- | --- | --- | --- |
| `ThumbnailUrl` | `varchar(500)` | sim | `null` | — |
| `ThumbnailAlt` | `varchar(200)` | sim | `null` | — |

Adicionar em `TaxPlanner.Server/Models/Post.cs`:

```csharp
[MaxLength(500)]
public string? ThumbnailUrl { get; set; }

[MaxLength(200)]
public string? ThumbnailAlt { get; set; }
```

> **Por que duas colunas em vez de uma estrutura `Thumbnail { Url, Alt }`?**
> O resto do `Post` é plano; manter consistência com `Summary`/`Tags`/
> `CoverImage`. As projeções anônimas dos controllers já omitem os
> campos sensíveis (`ContentHtml` na listagem) e devolvem os demais —
> o `ThumbnailUrl`/`ThumbnailAlt` cabem no mesmo padrão.

### Endpoints (contratos)

#### `POST /api/admin/posts` — corpo passa a aceitar:

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

- `thumbnailUrl` opcional.
- Se presente e não vazio, deve começar com `http://` ou `https://` —
  caso contrário `400 Bad Request` com
  `{ "error": "thumbnailUrl deve iniciar com http:// ou https://" }`.

#### `PUT /api/admin/posts/{id}` — mesma forma de Create + `isPublished`.

#### `GET /api/admin/posts` (lista) e `GET /api/admin/posts/{id}` — resposta inclui:

```json
{
  "id": 1, "slug": "...", "title": "...", "summary": "...",
  "tags": "...", "isPublished": true,
  "publishedAt": "...", "createdAt": "...", "updatedAt": "...",
  "thumbnailUrl": "https://exemplo.com/capa.jpg",
  "thumbnailAlt": "..."
}
```

#### `GET /api/posts` (público) e `GET /api/posts/{slug}` — resposta inclui:

```json
{
  "slug": "...", "title": "...", "summary": "...",
  "tags": "...", "publishedAt": "...", "updatedAt": null,
  "thumbnailUrl": "https://exemplo.com/capa.jpg",
  "thumbnailAlt": "..."
}
```

> Os records `PostSummary` e `PostDetail` em
> `TaxPlanner.Wasm/Services/BlogService.cs:14-15` **já esperam** esses
> dois campos — por isso o binding JSON do Wasm já absorve a mudança
> sem precisar de `JsonSerializerOptions` extras.

### UI admin (Server)

Em `NovoPost.razor` e `EditarPost.razor`, **inserir antes do `MudItem`
do título** um novo bloco:

- Label "Imagem de capa".
- `MudTextField` ligado a `_model.ThumbnailUrl` (placeholder:
  `https://exemplo.com/capa.jpg`).
- `MudButton` "Fazer upload" (`HtmlTag="label"`) que abre o
  `InputFile`; no callback, chama o
  `BlogService.UploadImageAsync` existente, recebe a URL, e preenche
  `_model.ThumbnailUrl`.
- `MudTextField` ligado a `_model.ThumbnailAlt` (placeholder:
  `Descrição da imagem (acessibilidade)`).
- `MudPaper` com `<img src="@_model.ThumbnailUrl">` quando não vazio,
  com `loading="lazy"`, altura 240px, `object-fit: cover` e bordas
  arredondadas (`var(--tp-radius-md)`).
- `MudButton` "Remover capa" quando há URL definida (limpa os 2 campos).

> O bloco é um "fieldset" simples. Não usamos `MudFileUpload` porque
> o padrão atual do projeto já usa `<InputFile hidden>` dentro de
> `MudButton HtmlTag="label"`.

## Technical Considerations

### Segurança

- **SSRF / open redirect:** não é um risco aqui porque **não baixamos**
  a URL. Renderizamos como `<img src>`. O navegador valida o scheme
  nativamente; se um admin digitar `javascript:foo`, o `<img>` não
  executa. Ainda assim, **validamos no servidor** para evitar armazenar
  lixo que quebre o front.
- **XSS via `alt`:** o Wasm injeta o `alt` em `alt="..."` (atributo
  HTML escapado pelo Blazor) e o Blazor em si escapa atributos
  automaticamente. **Não** usamos `@(MarkupString)` no `alt`.
- **Validação de scheme:** `http`/`https` apenas, case-insensitive.
- **Limite de URL:** 500 caracteres (alinhado à coluna). Backend
  retorna `400` se exceder.

### Migração

- Geração: `dotnet ef migrations add AddPostThumbnail --project TaxPlanner.Server`.
- A migration será aplicada localmente imediatamente após gerada
  (regra: atomic chain — gerar → drift-check → rodar localmente).
- Ambas as colunas são `nullable`, sem default, sem `unique` — backfill
  não é necessário; `null` significa "sem capa" e cai no placeholder.

### Padrões de projeto

- **Língua:** código e mensagens de erro em **português (pt-BR)**, em
  linha com o resto do projeto (ex.: "Tipo de ficheiro inválido" no
  `BlogService.cs:140`).
- **Mensagens de erro:** seguir o padrão já existente nos controllers
  (`{ "error": "..." }`).
- **Validação em duas camadas:** controller faz o parse simples
  (scheme, tamanho); a regra de "negócio" (persistir) fica no
  `BlogService`.
- **Sem novos serviços:** estende `BlogService` diretamente, sem
  abstrações prematuras (YAGNI). O ADR 0004 já prevê refactor para
  `IFileStorageService` no futuro.

### Compatibilidade

- **Front público:** o Wasm já lê `thumbnailUrl`/`thumbnailAlt` como
  nullable; sem `JsonIgnoreCondition`, o binding trata `null` como
  ausente e o `BlogThumbnail` cai no placeholder.
- **API pública:** adicionar campos ao JSON é **compatível** com
  consumidores existentes (aditiva).
- **CORS:** nenhuma mudança necessária.

### Performance

- A projeção dos controllers usa objetos anônimos — incluir 2
  propriedades a mais é desprezível.
- Nenhuma query nova no banco.

## Acceptance Criteria

### Criar post com upload de capa (happy path)

- **Dado** um admin autenticado em `/admin/posts/novo`
- **Quando** ele clica em "Fazer upload" e seleciona `capa.jpg` (1 MB)
- **E** digita o alt "Mulher com planilha"
- **E** clica em "Salvar Rascunho"
- **Então** o backend salva o post com `thumbnailUrl =
  /uploads/posts/<guid>.jpg` e `thumbnailAlt = "Mulher com planilha"`
- **E** o `GET /api/admin/posts` lista o post com a URL da capa
- **E** o `GET /api/posts/{slug}` retorna a capa no JSON

### Criar post com URL pública (happy path)

- **Dado** um admin em `/admin/posts/novo`
- **Quando** ele cola `https://exemplo.com/capa.jpg` no campo de URL
- **E** clica em "Salvar Rascunho"
- **Então** o backend persiste a URL como está e devolve o post com
  `thumbnailUrl = "https://exemplo.com/capa.jpg"`

### Editar e remover capa

- **Dado** um post existente com capa
- **Quando** o admin edita o post e clica em "Remover capa"
- **E** salva
- **Então** o backend grava `thumbnailUrl = null` e `thumbnailAlt = null`
- **E** o front público volta a exibir o placeholder

### Front público renderiza a capa

- **Dado** um post publicado com `thumbnailUrl` definido
- **Quando** o visitante acessa `/post/{slug}`
- **Então** o `<article class="tp-article">` contém o
  `<figure class="tp-article-hero">` com `<img src=thumbnailUrl>` (linha
  62-69 de `Post.razor`)

### Card da listagem mostra a capa

- **Dado** o blog `/blog` com posts publicados
- **Quando** o post em destaque (primeiro da página 1) tem capa
- **Então** `BlogThumbnail` (variant `featured`) renderiza o `<img>`
- **E** os demais cards (variant `card`) também renderizam a capa

### Validação de scheme

- **Dado** um admin enviando `javascript:alert(1)` como URL de capa
- **Quando** ele salva
- **Então** o backend retorna `400 Bad Request` com mensagem clara
- **E** nada é persistido

### URL inválida (formato)

- **Dado** um admin enviando `://sem-scheme` como URL
- **Quando** ele salva
- **Então** o backend retorna `400 Bad Request`

### URL sem scheme (caso comum)

- **Dado** um admin enviando `www.exemplo.com/capa.jpg` (esqueceu o
  `https://`)
- **Quando** ele salva
- **Então** o backend retorna `400 Bad Request` com a mesma mensagem
  do caso anterior, **sugerindo** prefixar com `http://` ou `https://`

### Post sem capa (regressão)

- **Dado** um post sem `thumbnailUrl` (criado antes da feature)
- **Quando** ele é listado/detalhado
- **Então** o `BlogThumbnail` exibe o placeholder editorial (ícone +
  "TaxPlanner")

### Sanitização do alt

- **Dado** um admin digitando `<script>alert(1)</script>` como alt
- **Quando** o post é renderizado no front
- **Então** o Blazor escapa o atributo e nenhum script executa

### Limite de tamanho

- **Dado** um admin tentando upload de capa com 6 MB
- **Quando** ele seleciona o arquivo
- **Então** o frontend exibe `Snackbar` de erro "máximo 5 MB"
- **E** nenhum upload é feito

## Implementation Plan

| Phase | Name | Depends On | Status |
| --- | --- | --- | --- |
| 1 | Modelo de dados + migration | None | ✅ Completed |
| 2 | Serviço + Controllers (admin + público) | Phase 1 | ⬜ Pending |
| 3 | UI admin (NovoPost + EditarPost) | Phase 2 | ⬜ Pending |
| 4 | Documentação | Phase 3 | ⬜ Pending |

---

### Phase 1: Modelo de dados + migration

**Status**: ✅ Completed (2026-06-10)
**Objective**: adicionar `ThumbnailUrl` e `ThumbnailAlt` em `Post`, gerar
migration e aplicar localmente sem drift.
**Dependencies**: None

**Tasks**:

- [ ] T001 [US1] Adicionar propriedades em `TaxPlanner.Server/Models/Post.cs`
  - Adicionar `ThumbnailUrl` com `[MaxLength(500)]`, tipo `string?`
  - Adicionar `ThumbnailAlt` com `[MaxLength(200)]`, tipo `string?`
  - Manter agrupamento ao lado de `Tags` (mesma família de campos editoriais)
- [ ] T002 [US1] Gerar migration EF Core
  - Comando: `dotnet ef migrations add AddPostThumbnail --project TaxPlanner.Server -o Migrations`
  - Verificar que o `Up` cria **apenas** as 2 colunas novas em `Posts`
    (sem mudanças em outras tabelas) — drift-check via inspeção do
    arquivo gerado
  - Verificar que o `Down` remove as 2 colunas
  - Rodar localmente: `dotnet ef database update --project TaxPlanner.Server`
  - Validar com `psql` (ou `dotnet ef dbcontext info`) que as colunas
    existem com os tipos esperados

**After completing this phase**:

1. `dotnet build` em `TaxPlanner.slnx` — 0 erros e 0 warnings.
2. Marcar Phase 1 como `✅ Completed` na tabela.

---

### Phase 2: Serviço + Controllers (admin + público)

**Status**: ✅ Completed (2026-06-10)
**Objective**: persistir e devolver `ThumbnailUrl`/`ThumbnailAlt` em todos
os endpoints, com validação de scheme.
**Dependencies**: Phase 1

**Tasks**:

- [ ] T003 [US1] Estender `BlogService.CreateAsync` em
  `TaxPlanner.Server/Services/BlogService.cs`
  - Adicionar parâmetro `string? thumbnailUrl, string? thumbnailAlt` ao
    final da assinatura (preservar compat com chamadas internas que
    ainda não passam)
  - Atribuir `post.ThumbnailUrl = thumbnailUrl` e
    `post.ThumbnailAlt = thumbnailAlt` (helper já faz `Trim()` —
    ver T005)
  - Se `thumbnailUrl` for não-nulo e não-vazio, **validar scheme** com
    helper `IsValidExternalUrl` (helper privado: `Uri.TryCreate` +
    `Scheme == "http" || "https"`); se inválido, lançar
    `ArgumentException` com mensagem em pt-BR
  - **Assinatura final** (com defaults `null` para não quebrar callers
    legados que ainda não foram migrados):
    `Task<Post> CreateAsync(string title, string? summary, string contentMarkdown, string? tags, string slug, string? thumbnailUrl = null, string? thumbnailAlt = null)`
- [ ] T004 [US1] Estender `BlogService.UpdateAsync` em
  `TaxPlanner.Server/Services/BlogService.cs`
  - Mesma assinatura nova, com defaults `null`:
    `Task<Post?> UpdateAsync(int id, string title, string? summary, string contentMarkdown, string? tags, string slug, bool isPublished, string? thumbnailUrl = null, string? thumbnailAlt = null)`
  - Mesma validação de scheme
  - Sobrescrever `post.ThumbnailUrl` e `post.ThumbnailAlt` com os novos
    valores (incluindo `null` para "remover capa")
- [ ] T005 [US1] Adicionar helper privado em `BlogService`
  - `private static string? NormalizeCoverUrl(string? url)` — faz
    `Trim()`; se vazio retorna `null`; se não vazio, exige scheme
    `http`/`https` via `Uri.TryCreate(value, UriKind.Absolute, out var u) && (u.Scheme == Uri.UriSchemeHttp || u.Scheme == Uri.UriSchemeHttps)`; caso contrário lança `ArgumentException` em pt-BR
  - Encapsular `Trim` + validação no helper para ter uma única fonte
    da verdade (T003 e T004 só chamam o helper)
- [ ] T006 [US1] Estender `CreatePostRequest` em
  `TaxPlanner.Server/Controllers/AdminController.cs`
  - Adicionar `string? ThumbnailUrl` e `string? ThumbnailAlt` ao record
- [ ] T007 [US1] Estender `UpdatePostRequest` em
  `TaxPlanner.Server/Controllers/AdminController.cs`
  - Mesmos campos adicionais
- [ ] T008 [US1] Atualizar `Create` no `AdminPostsController` para
  passar os novos campos ao `BlogService.CreateAsync`
  - Em caso de `ArgumentException` (URL inválida), retornar
    `BadRequest(new { error = ex.Message })`
- [ ] T009 [US1] Atualizar `Update` no `AdminPostsController` para
  passar os novos campos ao `BlogService.UpdateAsync`
  - Mesmo tratamento de `ArgumentException`
- [ ] T010 [US1] Atualizar `List` no `AdminPostsController` para incluir
  `thumbnailUrl`/`thumbnailAlt` na projeção anônima
- [ ] T011 [US1] Atualizar `Get` (admin) para incluir os 2 campos na
  resposta
  - Manter inalterada a projeção atual (que já devolve `ContentHtml`,
    `ContentMarkdown` e `Summary`); apenas **adicionar**
    `post.ThumbnailUrl` e `post.ThumbnailAlt` ao objeto anônimo
    retornado (`AdminController.cs:45-58`)
- [ ] T012 [US1] Atualizar `List` no `PostsController` (público) para
  incluir `thumbnailUrl`/`thumbnailAlt` na projeção de `PostSummary`
- [ ] T013 [US1] Atualizar `Get` (público) no `PostsController` para
  incluir `thumbnailUrl`/`thumbnailAlt` na resposta de `PostDetail`

**After completing this phase**:

1. `dotnet build` em `TaxPlanner.slnx` — 0 erros e 0 warnings.
2. Marcar Phase 2 como `✅ Completed`.

---

### Phase 3: UI admin (NovoPost + EditarPost)

**Status**: ✅ Completed (2026-06-10)
**Objective**: disponibilizar campo "Imagem de capa" no topo dos dois
formulários, com 2 modos (upload/URL) e preview.
**Dependencies**: Phase 2

**Tasks**:

- [ ] T014 [US1] Adicionar campos no `PostCreateModel` em
  `TaxPlanner.Server/Components/Pages/Admin/NovoPost.razor`
  - Adicionar `public string? ThumbnailUrl { get; set; }` e
    `public string? ThumbnailAlt { get; set; }` à classe privada
- [ ] T015 [US1] Inserir bloco "Imagem de capa" no `NovoPost.razor`
  - Posição: **antes** do `MudItem xs="12" md="8"` do título
  - **Renomear o bloco existente de imagens inline** (linhas 58-116)
    de "Imagens" para "Imagens do conteúdo" para deixar claro que a
    capa é distinta das imagens inseridas no markdown
  - Layout: `MudItem xs="12"` com `MudPaper` contendo:
    - `MudTextField @bind-Value="_model.ThumbnailUrl"` (label "URL da
      imagem de capa", placeholder `https://exemplo.com/capa.jpg`,
      variant Outlined)
    - `MudButton` **"Fazer upload da capa"** (rótulo explícito) que
      aciona o `InputFile` (id próprio `upload-cover-input-novo`)
    - `MudTextField @bind-Value="_model.ThumbnailAlt"` (label "Texto
      alternativo (acessibilidade)", variant Outlined, HelperText
      "Descreva a imagem para leitores de tela")
    - Preview: `<img src="@_model.ThumbnailUrl" loading="lazy" ...>`
      quando não-vazio, com `object-fit: cover`, altura 240px, raio
      `var(--tp-radius-md)`, mensagem "Sem capa definida" quando vazio
    - `MudButton Color="Color.Error" OnClick="ClearCover"` "Remover capa"
      visível quando há URL
- [ ] T016 [US1] Adicionar handler `HandleCoverUpload` em
  `NovoPost.razor` (`@code`)
  - Idêntico ao `HandleImageUpload` existente, mas com id próprio
    (`upload-cover-input-novo`), limite de tamanho e validação de
    extensão — **reusar** as constantes públicas `BlogService.AllowedImageExtensions`
    e `BlogService.MaxImageSize` (hoje privadas em `BlogService.cs:126-131`).
    Para isso, expor como `internal static readonly` no `BlogService`
    (mínimo impacto, mesmo assembly) e remover o `HashSet` duplicado
    que está em `NovoPost.razor:181` e `EditarPost.razor:218`.
  - Em sucesso, atribui `_model.ThumbnailUrl = url` retornado pelo
    `BlogService.UploadImageAsync`; **não insere** markdown
  - Reusa `Snackbar` para feedback
- [ ] T017 [US1] Adicionar handler `ClearCover` em `NovoPost.razor`
  - `_model.ThumbnailUrl = null; _model.ThumbnailAlt = null`
- [ ] T018 [US1] Atualizar `Save` e `SaveAndPublish` em `NovoPost.razor`
  - Passar `_model.ThumbnailUrl` e `_model.ThumbnailAlt` como
    parâmetros adicionais a `BlogService.CreateAsync`
  - Capturar `ArgumentException` para exibir
    `Snackbar.Add(ex.Message, Severity.Error)` (UX consistente com o
    resto do arquivo)
- [ ] T019 [US1] Repetir T014–T018 em
  `TaxPlanner.Server/Components/Pages/Admin/EditarPost.razor`
  - Classe `PostEditModel` ganha os mesmos 2 campos
  - **Renomear o bloco "Imagens"** (linhas 68-126) para "Imagens do
    conteúdo" — espelhando T015 para manter consistência com
    `NovoPost.razor`
  - `OnInitializedAsync` preenche os campos a partir de
    `post.ThumbnailUrl`/`post.ThumbnailAlt`
  - Id do `InputFile` diferente: `upload-cover-input-editar`
  - Handler do upload chama `BlogService.AllowedImageExtensions` /
    `BlogService.MaxImageSize` (constantes expostas em T016)
  - `Save`/`SaveAndPublish` passam os 2 novos parâmetros a
    `BlogService.UpdateAsync`
- [ ] T020 [US1] Smoke test manual
  - `dotnet run --project TaxPlanner.Wasm` para validar UI (ou
    `dotnet run --project TaxPlanner.Server` e abrir
    `https://localhost:7053/admin/posts/novo`)
  - Cenários: upload válido, URL pública válida, URL com scheme
    inválido (verificar snackbar de erro), remover capa, editar post
    existente
  - Visual: capa aparece em `/blog` e `/post/{slug}`

**After completing this phase**:

1. `dotnet build` em `TaxPlanner.slnx` — 0 erros e 0 warnings.
2. Marcar Phase 3 como `✅ Completed`.

---

### Phase 4: Documentação

**Status**: ✅ Completed (2026-06-10)
**Objective**: manter `docs/` consistente com o novo contrato.
**Dependencies**: Phase 3

**Tasks**:

- [ ] T021 Atualizar `docs/integrations.md`
  - Adicionar `thumbnailUrl` e `thumbnailAlt` aos exemplos de
    `GET /api/posts` (listagem) e `GET /api/posts/{slug}` (detalhe)
  - Adicionar os 2 campos ao exemplo de `POST /api/admin/posts`
  - Documentar a regra de validação de scheme (http/https) na seção
    de "Tratamento de falhas" ou em uma nota inline
- [ ] T022 Atualizar `docs/modules/blog-admin.md`
  - Listar `thumbnailUrl`/`thumbnailAlt` na tabela "Endpoints" como
    campos opcionais
  - Atualizar o bloco "Contratos" (POST e PUT) com o JSON completo
  - Atualizar a seção "Comportamentos importantes" com a validação
    de scheme
  - **Acrescentar nota referenciando o ADR 0004** para o pipeline de
    upload (a capa **reutiliza** `BlogService.UploadImageAsync` e o
    endpoint `/api/posts/upload-image`; não há pipeline paralelo)
- [ ] T023 Atualizar `docs/modules/blog-publico.md`
  - Documentar que a API pública passa a devolver `thumbnailUrl`/
    `thumbnailAlt` em listagem e detalhe
- [ ] T024 Atualizar `docs/features/blog-post.md`
  - Mencionar a renderização condicional da hero via `ThumbnailUrl`
    (já está implementada, mas documentar)
- [ ] T025 Atualizar `docs/features/blog-listagem.md`
  - Mencionar que `BlogThumbnail` exibe a capa quando disponível
- [ ] T026 Atualizar `docs/runbooks/uploads-de-imagens.md` e
  `docs/runbooks/aplicar-migracoes.md`
  - Em `uploads-de-imagens.md`: adicionar uma linha no topo
    "Este pipeline atende **também** à imagem de capa das postagens;
    ver ADR 0004 para as regras (5 MB, extensões, nomenclatura)"
  - Em `aplicar-migracoes.md`: nada a alterar (a nova migration
    `AddPostThumbnail` segue o mesmo procedimento) — apenas garantir
    que está referenciada se a runbook listar migrations aplicadas

**After completing this phase**:

1. Marcar Phase 4 como `✅ Completed`.
2. Confirmar com `git status` que `docs/` está consistente.

---

## ✅ Master Checklist

### Phase 1: Modelo de dados + migration
- [x] T001 Adicionar `ThumbnailUrl`/`ThumbnailAlt` em `Post.cs`
- [x] T002 Gerar, drift-checkar e aplicar migration `AddPostThumbnail`

### Phase 2: Serviço + Controllers (admin + público)
- [x] T003 Estender `BlogService.CreateAsync`
- [x] T004 Estender `BlogService.UpdateAsync`
- [x] T005 Adicionar `IsValidExternalUrl` em `BlogService`
- [x] T006 Adicionar campos em `CreatePostRequest`
- [x] T007 Adicionar campos em `UpdatePostRequest`
- [x] T008 Atualizar `Create` no admin controller
- [x] T009 Atualizar `Update` no admin controller
- [x] T010 Listar `thumbnailUrl`/`thumbnailAlt` no `List` admin
- [x] T011 Devolver `thumbnailUrl`/`thumbnailAlt` no `Get` admin
- [x] T012 Devolver `thumbnailUrl`/`thumbnailAlt` no `List` público
- [x] T013 Devolver `thumbnailUrl`/`thumbnailAlt` no `Get` público
- [x] Build passa

### Phase 3: UI admin (NovoPost + EditarPost)
- [x] T014 Adicionar campos em `PostCreateModel`
- [x] T015 Inserir bloco "Imagem de capa" em `NovoPost.razor`
- [x] T016 `HandleCoverUpload` em `NovoPost.razor`
- [x] T017 `ClearCover` em `NovoPost.razor`
- [x] T018 Passar novos campos em `Save`/`SaveAndPublish` de `NovoPost.razor`
- [x] T019 Espelhar T014–T018 em `EditarPost.razor`
- [ ] T020 Smoke test manual
- [x] Build passa

### Phase 4: Documentação
- [x] T021 Atualizar `docs/integrations.md`
- [x] T022 Atualizar `docs/modules/blog-admin.md`
- [x] T023 Atualizar `docs/modules/blog-publico.md`
- [x] T024 Atualizar `docs/features/blog-post.md`
- [x] T025 Atualizar `docs/features/blog-listagem.md`
- [x] T026 Ajustar `docs/runbooks/uploads-de-imagens.md` se necessário

## Clarifications

Nenhuma clarificação pendente. Decisões aplicadas:

- **URL pública** é armazenada como está, sem espelhamento.
- **UX admin** usa campo dedicado no topo do formulário.

A validação de scheme (`http`/`https`) foi registrada na seção
"Decisões de design" e como AC de erro.

## Fora de escopo (decisões explícitas)

- **Enriquecer o feed RSS** com `<enclosure>` ou `<media:thumbnail>`:
  fica para uma tarefa futura se houver demanda de leitores de feed
  que renderizem capa. O contrato atual do RSS (`PostsController.Get`,
  linhas 93-136) **não** é alterado neste plano.
- **Validação de existência/HEAD request** para a URL pública:
  evitado por custo e latência no `Save`.
- **Download-and-mirror de URL externa** para o storage local:
  decisão consciente de não fazer (vide "Decisões de design" #1).
  Pode ser revisto se o ADR 0004 evoluir para um `IFileStorageService`.
- **Autorização dos endpoints**: o painel admin continua sem
  `[Authorize]` aplicado (dívida pré-existente, registrada em
  `docs/modules/blog-admin.md`). Não faz parte deste plano.
- **Migração de posts antigos**: colunas são nullable; posts
  existentes ficam com `null` e o `BlogThumbnail` exibe o placeholder
  editorial. Sem backfill automático.
