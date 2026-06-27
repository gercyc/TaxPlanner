# Ambientes — TaxPlanner

Matriz de ambientes, configuração, segredos, fluxo de deploy e observabilidade
para o projeto **TaxPlanner**.

## Matriz de ambientes

| Ambiente       | Propósito                                | Hospedagem                                  | Banco de dados                  |
| -------------- | ---------------------------------------- | ------------------------------------------- | ------------------------------- |
| **Local (dev)** | Desenvolvimento e debug                  | `dotnet run` (Wasm em `localhost:7053`, Server via profile) | Postgres local ou containerizado |
| **Dev (compartilhado)** | Integração entre features             | (não provisionado no momento)              | (não provisionado)             |
| **Staging**    | Validação pré-produção                   | (não provisionado no momento)              | (não provisionado)             |
| **Produção**   | Operação real                            | (não provisionado no momento)              | (não provisionado)             |

> Os ambientes **Dev/Staging/Produção** ainda não estão provisionados na
> infraestrutura atual. A operação ocorre totalmente em ambiente local.

## Configuração e segredos

### Fontes de configuração

| Origem                | Uso                                                                  |
| --------------------- | -------------------------------------------------------------------- |
| `appsettings.json`    | Configuração base compartilhada.                                     |
| `appsettings.Development.json` | Overrides por ambiente de execução.                         |
| **User Secrets**      | Segredos locais em desenvolvimento (recomendado para `DefaultConnection`). |
| **Variáveis de ambiente** | Para deploys em containers / hosts (futuro).                     |

`UserSecretsId` do `TaxPlanner.Server`:

```
aspnet-TaxPlanner_Server-72eef20c-6443-4820-8e15-490dfea4b366
```

Comandos úteis:

```bash
# Inicializar user secrets (uma vez)
dotnet user-secrets init --project TaxPlanner.Server

# Definir a connection string do Postgres
dotnet user-secrets set "ConnectionStrings:DefaultConnection" \
  "Host=localhost;Port=5432;Database=taxplanner;Username=postgres;Password=***"

# Listar segredos
dotnet user-secrets list --project TaxPlanner.Server
```

### Fronteiras de segredo

- **Nunca** commitar connection strings, senhas ou tokens.
- Toda credencial deve entrar via **user secrets** (dev) ou **variáveis de
  ambiente** (produção futura).
- O `.gitignore` da raiz já deve cobrir `appsettings.*.Local.json` e o
  diretório de saída `bin/obj` (verificar antes de cada commit).

## Fluxo de deploy (local)

### Backend (`TaxPlanner.Server`)

1. `dotnet restore`
2. `dotnet build`
3. Aplicar migrações:
   ```bash
   dotnet ef database update --project TaxPlanner.Server
   ```
4. Executar:
   ```bash
   dotnet run --project TaxPlanner.Server
   ```

### Frontend (`TaxPlanner.Wasm`)

1. `dotnet restore`
2. `dotnet build`
3. Executar (HTTPS em `localhost:7053`):
   ```bash
   dotnet run --project TaxPlanner.Wasm
   ```
4. Publicar:
   ```bash
   dotnet publish -c Release
   ```

### Publicar frontend + backend juntos

- O `TaxPlanner.Server` pode servir o `wwwroot/_framework` do Wasm já
  publicado. Em produção, publique o Wasm e copie (ou monte) o conteúdo em
  `TaxPlanner.Server/wwwroot/_framework/`.
- O `Program.cs` já chama `app.MapStaticAssets()` para servir estáticos.

## PWA e service worker

| Modo          | Arquivo                             | Cache offline |
| ------------- | ----------------------------------- | ------------- |
| Desenvolvimento | `wwwroot/service-worker.js`       | Desativado    |
| Produção      | `wwwroot/service-worker.published.js` | Ativo (estratégia offline-first) |

Manifesto PWA: `TaxPlanner.Wasm/wwwroot/manifest.webmanifest` — define nome,
ícones, cores e modo de exibição.

## Observabilidade

- **Logs**: configuração padrão do ASP.NET Core, ajustável em
  `appsettings.json` (`Logging:*`).
- **Health checks**: não configurados no momento (recomendado para
  ambientes futuros).
- **Tracing / métricas**: não configurados.
- **Erros em desenvolvimento**: `app.UseMigrationsEndPoint()` habilitado.
- **Erros em produção**: `app.UseExceptionHandler("/Error", ...)` + HSTS de
  30 dias (`app.UseHsts()`).

## Acesso por ambiente

| Ambiente | Acesso à rede              | HTTPS      | Quem acessa          |
| -------- | -------------------------- | ---------- | -------------------- |
| Local    | `localhost` / `127.0.0.1`  | Sim (dev cert) | Desenvolvedor    |
| Dev      | (não provisionado)         | —          | —                    |
| Staging  | (não provisionado)         | —          | —                    |
| Produção | (não provisionado)         | —          | —                    |

## Restrições críticas por ambiente

1. **CORS** está `AllowAnyOrigin` no `Program.cs` — **aceitável só em
   desenvolvimento**. Restringir antes de subir para qualquer ambiente
   compartilhado.
2. **HSTS** de 30 dias em produção.
3. **Uploads** em `wwwroot/uploads/posts/` exigem volume persistente em
   ambientes efêmeros.
4. **Migrações** devem ser aplicadas **antes** do primeiro start em
   ambiente novo.
