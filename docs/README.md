# Documentação do Projeto TaxPlanner

Este diretório concentra toda a documentação do repositório **TaxPlanner** — fonte
de verdade operacional, arquitetural e de processos. Todo o conteúdo é mantido em
**português (pt-BR)**.

## Estrutura geral

| Seção                                | Conteúdo                                                                          |
| ------------------------------------ | --------------------------------------------------------------------------------- |
| [`infrastructure.md`](./infrastructure.md)     | Provedor, topologia de runtime, deploy, restrições operacionais críticas.        |
| [`architecture.md`](./architecture.md)         | Visão arquitetural, stack, mapa de módulos, invariantes e guias de mudança.      |
| [`integrations.md`](./integrations.md)         | Catálogo de integrações internas/externas, autenticação e contratos.              |
| [`environments.md`](./environments.md)         | Matriz de ambientes, segredos, fluxo de deploy e observabilidade.                 |
| [`glossary.md`](./glossary.md)                 | Termos canônicos do domínio, siglas e desambiguação.                             |
| [`workflow/operational-overrides.md`](./workflow/operational-overrides.md) | Política operacional do projeto (precedência e overrides). |
| [`modules/`](./modules/README.md)              | Documentação por módulo de backend (`TaxPlanner.Server`).                        |
| [`features/`](./features/README.md)            | Documentação por feature de frontend (`TaxPlanner.Wasm`).                         |
| [`lambdas/`](./lambdas/README.md)              | Lambdas serverless (vazio — não há Lambdas no projeto).                          |
| [`runbooks/`](./runbooks/README.md)            | Procedimentos operacionais e troubleshooting.                                    |
| [`decisions/`](./decisions/README.md)          | ADRs (Architecture Decision Records).                                            |
| [`brainstorms/`](./brainstorms/README.md)      | Saídas brutas de discovery e ideação.                                            |
| [`plans/`](./plans/README.md)                  | Planos de execução prontos para implementação.                                    |
| [`work-plans/`](./work-plans/README.md)        | Planos de trabalho em andamento com log de execução.                             |
| [`solutions/`](./solutions/README.md)          | Soluções reutilizáveis, lições aprendidas e padrões.                             |

## Como usar

1. Antes de alterar código, consulte `architecture.md` e o módulo/feature afetado.
2. Ao integrar com um sistema externo, atualize `integrations.md`.
3. Ao introduzir um novo ambiente ou segredo, atualize `environments.md`.
4. Decisões arquiteturais relevantes devem virar ADR em `decisions/`.
5. Problemas recorrentes devem virar runbook em `runbooks/`.

## Convenção de idioma

Todos os documentos novos e atualizações devem ser escritos em **português do
Brasil (pt-BR)**. Termos técnicos e identificadores de código permanecem em
inglês.
