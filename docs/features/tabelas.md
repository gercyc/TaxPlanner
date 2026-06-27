# Feature: Tabelas da Receita Federal

- **Rota**: `/tabelas`
- **Componente**: `TaxPlanner.Wasm/Pages/Tabelas.razor`
- **Status**: Estável

## Propósito

Exibe as **tabelas oficiais** de INSS, IRRF mensal e IR anual utilizadas
nos cálculos do simulador. Quando o ano selecionado ainda não tem tabela
publicada pela Receita Federal, o componente mostra um alerta e utiliza
a última tabela disponível como estimativa.

## Pontos de entrada

- AppBar (desktop e drawer mobile): `Href="/tabelas"`.
- Footer: link "Tabelas IR / INSS".

## Componente e dependências

- `Pages/Tabelas.razor`:
  - `inject CalculadoraIRService Calculadora` — provê `ObterTabelas(ano)`.
  - `Models.TabelasImposto` — DTO com INSS, IRRF mensal, IR anual,
    tetos e (quando aplicável) regras de redução da Lei 15.270/2025.
- Helpers locais: `FormatarMoeda(decimal)`, `FormatarPercentual(decimal)`
  — cultura `pt-BR`.

## Estado

```csharp
private int anoSelecionado = DateTime.Now.Year - 1;
private TabelasImposto? tabelas;
```

- `OnInitialized` chama `CarregarTabelas()`.
- `@bind-Value:after="CarregarTabelas"` no `MudNumericField` recarrega
  sempre que o ano muda.

## Integrações

- **Sem chamadas HTTP** — todas as tabelas estão embutidas no
  `CalculadoraIRService` (lado cliente).
- Quando `TabelaEstimada == true`, exibe `MudAlert Severity.Warning`.

## Acessibilidade

- Tabelas com `<MudTable>` em modo responsivo (`Breakpoint.Sm`).
- Cabeçalhos de coluna com alinhamento à direita para valores monetários.
- Texto de "Última faixa" / "Acima de" lido por leitores de tela.

## Referências

- `TaxPlanner.Wasm/Pages/Tabelas.razor`
- `TaxPlanner.Wasm/Services/CalculadoraIRService.cs`
- `TaxPlanner.Wasm/Models/TabelasImposto.cs`
