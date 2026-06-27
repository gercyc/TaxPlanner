# Lambdas — TaxPlanner

Esta seção é reservada para documentar **AWS Lambda functions** (ou
equivalentes serverless) do projeto.

> No estado atual do repositório, **não há Lambdas** provisionadas. Toda a
> lógica de backend roda no `TaxPlanner.Server` (ASP.NET Core hospedado).
>
> Caso sejam adicionadas Lambdas no futuro (por exemplo, processamento
> assíncrono de imagens, envio de e-mails, webhooks), criar um arquivo
> `<nome-da-lambda>.md` neste diretório seguindo o esqueleto abaixo.

## Esqueleto de documentação de Lambda (futuro)

1. **Propósito** — o que a Lambda faz e qual trigger a invoca.
2. **Handler** — assinatura, linguagem e runtime.
3. **Entradas / Saídas** — evento de entrada (SQS, SNS, API Gateway, etc.) e
   contrato de saída.
4. **Variáveis de ambiente** — chaves e origem dos segredos.
5. **Idempotência** — chave de idempotência e armazenamento de estado.
6. **Cold start e timeout** — duração esperada, ajuste de memória.
7. **Observabilidade** — CloudWatch logs, métricas, alarmes.
8. **Deploy** — pipeline, IaC (CDK/Terraform), versionamento.
9. **Referências** — código, diagramas e runbooks.
