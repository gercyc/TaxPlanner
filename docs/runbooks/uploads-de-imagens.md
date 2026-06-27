# Runbook: Uploads de imagens

> **Escopo**: este pipeline atende **também** à imagem de capa
> (`ThumbnailUrl`/`ThumbnailAlt`) das postagens. A capa pode ser um
> upload local (reusa o `POST /api/posts/upload-image` e cai no mesmo
> `wwwroot/uploads/posts/`) **ou** uma URL pública `http(s)://` que o
> admin cola no formulário — nesse caso não há arquivo em disco, apenas
> a URL persistida no banco. Ver
> [ADR 0004](../decisions/0004-uploads-de-imagem-em-disco-wwwroot-uploads-posts.md)
> para as regras de upload (5 MB, extensões, nomenclatura
> `Guid.NewGuid():N`) e
> [`blog-admin.md`](../modules/blog-admin.md) para a UX de capa.

## Cenário

Inspecionar, limpar ou recuperar espaço em disco ocupado por imagens
enviadas pelo painel administrativo (`/api/admin/posts/images/*` e
`/api/posts/upload-image`).

## Pré-condições

- Acesso ao servidor onde o `TaxPlanner.Server` está rodando.
- Permissão de leitura/escrita em `TaxPlanner.Server/wwwroot/uploads/posts/`.

## Impacto e risco

- **Risco**: deletar imagens referenciadas por posts publicados quebra a
  renderização do conteúdo. Sempre validar referências antes de remover.
- **Impacto esperado**: lista via `GET /api/admin/posts/images` (somente admin).

## Passos

### Listar imagens (via API)

```bash
curl -X GET http://localhost:5000/api/admin/posts/images
```

Resposta:

```json
[
  { "fileName": "ab12...ef.png", "url": "/uploads/posts/ab12...ef.png", "size": 12345 }
]
```

### Inspecionar no disco

- Localização: `TaxPlanner.Server/wwwroot/uploads/posts/`.
- Padrão de nome: `Guid.NewGuid():N` + extensão válida (`.jpg`, `.jpeg`,
  `.png`, `.gif`, `.webp`, `.svg`).
- Limite por arquivo: 5 MB.

### Remover uma imagem (via API)

```bash
curl -X DELETE http://localhost:5000/api/admin/posts/images/ab12...ef.png
```

A API já valida path traversal (`..`, `/`, `\\`).

### Remover manualmente (emergência)

```bash
# Backup antes
cp -r wwwroot/uploads/posts /tmp/uploads-backup-$(date +%Y%m%d)

# Remoção segura (somente após conferir referências nos posts)
rm wwwroot/uploads/posts/<fileName>
```

## Verificação

- Após remoção, `GET /api/admin/posts/images` não retorna o arquivo.
- Acessar a URL pública (`https://.../uploads/posts/<fileName>`) deve
  retornar `404 Not Found`.

## Rollback

- Restaurar do backup:
  ```bash
  cp -r /tmp/uploads-backup-YYYYMMDD/* wwwroot/uploads/posts/
  ```

## Referências

- [`../integrations.md`](../integrations.md) — contrato da API de upload e
  exclusão de imagens.
- `TaxPlanner.Server/Controllers/AdminController.cs` — endpoints.
- `TaxPlanner.Server/Services/BlogService.cs` — lógica de storage.
