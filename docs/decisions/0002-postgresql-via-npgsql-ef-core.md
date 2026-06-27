# ADR 0002 — PostgreSQL como banco de dados (via Npgsql + EF Core)

- **Data**: 2026-06-10
- **Status**: Aceito
- **Contexto**: o backend precisa de um banco relacional para persistir
  posts do blog e dados de Identity (usuários, roles, claims).
- **Decisão**:
  - **PostgreSQL** como SGBD.
  - **EF Core 10.0.8** como ORM.
  - **Npgsql.EntityFrameworkCore.PostgreSQL 10.0.2** como provider.
  - Connection string via chave `DefaultConnection` em
    `appsettings.json` / user secrets / variáveis de ambiente.
- **Consequências**:
  - **Positivas**: PostgreSQL é amplamente suportado, robusto e tem
    bom desempenho para o workload atual; EF Core acelera o
    desenvolvimento com migrations code-first; Npgsql é o provider
    oficial e estável.
  - **Negativas**: o desenvolvedor precisa de um Postgres local
    rodando para dev (via Docker, service ou instalação nativa);
    exige migration explícita em ambientes novos.
- **Alternativas consideradas**:
  - **SQLite** — simplificaria o dev local, mas não é recomendado
    para produção com EF Core Identity em escala.
  - **SQL Server** — exige licença ou Windows-only; menos
    multiplataforma que Postgres.
- **Referências**:
  - [`../infrastructure.md`](../infrastructure.md)
  - [`../integrations.md`](../integrations.md) (seção PostgreSQL)
  - [`../runbooks/aplicar-migracoes.md`](../runbooks/aplicar-migracoes.md)
  - `TaxPlanner.Server/TaxPlanner.Server.csproj`
  - `TaxPlanner.Server/Program.cs` (`AddDbContext<ApplicationDbContext>`)
  - `TaxPlanner.Server/Migrations/20260608185223_CreatePostsTable.cs`
