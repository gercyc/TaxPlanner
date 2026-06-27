# Runbook: Publicar nova versão

## Cenário

Publicar uma nova versão do `TaxPlanner.Wasm` (frontend PWA) e/ou
`TaxPlanner.Server` (backend) em ambiente local de produção simulada.

## Pré-condições

- .NET 10 SDK instalado.
- Acesso à connection string do Postgres de destino.
- Build local sem erros e sem warnings (ver
  `docs/architecture.md` e `CLAUDE.md`).

## Impacto e risco

- **Risco**: downtime durante reinício do servidor e aplicação de
  migrations.
- **Impacto esperado**: usuários do frontend verão a nova versão após
  reload (service worker pode segurar versões antigas até atualização).

## Passos

### 1. Build

```bash
dotnet restore
dotnet build -c Release
```

### 2. Publicar backend

```bash
dotnet publish TaxPlanner.Server -c Release -o ./publish/server
```

### 3. Publicar frontend

```bash
dotnet publish TaxPlanner.Wasm -c Release -o ./publish/wasm
```

### 4. Aplicar migrations (se houver)

```bash
dotnet ef database update --project TaxPlanner.Server
```

### 5. Implantar

- Substituir binários do servidor em produção futura.
- Para o Wasm, copiar `./publish/wasm/wwwroot/_framework` para
  `TaxPlanner.Server/wwwroot/_framework` (ou para o host estático que
  serve o frontend).

### 6. Reiniciar o servidor

```bash
# Em systemd / Docker / IIS — conforme o ambiente
systemctl restart taxplanner-server
# ou
docker restart taxplanner-server
# ou
# recycle do app pool no IIS
```

## Verificação

- `GET /api/posts` retorna `200 OK`.
- `GET /` (Wasm) carrega a nova versão (inspecionar
  `?v=...` no nome dos artefatos em `_framework/`).
- Service worker (`service-worker.published.js`) ativo em produção.
- Health check da app sobe sem exceções nos logs.

## Rollback

- Manter artefatos da versão anterior em `./publish/previous/`.
- Reverter binários do servidor.
- Reverter arquivos estáticos do Wasm.
- Para o banco, ver
  [`aplicar-migracoes.md`](./aplicar-migracoes.md) — seção Rollback.

## Referências

- [`../architecture.md`](../architecture.md)
- [`../environments.md`](../environments.md)
- `CLAUDE.md` — comandos essenciais (`dotnet build`, `dotnet run`,
  `dotnet publish`).
