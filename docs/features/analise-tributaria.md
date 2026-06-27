# Feature: Análise Tributária (simulador de IR)

- **Rota**: `/analise-tributaria`
- **Componente**: `TaxPlanner.Wasm/Pages/AnaliseTributaria.razor`
- **Status**: Estável — feature central do produto

## Propósito

Permite ao usuário **simular o Imposto de Renda** pessoa física informando
períodos salariais brutos (com datas de início/fim). O cálculo considera:

- Tabela progressiva de INSS (CLT).
- IRRF mensal (tabela vigente).
- IR anual (tabela progressiva).
- 13º salário opcional com tributação exclusiva na fonte.
- Reduções previstas em legislação (ex.: Lei 15.270/2025, quando
  aplicável ao ano selecionado).

## Pontos de entrada

- AppBar (desktop e drawer mobile).
- Home: CTA principal "Começar Simulação".
- Home: card "Análise Tributária".
- Footer: link "Análise Tributária".

## Componentes e dependências

- `Pages/AnaliseTributaria.razor` — UI completa.
- `inject CalculadoraIRService Calculadora` — regra de cálculo.
- `Models.PeriodoSalarial` — período com `DataInicio`, `DataFim` (DateOnly)
  e `SalarioBruto` (decimal).
- `Models.ResultadoIR` — saída do cálculo (ver estrutura abaixo).
- Estilos locais via bloco `<style>` para overflow do `MudTable` em
  `.periodos-table`.

## Fluxo da UI (3 passos)

1. **Configurações** — ano-calendário e toggle "Incluir 13º salário".
2. **Períodos Salariais** — tabela editável com `MudDatePicker` (mês/ano)
   e `MudNumericField` para salário. Permite adicionar/remover períodos.
3. **Calcular** — dispara `Calculadora.Calcular(...)` e renderiza o
   resultado.

### Validações

- `DataFim >= DataInicio`.
- `SalarioBruto > 0` em todos os períodos.
- **Sem sobreposição** entre períodos.

## Modelo de saída (`ResultadoIR`)

Campos principais exibidos no resultado:

- `RendimentoBrutoAnual` (decimal)
- `InssAnual`, `IrrfRetidoAnual` (decimal)
- `BaseDeCaculoAnual`, `IrDevidoAnual` (decimal)
- `ResultadoFinal` (decimal; **negativo = restituir**, **positivo = pagar**)
- `SalarioDecimoTerceiro`, `InssDecimoTerceiro`,
  `BaseCaculoDecimoTerceiro`, `IrrfRetidoDecimoTerceiro` (somente quando
  `IncluiDecimo`)
- `DetalhesMensais` — lista por mês/ano com salário bruto, INSS, base de
  cálculo e IRRF retido.

## Estado

```csharp
private int anoCalendario = DateTime.Now.Year - 1;
private bool incluirDecimo = true;
private List<PeriodoSalarial> periodos = [];
private ResultadoIR? resultado;
private string erroPeriodos = string.Empty;
```

- Toda mutação nos períodos invalida `resultado` (`= null`) para forçar
  recálculo.
- O cálculo é **síncrono** (roda 100% no navegador — sem rede).

## Integrações

- Nenhuma chamada HTTP. Cálculo local via `CalculadoraIRService`.
- `Calculadora.TabelaEstimada(ano)` é consultada para exibir alerta
  amarelo quando a tabela do ano ainda não foi publicada.

## Acessibilidade

- `MudTable` com `Breakpoint.Sm` para colapsar em cards no mobile.
- Labels explícitas nos campos (`HelperText` no `MudNumericField`).
- `MudChip` para "X meses" no período, com contraste via `Color.Info`.
- `MudExpansionPanels MultiExpansion="false"` no detalhamento.

## Referências

- `TaxPlanner.Wasm/Pages/AnaliseTributaria.razor`
- `TaxPlanner.Wasm/Services/CalculadoraIRService.cs`
- `TaxPlanner.Wasm/Models/PeriodoSalarial.cs`
- `TaxPlanner.Wasm/Models/ResultadoIR.cs`
- `DESIGN.md` (paleta, espaçamentos, raios)
