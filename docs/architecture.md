# Arquitetura — TaxPlanner

Este documento é a **fonte de verdade arquitetural** do projeto **TaxPlanner**,
composta por dois projetos .NET: `TaxPlanner.Wasm` (frontend PWA) e
`TaxPlanner.Server` (backend com UI administrativa e API).

## Visão geral do sistema

O **TaxPlanner** é uma aplicação dividida em cliente e servidor:

- **Cliente (`TaxPlanner.Wasm`)** — Single Page Application compilada para
  **WebAssembly** (Blazor WebAssembly), instalável como **PWA**. Roda
  inteiramente no navegador, sem servidor de aplicação no runtime. Carrega
  dados via chamadas `HttpClient` para a API.
- **Servidor (`TaxPlanner.Server`)** — Aplicação **ASP.NET Core 10** com:
  - **Blazor Server** (componentes interativos) para a área administrativa
    autenticada.
  - **API REST** sob `/api/*` consumida pelo Wasm e por integrações externas.
  - **ASP.NET Core Identity** com cookies para autenticação.
  - **EF Core + PostgreSQL** para persistência.
  - **Servidor de arquivos estáticos** para `wwwroot` (incluindo artefatos do
    Wasm publicados e uploads de imagens).

## Limites arquiteturais

| Limite                        | De                                    | Para                                |
| ----------------------------- | ------------------------------------- | ----------------------------------- |
| **Apresentação pública**      | Wasm (PWA no navegador)               | Usuário final                       |
| **Apresentação administrativa** | Blazor Server (componentes interativos) | Administrador (autenticado)         |
| **API pública**               | `PostsController`                     | Frontend Wasm / consumidores externos |
| **API administrativa**        | `AdminPostsController`                | Painel admin (autenticado)          |
| **Domínio de blog**           | `BlogService`                         | Controladores + DbContext           |
| **Persistência**              | `ApplicationDbContext` (EF Core)      | PostgreSQL                          |
| **Identidade**                | `IdentityDbContext<ApplicationUser>`  | PostgreSQL (tabelas `AspNet*`)      |

## Stack e principais bibliotecas

| Camada                  | Tecnologia                                                       |
| ----------------------- | ---------------------------------------------------------------- |
| Linguagem               | C# (componentes Razor + code-behind)                             |
| Runtime                 | .NET 10 (`net10.0`)                                              |
| Frontend                | Blazor WebAssembly 10.0.8 + MudBlazor 9.5 + PWA (service worker) |
| Backend                 | ASP.NET Core 10 + MudBlazor 9.5 + Blazor Server                  |
| ORM                     | EF Core 10.0.8 + `Npgsql.EntityFrameworkCore.PostgreSQL` 10.0.2  |
| Identidade              | `Microsoft.AspNetCore.Identity.EntityFrameworkCore` 10.0.8       |
| Markdown                | `Markdig` 1.2 (render no servidor)                               |
| Auth UI (estática)      | `Extensions.MudBlazor.StaticInput` 4.1                           |
| Fontes                  | Plus Jakarta Sans + Inter + JetBrains Mono (Google Fonts)        |

## Mapa de interação entre módulos

```
[Navegador Wasm] ──HTTP──> [/api/posts/*]      (público)
                          [/api/admin/posts/*] (autenticado)

[Admin Blazor Server] ──Server-side──> [BlogService] ──> [EF Core] ──> [PostgreSQL]
                                     [ApplicationDbContext (Identity)]
                                     [FileSystem: wwwroot/uploads/posts/]
```

### `TaxPlanner.Wasm` (frontend PWA)

- `Program.cs` — bootstrap do host WebAssembly.
- `App.razor` — raiz da aplicação, contém o `<Router>`.
- `_Imports.razor` — usings globais para todos os componentes Razor.
- `Pages/` — componentes roteáveis (`Home`, `Tabelas`, `AnaliseTributaria`,
  `Blog`, `Post`, `NotFound`).
- `Layout/MainLayout.razor` — layout raiz: AppBar translúcido, drawer mobile,
  footer de 4 colunas. Define a identidade visual editorial.
- `Theme/TaxPlannerTheme.cs` — `MudTheme` corporativo (paleta black/charcoal
  com verde-menta como destaque). Suporta modos claro e escuro.
- `Services/` — clientes HTTP e lógica de cálculo.
- `Models/` — DTOs e entidades de domínio.
- `wwwroot/` — ativos estáticos (HTML, CSS, ícones, manifest PWA).

### `TaxPlanner.Server` (backend)

- `Program.cs` — bootstrap do host, registro de serviços, pipeline HTTP, CORS.
- `Components/Account/` — componentes Razor de Identity (login, registro,
  passkeys, validação de auth state).
- `Controllers/PostsController.cs` — endpoints públicos de leitura e
  feed RSS do blog.
- `Controllers/AdminController.cs` — CRUD administrativo de posts e gestão
  de imagens.
