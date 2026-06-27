# Feature: Home (landing page)

- **Rota**: `/`
- **Componente**: `TaxPlanner.Wasm/Pages/Home.razor`
- **Status**: Estável

## Propósito

Página de entrada do **TaxPlanner**. Apresenta o produto, explica o fluxo
de uso em 4 passos e destaca as principais features (Análise Tributária,
Tabelas, Privacidade). Serve como ponto de partida para a ação principal
do usuário: iniciar uma simulação.

## Pontos de entrada

- `<MudButton Href="/analise-tributaria">` — CTA principal "Começar
  Simulação".
- Cards secundários linkando para `/analise-tributaria` e `/tabelas`.
- Cards do footer e AppBar também apontam para Home (`/`).

## Integrações

- **Nenhuma chamada HTTP** — a Home é totalmente estática.
- Não consome serviços de cálculo nem API do blog.

## Estado e ciclo de vida

- Sem `@code` complexo: markup puro + `MudGrid` / `MudCard`.
- Sem parâmetros de rota, sem `OnInitialized` assíncrono.

## Acessibilidade

- Hierarquia semântica com `Typo.h3`, `Typo.h5`, `Typo.body1`.
- `MudButton` usado com `StartIcon` para reforço visual.
- Alerta final (`MudAlert Severity.Info`) com texto em `strong` para
  destacar aviso legal.

## Design

- **Identidade visual**: editorial e corporativa — ver
  [`../../DESIGN.md`](../../DESIGN.md).
- Cores: `Color.Primary` (preto-carvão) e `Color.Secondary` (verde-menta).
- Ícones Material: `AccountBalance`, `Calculate`, `TableChart`, `Security`.

## Referências

- `TaxPlanner.Wasm/Pages/Home.razor`
- `TaxPlanner.Wasm/Layout/MainLayout.razor` (appbar + footer)
- `DESIGN.md`
