# Runbook: Aplicar migrações em novo ambiente

## Cenário

Provisionar o schema do banco de dados PostgreSQL em um ambiente novo
(local, dev compartilhado, staging ou produção futura) para o
`TaxPlanner.Server`.

## Pré-condições

- .NET 10 SDK instalado.
- `dotnet-ef` instalado globalmente (`dotnet tool install --global dotnet-ef`).
- Variável `ConnectionStrings:DefaultConnection` configurada (user secrets em
  dev, variáveis de ambiente em produção futura).
- Acesso de leitura/escrita ao banco Postgres de destino.

## Impacto e risco

- **Risco**: aplicar migrations cria/altera tabelas. Em produção, fazer
  sempre em janela de manutenção ou com migrações compatíveis com o schema
  anterior.
- **Impacto esperado**: criação das tabelas `AspNet*` (Identity) e `Posts`
  (blog) na primeira execução; em execuções subsequentes, aplicação
  incremental.

## Passos

1. Confirmar a connection string:
   ```bash
   dotnet user-secrets list --project TaxPlanner.Server
   ```
2. Aplicar as migrations:
   ```bash
   dotnet ef database update --project TaxPlanner.Server
   ```
3. Verificar se a tabela `Posts` foi criada:
   ```sql
   SELECT table_name FROM information_schema.tables
   WHERE table_schema = 'public' ORDER BY table_name;
   ```
   Deve listar: `AspNet*`, `__EFMigrationsHistory`, `Posts`.
4. (Opcional) Adicionar um usuário administrador via UI de registro do
   Identity ou via SQL direto.

## Verificação

- `__EFMigrationsHistory` contém todas as migrations esperadas:
  ```sql
  SELECT "MigrationId" FROM "__EFMigrationsHistory" ORDER BY "MigrationId";
  ```
- `dotnet run --project TaxPlanner.Server` inicia sem
  `InvalidOperationException` relacionado à connection string.
- `GET /api/posts` retorna `200 OK` com `posts: []`.

## Rollback

- Para reverter a última migration:
  ```bash
  dotnet ef database update <MigrationIdAnterior> --project TaxPlanner.Server
  ```
- Para reverter todas as migrations (apaga schema):
  ```bash
  dotnet ef database update 0 --project TaxPlanner.Server
  ```
  ⚠️ **Destrutivo** — faça backup antes em qualquer ambiente não-local.

## Referências

- [`../architecture.md`](../architecture.md) — mapa de módulos.
- [`../environments.md`](../environments.md) — segredos e configuração.
- `TaxPlanner.Server/Migrations/` — histórico de migrations.
