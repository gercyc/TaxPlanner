# Runbooks — TaxPlanner

Catálogo de runbooks (procedimentos operacionais) do projeto **TaxPlanner**.
Cada runbook documenta um cenário de incidente, manutenção ou operação
recorrente, em formato reproduzível.

## Índice de runbooks

| Cenário                                              | Arquivo                                              | Status      |
| ---------------------------------------------------- | ---------------------------------------------------- | ----------- |
| Aplicar migrações do banco em novo ambiente          | [`aplicar-migracoes.md`](./aplicar-migracoes.md)     | Disponível  |
| Limpar / inspecionar uploads de imagens              | [`uploads-de-imagens.md`](./uploads-de-imagens.md)   | Disponível  |
| Publicar nova versão do frontend e do backend        | [`publicar-nova-versao.md`](./publicar-nova-versao.md) | Disponível |

## Modelo de runbook

Cada runbook segue, quando aplicável, as seções abaixo (cabeçalhos estáveis
para permitir indexação):

1. **Cenário** — o que está acontecendo ou precisa ser feito.
2. **Pré-condições** — permissões, acessos e estado esperado do sistema.
3. **Impacto e risco** — o que pode dar errado durante a execução.
4. **Passos** — sequência numerada de comandos e verificações.
5. **Verificação** — como confirmar que o procedimento foi bem-sucedido.
6. **Rollback** — como desfazer caso algo dê errado.
7. **Referências** — links para código, configs e documentação relacionada.

## Ownership e escalonamento

- **Owner operacional atual**: time de desenvolvimento (não há SRE/ops
  dedicado no momento).
- **Escalonamento**: abrir issue no repositório com o rótulo `runbook` e
  descrição do incidente.

## Como adicionar um novo runbook

1. Criar arquivo em `docs/runbooks/<slug-kebab-case>.md`.
2. Adicionar linha na tabela de índice acima.
3. Usar o modelo descrito neste README.
4. Revisar com o time antes de promover para "Pronto".
