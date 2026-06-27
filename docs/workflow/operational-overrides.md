# Operational Overrides do Projeto TaxPlanner

Este arquivo define a política operacional **específica do projeto**, sobreposta
aos defaults do plugin `psters-ai-workflow`. Sempre que houver conflito, a
precedência é:

1. **Instrução explícita do usuário** (durante a conversa atual).
2. **Override do projeto** (este arquivo).
3. **Default do plugin** (regras base do `pwf-*`).

## Política YAML

```yaml
project: TaxPlanner
idioma_documentacao: pt-BR
documentos_pastas:
  - docs/infrastructure.md
  - docs/architecture.md
  - docs/integrations.md
  - docs/environments.md
  - docs/glossary.md
  - docs/workflow/operational-overrides.md
  - docs/runbooks/README.md
  - docs/modules/README.md
  - docs/features/README.md
  - docs/lambdas/README.md
  - docs/decisions/README.md
politica_arquivos:
  nao_destruir: true              # nunca deletar docs existentes
  nao_sobrescrever_se_nao_vazio: true
  avisar_antes_sobrescrever: true
politica_comandos:
  dotnet: permitir                # build, restore, run, publish, test
  git: somente_leitura_por_default
politica_revisoes:
  revisar_apos_implementacao: true
  sincronizar_plan: true
  atualizar_glossario_em_mudanca_dominio: true
```

## Notas

- Omitir uma política neste arquivo significa **aderir ao default do plugin**.
- Mudanças neste arquivo exigem commit dedicado e justificativa no `decisions/`.
- Este arquivo é carregado automaticamente pelos skills `pwf-*` no início da
  execução.

## Referências

- [`../README.md`](../README.md) — índice geral da documentação.
- Plugin `psters-ai-workflow` — regras base.
