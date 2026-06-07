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
- Nullable reference types e implicit usings habilitados
