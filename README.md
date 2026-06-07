# TaxPlanner

Simulador de Imposto de Renda Pessoa Física (IRPF) que roda inteiramente no navegador. Informe seus salários ao longo do ano — incluindo promoções e reajustes — e descubra se você terá restituição ou imposto a pagar.

## Demo

Acesse a versão online em: **https://tax-planner.gercy.workers.dev/**

## Funcionalidades

- **Análise Tributária** — simule o IR anual com suporte a múltiplos períodos salariais (promoções, reajustes)
- **13º salário** — calcule a tributação exclusiva na fonte do décimo terceiro separadamente
- **Tabelas Oficiais** — consulte as tabelas de INSS, IRRF mensal e IR anual por ano-calendário
- **Detalhamento mensal** — veja mês a mês os valores de salário bruto, INSS, base de cálculo e IRRF retido
- **Redução do IR** — aplica as regras da Lei nº 15.270/2025 para rendimentos a partir de 2026
- **100% privado** — todos os cálculos acontecem no navegador; nenhum dado é enviado a servidores

### Anos suportados

| Ano | INSS | IRRF Mensal | IR Anual |
|-----|------|-------------|----------|
| 2024 | Portaria MPS | Tabela vigente | IRPF |
| 2025 | Portaria MPS 1.432/2024 | MP 1.294/2025 | IRPF |
| 2026 | Portaria Interministerial MPS/MF nº 13/2026 | Lei nº 15.191/2025 + redução Lei nº 15.270/2025 | IRPF |
| 2027+ | Estimativa (usa 2026) | Estimativa (usa 2026) | Estimativa |

## Stack

- [.NET 10](https://dotnet.microsoft.com/) / C#
- [Blazor WebAssembly](https://learn.microsoft.com/aspnet/core/blazor/) (PWA)
- [MudBlazor 9](https://mudblazor.com/) — componentes de UI

## Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

## Desenvolvimento

```bash
# Restaurar dependências
dotnet restore

# Executar em modo desenvolvimento (HTTPS em https://localhost:7053)
dotnet run --project TaxPlanner.Wasm

# Build
dotnet build

# Publicar artefatos WebAssembly otimizados
dotnet publish -c Release
```

## Estrutura do projeto

```
TaxPlanner.slnx
TaxPlanner.Wasm/
├── Program.cs                    — Bootstrap: registra serviços e monta o host WebAssembly
├── App.razor                     — Raiz da aplicação; contém o <Router>
├── _Imports.razor                — Usings globais para todos os componentes .razor
├── Pages/
│   ├── Home.razor                — Página inicial com explicação do fluxo
│   ├── AnaliseTributaria.razor   — Simulador de IR (entrada de dados + resultado)
│   └── Tabelas.razor             — Consulta das tabelas oficiais por ano
├── Models/
│   ├── PeriodoSalarial.cs        — Representa um período com salário fixo
│   ├── ResultadoIR.cs            — Resultado completo da simulação
│   └── TabelaImposto.cs          — Estruturas das tabelas de INSS e IR
├── Services/
│   └── CalculadoraIRService.cs   — Lógica de cálculo de INSS, IRRF e IR anual
├── Layout/
│   └── MainLayout.razor          — Layout principal com navegação
└── wwwroot/                      — Ativos estáticos (HTML, CSS, ícones, PWA manifest)
```

## Como funciona o cálculo

1. Para cada mês do ano-calendário, o serviço calcula o **INSS progressivo** sobre o salário bruto
2. A base de cálculo mensal é `salário bruto − INSS`
3. O **IRRF mensal** é calculado pela tabela progressiva mensal aplicando a dedução da faixa; a partir de 2026 aplica-se a redução da Lei nº 15.270/2025
4. A **base anual** é a soma das bases mensais; sobre ela aplica-se a tabela progressiva anual para obter o **IR devido**
5. O **resultado final** é `IR devido anual − IRRF retido no ano` (negativo = restituição; positivo = imposto a pagar)
6. O **13º salário** tem tributação exclusiva na fonte e não entra na base da declaração anual

> **Aviso:** os valores calculados são estimativas baseadas nas tabelas oficiais. Não são consideradas deduções com saúde, educação, dependentes ou pensão alimentícia. Consulte um contador para uma análise tributária completa.