- `Services/BlogService.cs` — domínio de blog: CRUD, render Markdown, gestão
  de imagens no disco, geração de slug, paginação.
- `Data/ApplicationDbContext.cs` — DbContext agregando `IdentityDbContext`
  e `DbSet<Post>`.
- `Data/ApplicationUser.cs` — entidade de usuário (Identity).
- `Models/Post.cs` — entidade de post (slug, título, summary, conteúdo
  Markdown/HTML, tags, datas, flag de publicação).
- `Migrations/` — migrações EF Core (Npgsql). Inicial: `CreatePostsTable`.
- `Theme/TaxPlannerTheme.cs` — `MudTheme` corporativo espelhado do
  frontend (cópia verbatim de `TaxPlanner.Wasm/Theme/TaxPlannerTheme.cs`).
  Cópia intencional: ver plano
  `docs/plans/20260610232407-align-server-design-system-plan.md`
  (decisão de design #1) para o rationale de não compartilhar o tema
  entre projetos.
- `wwwroot/` — estáticos do servidor, `css/app.css` (variáveis `--tp-*`,
  base, `#blazor-error-ui`, loading progress) e diretório de uploads.

## Invariantes e guias de mudança

1. **Identidade e autorização**:
   - `TaxPlanner.Wasm` é público; não consome endpoints `/api/admin/*`.
   - Endpoints administrativos **devem** estar sob autenticação por cookie
     (`IdentityConstants.ApplicationScheme`) antes de ir para produção.
   - `CORS` aberto é aceitável apenas em dev — restringir origens em prod.
2. **Persistência**:
   - Toda migração de schema deve gerar migration EF Core
     (`dotnet ef migrations add <Nome>`).
   - Conteúdo de `Post` é persistido em **duas representações**:
     `ContentMarkdown` (fonte) e `ContentHtml` (cache do `Markdig`).
     Atualizar uma implica recomputar a outra (responsabilidade do
     `BlogService`).
3. **Uploads**:
   - Tamanho máximo: 5 MB.
   - Extensões permitidas: `.jpg`, `.jpeg`, `.png`, `.gif`, `.webp`, `.svg`.
   - Nome do arquivo no disco: `Guid.NewGuid():N + extensão` (não confiar no
     nome enviado pelo cliente).
   - **Validação de path traversal** obrigatória (`..`, `/`, `\\`).
4. **API pública**:
   - Listagem e detalhe de posts são **somente leitura**.
   - Publicar/despublicar são exclusivos do painel admin.
5. **PWA e offline**:
   - `service-worker.js` desativa cache em desenvolvimento.
   - `service-worker.published.js` ativa cache offline em produção.
6. **Design System**:
   - Identidade visual é **editorial e corporativa** (base preta/carvão,
     verde-menta). Detalhes em [`../DESIGN.md`](../DESIGN.md).
   - Componentes UI: MudBlazor 9.5 com `TaxPlannerTheme` em
     `TaxPlanner.Wasm/Theme/TaxPlannerTheme.cs` (Wasm) **e** em
     `TaxPlanner.Server/Theme/TaxPlannerTheme.cs` (Server) — cópias
     verbatim deliberadamente separadas.
   - Variáveis CSS compartilhadas (`--tp-radius-*`, `--tp-shadow-*`,
     `--tp-transition`) vivem no `wwwroot/css/app.css` de cada projeto;
     o Server carrega o subconjunto mínimo (sem as classes `.tp-*`
     editoriais do Wasm).
   - **NavLink ativo do drawer admin** (Server) usa classe
     `tp-server-drawer` no `<MudDrawer>` para sobrescrever a cor
     padrão do MudBlazor (que ficaria preto-carvão sobre
     preto-carvão, contraste 1:1). A regra CSS vive em
     `TaxPlanner.Server/wwwroot/css/app.css` e usa
     `--mud-palette-secondary` (verde-menta) como destaque —
     adapta-se automaticamente a dark mode. O Wasm não precisa
     dessa classe porque tem seu próprio CSS de navlink
     (`.tp-nav-link-active`).
   - Idioma: **português do Brasil (pt-BR)**.
   - **Dark mode no Server**: toggle no AppBar, preferência persistida
     em `localStorage["tp-theme-dark"]`, com script anti-flash em
     `App.razor` para evitar FOUC no SSR. Implementado na Phase 2 do
     plano `20260610232407-align-server-design-system-plan.md`
     (concluído em 2026-06-10).

## Como adicionar uma nova feature (visão)

1. Crie a rota no Wasm (`Pages/NovaFeature.razor`).
2. Adicione endpoints públicos em `PostsController` ou novos controllers
   sob `/api/*`.
3. Para área autenticada, adicione componentes Blazor Server em
   `TaxPlanner.Server/Components/Pages`.
4. Se houver mudança de schema, gere migration EF Core.
5. Atualize esta documentação e o feature correspondente em
   `docs/features/`.
