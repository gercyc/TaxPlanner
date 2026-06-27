# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Comandos Essenciais

```bash
# Executar em desenvolvimento (HTTPS em localhost:7053)
dotnet run --project TaxPlanner.Wasm

# Build
dotnet build

# Publicar (gera artefatos WebAssembly otimizados)
dotnet publish -c Release

# Restaurar dependências
dotnet restore
```

Não há testes configurados no projeto ainda.

## Arquitetura

**TaxPlanner** é uma aplicação **Blazor WebAssembly** (PWA) que roda inteiramente no navegador como WebAssembly — não há servidor de aplicação em runtime.

### Estrutura do projeto

```
TaxPlanner.slnx
TaxPlanner.Wasm/
├── Program.cs           — Bootstrap: registra serviços e monta o WebAssembly host
├── App.razor            — Raiz da aplicação; contém o <Router>
├── _Imports.razor       — Usings globais para todos os componentes .razor
├── Pages/               — Componentes roteáveis (@page "/rota")
├── Layout/              — MainLayout.razor envolve todas as páginas
│   └── MainLayout.razor — AppBar translúcido, drawer mobile, footer de 4 colunas
├── Theme/               — MudTheme corporativo (light + dark)
│   └── TaxPlannerTheme.cs
├── Models/              — DTOs e entidades de domínio
├── Services/            — HttpClient, lógica de cálculo, acesso ao blog
└── wwwroot/             — Ativos estáticos servidos diretamente (HTML, CSS, ícones, PWA manifest)
```

### Padrões de desenvolvimento

- **Componentes**: arquivos `.razor` combinam HTML + C# (Razor syntax). Lógica vai em `@code { }` ou em code-behind `.razor.cs`.
- **Roteamento**: adicionar `@page "/caminho"` em um componente o registra como rota; o `<Router>` em `App.razor` resolve automaticamente.
- **Injeção de dependência**: serviços registrados em `Program.cs` via `builder.Services.Add*()` e injetados com `@inject` ou `[Inject]`.
- **HttpClient**: pré-configurado para chamadas à API; usar `@inject HttpClient Http`.
- **PWA**: o `service-worker.js` desativa cache em desenvolvimento; em produção usa `service-worker.published.js` com estratégia de cache offline.

### Stack

- .NET 10 / C#
- Blazor WebAssembly (`Microsoft.AspNetCore.Components.WebAssembly` v10.0.8)
- MudBlazor 9.5 (componentes UI + `MudTheme` para o design system)
- Nullable reference types e implicit usings habilitados

## Design System

A identidade visual é **editorial e corporativa**: base preta/carvão sóbria com verde-menta como cor de destaque (tributos, receita, resultado positivo). Toda a documentação detalhada — paleta, tipografia, espaçamentos, componentes, padrões de uso e checklist de revisão — está em **[`DESIGN.md`](./DESIGN.md)**.

**Resumo rápido:**

- **Tema**: `TaxPlanner.Wasm/Theme/TaxPlannerTheme.cs` — `TaxPlannerTheme.CreateTheme()` retorna o `MudTheme` com paletas light e dark.
- **Cores principais**:
  - `Primary` `#0F172A` (preto-carvão) · `PrimaryDarken` `#020617` · `PrimaryLighten` `#1E293B`
  - `Secondary` `#0D9488` (verde-menta) · `SecondaryDarken` `#0F766E` · `SecondaryLighten` `#14B8A6`
  - `Background` claro `#FAFAF9` / escuro `#0B1220` · `Surface` claro `#FFFFFF` / escuro `#111827`
- **Tipografia**: Plus Jakarta Sans (UI/títulos) + Inter (corpo) + JetBrains Mono (código), carregadas via Google Fonts em `wwwroot/index.html`.
- **Tokens de raio**: `--tp-radius-sm 6px` · `--tp-radius-md 10px` (padrão) · `--tp-radius-lg 16px`.
- **Layout**: AppBar translúcido com `backdrop-filter blur(12px)` + drawer mobile (`<md`) + footer de 4 colunas.
- **CSS global**: classes utilitárias com prefixo `tp-` (`.tp-brand`, `.tp-nav-link`, `.tp-nav-link-active`, `.tp-drawer-*`, `.tp-footer-*`, `.tp-main`) e variáveis CSS em `:root`.

**Ao criar/modificar UI:**

1. Use sempre tokens semânticos (`Color.Primary`, `var(--mud-palette-text-primary)`) — **nunca** cores literais (`#0F172A`).
2. Confirme que o componente funciona em **modo claro e escuro**.
3. Confirme o layout responsivo (drawer mobile, sem overflow horizontal).
4. Mantenha o idioma em **português (pt-BR)**.
5. Botões só com ícone precisam de `aria-label` e/ou `MudTooltip`.
6. Antes de finalizar, rode `dotnet build` — 0 erros e 0 warnings.
