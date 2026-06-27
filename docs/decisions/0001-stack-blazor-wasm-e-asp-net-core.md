# ADR 0001 — Stack Blazor WebAssembly + ASP.NET Core para frontend e backend

- **Data**: 2026-06-10
- **Status**: Aceito
- **Contexto**: precisamos entregar uma aplicação de planejamento
  tributário que rode inteiramente no navegador (privacidade) e que
  também disponibilize um painel administrativo autenticado com blog
  (CRUD de posts, Markdown, gestão de imagens).
- **Decisão**:
  - Frontend público em **Blazor WebAssembly** (`TaxPlanner.Wasm`),
    empacotado como **PWA** (service worker + manifest).
  - Backend em **ASP.NET Core 10** (`TaxPlanner.Server`) com:
    - Componentes Blazor Server (modo interativo) para a área admin.
    - API REST sob `/api/*` para o frontend Wasm.
    - `MudBlazor` 9.5 como design system.
- **Consequências**:
  - **Positivas**: reuso da linguagem (C#) entre cliente e servidor;
    cálculos rodam 100% no navegador (privacidade); UI admin pode
    reusar componentes MudBlazor.
  - **Negativas**: payload inicial do Wasm é maior que alternativas
    JS; debug de WebAssembly no browser é menos maduro; publicação
    precisa lidar com service worker.
- **Alternativas consideradas**:
  - **SPA em React + API em .NET** — exigiria manter dois stacks
    (TS + C#); quebraria o reuso de tipos.
  - **Apenas Blazor Server para o app inteiro** — quebraria o
    requisito de rodar cálculos localmente sem servidor.
- **Referências**:
  - [`../architecture.md`](../architecture.md)
  - `CLAUDE.md` (visão geral e comandos essenciais)
  - `TaxPlanner.Wasm/Program.cs`, `TaxPlanner.Wasm/App.razor`
  - `TaxPlanner.Server/Program.cs`
