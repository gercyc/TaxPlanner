# ADR 0004 — Uploads de imagem em disco (`wwwroot/uploads/posts/`)

- **Data**: 2026-06-10
- **Status**: Aceito
- **Contexto**: o painel admin precisa enviar imagens (logos, gráficos
  tributários) que serão embutidas em posts Markdown.
- **Decisão**:
  - Uploads são gravados em **`TaxPlanner.Server/wwwroot/uploads/posts/`**.
  - O nome do arquivo no disco é **`Guid.NewGuid():N + extensão`**,
    descartando o nome enviado pelo cliente.
  - Limite de **5 MB** por arquivo.
  - Extensões permitidas: **`jpg, jpeg, png, gif, webp, svg`**.
  - Validação de **path traversal** (`..`, `/`, `\\`) na exclusão.
  - Servidos via `app.MapStaticAssets()` (estático).
- **Consequências**:
  - **Positivas**: implementação simples, sem dependência de storage
    externo; URLs diretas via `MapStaticAssets`.
  - **Negativas**: **não escala** horizontalmente (cada instância
    tem seu próprio disco); em containers efêmeros os arquivos se
    perdem se não houver volume persistente; sem CDN.
- **Alternativas consideradas**:
  - **S3 / Azure Blob / GCS** — robustos e escaláveis, mas
    acrescentam dependências e custo operacional; apropriados para
    produção futura.
  - **Banco como blob (bytea)** — acoplaria storage ao banco,
    pior para performance e backup.
- **Plano de migração (futuro)**: substituir o acesso ao
  `Path.Combine(...wwwroot/uploads/posts/)` por um `IFileStorageService`
  com implementação S3/blob; manter a interface atual.
- **Referências**:
  - [`../runbooks/uploads-de-imagens.md`](../runbooks/uploads-de-imagens.md)
  - `TaxPlanner.Server/Controllers/AdminController.cs`
  - `TaxPlanner.Server/Controllers/PostsController.cs`
  - `TaxPlanner.Server/Services/BlogService.cs`
