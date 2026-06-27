# Infraestrutura — TaxPlanner

Este documento é a **fonte de verdade** da infraestrutura do projeto
**TaxPlanner** e dos seus dois componentes: o frontend Blazor WebAssembly
(`TaxPlanner.Wasm`) e o backend ASP.NET Core com componentes Blazor Server e API
(`TaxPlanner.Server`).

## Modelo de provedor e ambiente

- **Provedor atual**: execução local em máquina de desenvolvimento (Windows /
  Linux / macOS com .NET 10 SDK). Não há dependência obrigatória de cloud
  específica neste momento.
- **Modelo de execução**:
  - `TaxPlanner.Wasm` — **estático + WebAssembly** servido por qualquer
    host estático ou pelo `TaxPlanner.Server` em produção.
  - `TaxPlanner.Server` — **ASP.NET Core 10** com Blazor Server (componentes
    interativos) e API REST sob `/api/*`.
- **Banco de dados**: **PostgreSQL** via `Npgsql.EntityFrameworkCore.PostgreSQL`
  (provider `Npgsql.EntityFrameworkCore.PostgreSQL 10.0.2`). Configurado pela
  connection string `DefaultConnection`.

## Topologia de runtime

```
┌────────────────────────────────┐
│ Navegador (cliente)            │
│  - Blazor WebAssembly (PWA)    │
│  - Service Worker (offline)    │
└──────────────┬─────────────────┘
               │ HTTPS
               ▼
┌────────────────────────────────┐
│ TaxPlanner.Server              │
│  - Blazor Server (UI admin)    │
│  - API REST (Controllers)      │
│  - Identity (cookie auth)      │
│  - Static files (wwwroot)      │
└──────────────┬─────────────────┘
               │ Npgsql
               ▼
┌────────────────────────────────┐
│ PostgreSQL                     │
│  - Tabelas Identity (AspNet*)  │
│  - Tabela Posts                │
└────────────────────────────────┘
```

## Serviços centrais

| Serviço                | Função                                              | Projeto            |
| ---------------------- | --------------------------------------------------- | ------------------ |
| `BlogService`          | CRUD de posts + render de Markdown + gestão de imagens | `TaxPlanner.Server` |
| `PostsController`      | Endpoints públicos do blog (`/api/posts/*`)         | `TaxPlanner.Server` |
| `AdminPostsController` | Endpoints administrativos (`/api/admin/posts/*`)    | `TaxPlanner.Server` |
| `ApplicationDbContext` | DbContext EF Core (Identity + `Posts`)              | `TaxPlanner.Server` |
| `MudBlazor`            | Design system + componentes UI                      | ambos os projetos  |

## Modelo de deploy

- **Build**:
  - Frontend: `dotnet build TaxPlanner.Wasm` — gera artefatos
    `wwwroot/_framework/*` prontos para hosting estático.
  - Backend: `dotnet build TaxPlanner.Server` — produz DLL ASP.NET Core.
- **Publicação**:
  - `dotnet publish -c Release` em cada projeto.
  - O `TaxPlanner.Server` pode hospedar o `wwwroot/_framework` do Wasm já
    publicado (ver configuração de static assets no `Program.cs`).
- **PWA**:
  - `service-worker.js` em desenvolvimento (cache desativado).
  - `service-worker.published.js` em produção (cache offline).
  - Manifesto PWA em `TaxPlanner.Wasm/wwwroot/manifest.webmanifest`.

## Restrições operacionais críticas

1. **Conexão com Postgres** é obrigatória para o `TaxPlanner.Server`. Sem
   `DefaultConnection` válida, a aplicação aborta a inicialização.
2. **Migrações EF Core** devem ser aplicadas antes do primeiro start em
   ambiente novo. Localizar em
   `TaxPlanner.Server/Migrations/<timestamp>_<nome>.cs`.
3. **Uploads de imagem** são gravados em
   `TaxPlanner.Server/wwwroot/uploads/posts/`. Em ambientes efêmeros
   (containers sem volume), esses arquivos serão perdidos — monte um volume
   persistente ou migre para storage de objeto.
4. **CORS** está configurado como `AllowAnyOrigin / AllowAnyMethod /
   AllowAnyHeader` no `Program.cs` — **restringir antes de expor para a
   internet**.
5. **HTTPS** é redirecionado em produção (`app.UseHttpsRedirection`).

## Referências de fonte de verdade

- `TaxPlanner.slnx` — solution file na raiz.
- `TaxPlanner.Server/Program.cs` — bootstrap do host, DI, pipeline, CORS.
- `TaxPlanner.Server/appsettings.json` (e `appsettings.Development.json`) —
  connection strings e logging.
- `TaxPlanner.Server/Migrations/` — histórico de schema EF Core.
- `TaxPlanner.Server/TaxPlanner.Server.csproj` — versões e pacotes.
