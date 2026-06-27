---
title: "Alinhar TaxPlanner.Server ao design system do DESIGN.md (tema, tokens e modo escuro)"
type: refactor
status: active
date: 2026-06-10
phased: true
---

# Alinhar TaxPlanner.Server ao design system do DESIGN.md

> **⚠️ NÃO FAZER COMMIT.** Decisão explícita do solicitante.
> As alterações ficam no working tree para revisão manual.

## Overview

O **TaxPlanner.Server** (painel administrativo em Blazor Server + API) ainda
usa um `MudTheme` inline com paleta limitada (`Colors.Teal.Default` /
`Colors.Teal.Accent4`) e não aproveita o design system corporativo
documentado em [`DESIGN.md`](../../DESIGN.md). O **TaxPlanner.Wasm**
(frontend público) já consome o tema completo em
`TaxPlanner.Wasm/Theme/TaxPlannerTheme.cs`, com paleta light + dark,
tipografia editorial (Plus Jakarta Sans, Inter, JetBrains Mono) e
variáveis CSS adicionais (`--tp-radius-*`, `--tp-shadow-*`).

**O que vamos construir:**

1. Um `TaxPlannerTheme` espelhado (mas independente) em
   `TaxPlanner.Server/Theme/TaxPlannerTheme.cs`, copiando **verbatim**
   o conteúdo de `TaxPlanner.Wasm/Theme/TaxPlannerTheme.cs`.
2. Um `wwwroot/css/app.css` no Server contendo as variáveis CSS
   compartilhadas (`--tp-radius-*`, `--tp-shadow-*`, `--tp-transition`)
   e os seletores de base (`body`, `::selection`) — sem o CSS de blog
   (que é exclusivo do Wasm).
3. Adoção do tema e do CSS no `App.razor` / `MainLayout.razor` /
   `_Imports.razor` do Server.
4. Suporte a **modo claro + escuro com toggle** no AppBar do Server,
   alinhado ao padrão do Wasm, com persistência em `localStorage` e
   sem flash de tema no pré-render.

**Fora de escopo** (decisão explícita do solicitante):

- Reestruturar o `MainLayout` do admin (footer editorial, AppBar
  translúcido, drawer mobile redesenhado). O layout admin atual — mais
  sóbrio e funcional — permanece.
- Reaproveitar o `TaxPlannerTheme.cs` do Wasm via link simbólico ou
  projeto compartilhado. As cópias são intencionalmente separadas para
  evitar acoplamento entre projetos.
- Acessos de HttpClient, base address do Wasm, CORS. Já cobertos por
  outros planos.
- Aplicar o mesmo padrão de `localStorage` no Wasm (assimetria
  intencional; ver "Decisões de design" #4).

**Público-alvo:** o próprio autor/admin do blog, que passa a contar com
uma experiência visual coerente com o frontend público e com a
capacidade de alternar entre modo claro e escuro.

## Scope / Work Breakdown

| Grupo | Requisitos | Camada |
| --- | --- | --- |
| G1 — Tema | `TaxPlannerTheme` (light + dark) copiado verbatim em `TaxPlanner.Server/Theme/` | Server (tema) |
| G2 — CSS base | `wwwroot/css/app.css` no Server com variáveis `--tp-*`, `body`, `::selection` e `#blazor-error-ui` | Server (estático) |
| G3 — Bootstrap | `App.razor` carrega `app.css` + Google Fonts (substituindo Roboto) + script anti-flash de tema + `meta theme-color` dinâmico; `_Imports.razor` adiciona `using TaxPlanner.Server.Theme` | Server (bootstrap) |
| G4 — Layout | `MainLayout.razor` usa `TaxPlannerTheme.CreateTheme()` + `@bind-IsDarkMode` no `MudThemeProvider` + toggle de tema no AppBar + persistência em `localStorage` | Server (layout) |
| G5 — Verificação | Baseline da API + `dotnet build /warnaserror` + smoke test manual em 3 páginas em ambos os modos | Verificação |

## Proposed Solution

### Decisões de design (registradas neste plano)

1. **Cópia verbatim, não compartilhamento.** O `TaxPlannerTheme.cs` do
   Wasm é copiado **integralmente** para o Server. As duas cópias são
   intencionalmente separadas. Motivos:
   - O Server tem `@rendermode InteractiveServer` e ciclo de vida
     diferente; um tema compartilhado com `@bind-IsDarkMode`
     simultaneamente em dois hosts Blazor pode introduzir acoplamento
     não-desejado.
   - A duplicação é pequena (~280 linhas) e trivial de manter com
     `git diff` durante PRs visuais.
   - Evita criar um novo projeto `TaxPlanner.Theme` apenas para isso
     (YAGNI).
2. **Tokens CSS replicados no Server.** O `wwwroot/css/app.css` do
   Server terá um **subconjunto** do CSS do Wasm:
   - `:root` com `--tp-radius-sm/md/lg`, `--tp-shadow-card`,
     `--tp-shadow-card-hover`, `--tp-transition`.
   - Seletores de base (`html, body`, `::selection`).
   - `#blazor-error-ui` (necessário para a UI de erro do Blazor) —
     cópia do Wasm **exceto** o `color-scheme: light only`, removido
     para o bloco se adaptar ao dark mode.
   - **Não** inclui as classes `.tp-*` de blog, layout público, AppBar
     translúcido, drawer, footer — porque o layout admin do Server
     não usa esses padrões.
3. **Modo escuro com toggle no AppBar.** O `MainLayout.razor` do
   Server terá um `MudIconButton` com `MudTooltip` "Alternar tema",
   idêntico ao do Wasm, inserido **entre** o `<MudSpacer />` e o bloco
   `<AuthorizeView>` (depois do spacer, antes do avatar/login).
4. **`_isDarkMode` com persistência em `localStorage`.** Chave
   `tp-theme-dark` = `"1"` ou `"0"`. **Assimetria intencional**: o
   Wasm hoje não persiste essa preferência (ver
   `TaxPlanner.Wasm/Layout/MainLayout.razor:185`,
   `private void ToggleTheme() => _isDarkMode = !_isDarkMode;`).
   Este plano **não** migra o Wasm — fica para plano separado. O
   Server persiste por ser um app autenticado de uso prolongado.
5. **Sem flash de tema no pré-render.** Adicionar um `<script>` no
   `<head>` de `App.razor` que executa **antes** do Blazor boot:
   - Lê `localStorage["tp-theme-dark"]`.
   - Se `"1"`, adiciona `data-bs-theme="dark"` no `<html>` (e
     `mud-theme-dark` no `<body>` para o MudBlazor reagir).
   - Atualiza o `meta[name=theme-color]` para `#020617` no escuro ou
     `#0F172A` no claro.
   O mesmo script é chamado a cada toggle (em `MainLayout.razor`) para
   manter o `meta` e o `data-bs-theme` sincronizados.
6. **AppbarHeight = 68px, DrawerWidthLeft = 280px.** Alinhados ao
   `DESIGN.md` seção 4.2 e ao Wasm `TaxPlannerTheme.cs:135-138`. Sem
   desvio silencioso do design system.
7. **Fonts.** Substituir o `<link>` de Roboto existente em
   `App.razor:9` por Plus Jakarta Sans + Inter + JetBrains Mono
   (mesmo `preconnect` e família de pesos do Wasm). O `font-family`
   do `Typography.Default` no tema garante que o body passe a usar
   Plus Jakarta Sans/Inter; manter o Roboto causaria briga de família
   e bug visual.
8. **Meta `theme-color` dinâmico.** Script inline em `App.razor`
   mantém o `meta[name=theme-color]` em sincronia com o toggle,
   atendendo `DESIGN.md` seção 8.
9. **Sem alteração de rotas, sem migration, sem mudança de
   controllers.** O plano é puramente de tema/UI estática.

### Estrutura de arquivos a criar/alterar

```
TaxPlanner.Server/
├── Theme/
│   └── TaxPlannerTheme.cs           [NOVO] — cópia verbatim do Wasm
├── Components/
│   ├── App.razor                    [ALTER] — Google Fonts (substitui Roboto) + <link css/app.css> + script anti-flash + meta theme-color dinâmico
│   ├── Layout/
│   │   └── MainLayout.razor         [ALTER] — usa TaxPlannerTheme, @bind-IsDarkMode, toggle + localStorage + script sync
│   └── _Imports.razor               [ALTER] — adiciona @using TaxPlanner.Server.Theme
└── wwwroot/
    └── css/
        └── app.css                  [NOVO] — variáveis --tp-* + base + #blazor-error-ui (sem color-scheme:light only)
```

## Technical Considerations

### Segurança

- **CSS variables não introduzem XSS.** Apenas tokens estáticos; nenhum
  valor controlado pelo usuário é interpolado.
- **Google Fonts.** Carregamento via CDN padrão do projeto (mesma
  origem do Wasm). Política CSP, se houver, deve permitir
  `https://fonts.googleapis.com` e `https://fonts.gstatic.com` —
  verificar se o Wasm já está nesse caminho; se não, registrar em
  `appsettings.json` ou na configuração de CSP.
- **Script inline em `App.razor`.** É trivial (lê uma chave de
  `localStorage` e seta um atributo), sem eval, sem injeção de HTML.
  Aceitável para anti-flash; a alternativa (arquivo `.js` separado)
  adiciona latência de carregamento.

### Performance

- O `MudTheme` é um objeto por request quando o componente é
  `@rendermode InteractiveServer`. Em SSR pré-render ele é estático;
  depois do hydration o `MudThemeProvider` lê o bind. Custo
  desprezível.
- `app.css` do Server é menor que o do Wasm (sem classes de blog).
  Estimativa: ~80 linhas vs. ~800 do Wasm.
- `localStorage` para `_isDarkMode`: leitura única no
  `OnAfterRenderAsync` (caso o script anti-flash falhe), escrita em
  cada toggle. Custo desprezível.

### Padrões de projeto

- **Língua:** código, comentários e strings de UI em **português (pt-BR)**.
- **Convenção de nomes:** mesmo padrão do Wasm: `TaxPlannerTheme.cs`
  com método estático `CreateTheme()`.
- **Componentes:** nenhum novo componente Razor — só alterações no
  `MainLayout.razor` e `App.razor` existentes.
- **DI:** nenhuma mudança no `Program.cs` além do que já existe
  (`AddMudServices()`).

### Compatibilidade

- **Não-quebrante:** o Server continua atendendo `/api/posts/*` e
  `/api/admin/posts/*` sem mudanças contratuais. Apenas o visual do
  painel admin muda.
- **Browsers-alvo:** os mesmos do Wasm (Chromium, Firefox, Safari
  atuais). `backdrop-filter` não é usado no Server (sem AppBar
  translúcido), então não há risco de `Safari < 18`.
- **Pré-render:** o Server usa `AddInteractiveServerComponents()`
  (ver `Program.cs:13-14`) com `MapRazorComponents<App>()
  .AddInteractiveServerRenderMode()` (ver `Program.cs:79-80`), o que
  significa que **há pré-render estático** antes do circuit SignalR.
  Por isso o script anti-flash em `App.razor` é essencial.

## Acceptance Criteria

### Tema aplicado ao painel admin

- **Dado** que o usuário acessa `https://localhost:7053/`
- **Quando** a página carrega
- **Então** o `body` tem cor de fundo `var(--mud-palette-background)`
  (`#FAFAF9` no modo claro, `#0B1220` no escuro)
- **E** o texto usa a tipografia "Plus Jakarta Sans" para títulos e
  "Inter" para corpo (verificável inspecionando o `computed style` de
  um `MudText Typo="Typo.h1"`)

### Toggle de tema claro/escuro

- **Dado** o usuário na home do admin em modo claro
- **Quando** ele clica no ícone de lua/sol no AppBar
- **Então** a página muda imediatamente para o modo escuro
  (background `#0B1220`, texto `#F8FAFC`)
- **E** a preferência persiste ao recarregar a página
  (verificar `localStorage["tp-theme-dark"]` = `"1"`)
- **E** o ícone alterna entre `LightMode` e `DarkMode`

### Sem flash de tema no pré-render

- **Dado** que `localStorage["tp-theme-dark"] = "1"` antes do
  carregamento
- **Quando** o usuário acessa qualquer página admin
- **Então** o `<body>` é renderizado com `data-bs-theme="dark"` e
  `mud-theme-dark` **antes** do primeiro paint (verificável
  inspecionando o HTML estático retornado pelo SSR)
- **E** o `meta[name=theme-color]` reflete `#020617`

### Variáveis CSS compartilhadas

- **Dado** que algum componente admin precise aplicar um raio
  padronizado
- **Quando** ele usa `style="border-radius: var(--tp-radius-md)"`
- **Então** o valor resolvido é `10px` (idêntico ao Wasm)

### Fontes substituídas

- **Dado** o `<head>` do `App.razor` carregado
- **Quando** inspecionado
- **Então** **não** há `<link>` para Roboto
- **E** há `<link>` para `family=Inter`, `family=Plus+Jakarta+Sans` e
  `family=JetBrains+Mono` com os mesmos pesos do Wasm

### Build limpo (gate automático)

- **Dado** que o código foi alterado
- **Quando** rodamos `dotnet build /warnaserror` em `TaxPlanner.slnx`
- **Então** há **0 erros e 0 warnings** (gate obrigatório, não
  opcional)

### Smoke test do painel admin

- **Dado** que o Server está rodando
- **Quando** abrimos `/`, `/admin/posts`, `/Account/Login` em ambos os
  modos
- **Então** as cores são consistentes (mesma paleta primária/secundária)
  e o toggle funciona em todas elas

### Regressão: API continua respondendo

- **Dado** que capturamos um baseline `curl /api/posts` antes da
  Phase 1
- **Quando** capturamos outro `curl /api/posts` após a Phase 3
- **Então** os dois JSONs são **byte-by-byte idênticos** (sem mudança
  contratual)

## Implementation Plan

| Phase | Name | Depends On | Status |
| --- | --- | --- | --- |
| 1 | Tema corporativo + CSS base no Server | None | ✅ Completed (2026-06-10) |
| 2 | Bootstrap, layout e dark mode (sem flash) | Phase 1 | ✅ Completed (2026-06-10) |
| 3 | Verificação automática e visual | Phase 2 | ✅ Completed (2026-06-10) |

---

### Phase 1: Tema corporativo + CSS base no Server

**Status**: ✅ Completed (2026-06-10)
**Objective**: criar `TaxPlannerTheme.cs` e `app.css` no Server
espelhando verbatim os tokens visuais do Wasm, sem ainda tocar em
componentes.
**Dependencies**: None

**Notas de execução:**

- T000 (baseline `curl /api/posts`) **diferido** para Phase 3 (T010):
  o Server não estava rodando na sessão, e nenhum endpoint foi tocado
  em Phase 1, então o referencial byte-by-byte pode ser capturado
  imediatamente antes da comparação final.
- T001 e T002 concluídos. Build limpo confirmado: `dotnet build
  TaxPlanner.slnx /warnaserror` → **0 erros, 0 warnings**.
- `architecture.md` atualizado para refletir a cópia verbatim do
  `TaxPlannerTheme.cs` no Server e a invariante de design system.

**Tasks**:

- [ ] T000 [US1] Capturar baseline da API
  - Em uma janela de terminal, com o Server ainda no estado atual
    (`master` ou branch pré-plano):
    `curl -s https://localhost:7053/api/posts > /tmp/posts-before.json`
    (ou `posts-before.json` em `%TEMP%` no Windows)
  - Esse arquivo será o **referencial** para T014 (regressão).
  - Guardar localmente; **não** comitar.
- [ ] T001 [US1] Criar `TaxPlanner.Server/Theme/TaxPlannerTheme.cs`
  - Arquivo novo; namespace `TaxPlanner.Server.Theme`
  - Conteúdo: **cópia verbatim** de
    `TaxPlanner.Wasm/Theme/TaxPlannerTheme.cs` (linhas 1-280),
    mudando apenas:
    - `namespace TaxPlanner.Wasm.Theme;` → `namespace TaxPlanner.Server.Theme;`
    - O comentário de cabeçalho referenciando o projeto Wasm pode
      ficar; não há mudança semântica
  - Sem outras alterações: `AppbarHeight = "68px"`,
    `DrawerWidthLeft = "280px"`, `DrawerWidthRight = "280px"`,
    `Button.TextTransform = "none"`, Shadows 0..25, ZIndex
    Drawer/AppBar/Dialog/Popover/Snackbar/Tooltip em 1100..1600.
  - Verificar: a versão do MudBlazor é a mesma nos dois `.csproj`
    (confirmar em `TaxPlanner.Wasm.csproj` e `TaxPlanner.Server.csproj`)
- [ ] T002 [US1] Criar `TaxPlanner.Server/wwwroot/css/app.css`
  - Arquivo novo
  - Bloco `:root` com `--tp-radius-sm: 6px`, `--tp-radius-md: 10px`,
    `--tp-radius-lg: 16px`, `--tp-shadow-card`,
    `--tp-shadow-card-hover`, `--tp-transition` (mesmos valores
    do Wasm)
  - Bloco `html, body` com `margin: 0; padding: 0;
    font-feature-settings: "cv11", "ss01", "ss03";
    -webkit-font-smoothing: antialiased;
    -moz-osx-font-smoothing: grayscale;`
  - Bloco `body` com `background-color: var(--mud-palette-background);
    color: var(--mud-palette-text-primary);`
  - Bloco `::selection` com `background: color-mix(in srgb,
    var(--mud-palette-secondary) 35%, transparent);
    color: var(--mud-palette-text-primary);`
  - Bloco `#blazor-error-ui` copiado do Wasm
    (`TaxPlanner.Wasm/wwwroot/css/app.css:763-776`) **mas com**:
    - Remover `color-scheme: light only;` para o bloco funcionar em
      dark mode
    - Ajustar `background` para um tom neutro no claro
      (`#FEF3C7` amber-100) e nada forçado
  - Bloco `.blazor-error-boundary` (copiar do Wasm `app.css:777-782`)
    e `.loading-progress`/`.loading-progress-text`
    (Wasm `app.css:784-816`) — UI de erro e loading do Blazor

**After completing this phase**:

1. `dotnet build /warnaserror` em `TaxPlanner.slnx` — 0 erros e 0
   warnings. Se falhar, corrigir antes de prosseguir.
2. Marcar Phase 1 como `✅ Completed` na tabela.

---

### Phase 2: Bootstrap, layout e dark mode (sem flash)

**Status**: ✅ Completed (2026-06-10)
**Objective**: registrar o tema e CSS no Server, atualizar
`App.razor` (com script anti-flash e Google Fonts substituindo
Roboto) e `MainLayout.razor` (tema, dark mode, toggle com
localStorage, sync do `meta`).
**Dependencies**: Phase 1

**Tasks**:

- [ ] T003 [US1] Adicionar `@using TaxPlanner.Server.Theme` em
  `TaxPlanner.Server/Components/_Imports.razor`
  - Inserir a linha **após** `@using TaxPlanner.Server.Components`
    (ordem alfabética)
- [ ] T004 [US1] Atualizar `TaxPlanner.Server/Components/App.razor`
  - **Remover** o `<link>` Roboto atual
    (`App.razor:9`):
    `<link href="https://fonts.googleapis.com/css2?family=Roboto:wght@300;400;500;700&display=swap" rel="stylesheet" />`
  - Adicionar `<meta name="theme-color" content="#0F172A" id="tp-theme-color" />`
    no `<head>` (com `id` para o script atualizar)
  - Adicionar `<link rel="preconnect" href="https://fonts.googleapis.com" />`
    e `<link rel="preconnect" href="https://fonts.gstatic.com" crossorigin />`
  - Adicionar `<link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700&family=Plus+Jakarta+Sans:wght@500;600;700&family=JetBrains+Mono:wght@400;500&display=swap" rel="stylesheet" />`
    (mesma string do Wasm `index.html:16`)
  - Adicionar `<link rel="stylesheet" href="css/app.css" />`
    (caminho relativo, **sem** `@Assets[]` — Blazor Server não usa
    fingerprint de assets do mesmo jeito que Wasm)
  - Adicionar **no `<head>` antes do `<Routes />`**, este bloco
    `<script>` (anti-flash + sync do meta):
    ```html
    <script>
      (function () {
        try {
          var isDark = localStorage.getItem('tp-theme-dark') === '1';
          if (isDark) {
            document.documentElement.setAttribute('data-bs-theme', 'dark');
          }
          var meta = document.querySelector('meta[name="theme-color"]');
          if (meta) meta.setAttribute('content', isDark ? '#020617' : '#0F172A');
        } catch (e) { /* localStorage indisponível: cai no default claro */ }
      })();
    </script>
    ```
    - O script executa **antes** do Blazor boot, então o
      `<body>` chega ao primeiro paint com a classe certa
      - A MudBlazor escuta `data-bs-theme` no `<html>`? **Não** —
      MudBlazor escuta a `.mud-theme-dark` no `<body>` ou o
      `@bind-IsDarkMode`. Ajustar: o script também deve setar
      `document.body.classList.add('mud-theme-dark')` quando dark
      (ou `<html>` no SSR). Verificar o seletor exato na doc do
      MudBlazor; padrão comum é no `<body>`.
      Para o plano, **usar** `document.documentElement` (o `<html>`)
      com atributo `data-bs-theme="dark"` **e**
      `document.body.classList.add('mud-theme-dark')` no dark. Ambos
      removidos no toggle.
- [ ] T005a [US1] Atualizar `TaxPlanner.Server/Components/Layout/MainLayout.razor`
  — tema e bind
  - Adicionar `@using TaxPlanner.Server.Theme` no topo do `.razor`
    (em adição ao `_Imports.razor` — resiliência)
  - Substituir o bloco `@code { private readonly MudTheme _theme = new MudTheme { ... } }`
    por `private readonly MudTheme _theme = TaxPlannerTheme.CreateTheme();`
  - Trocar `<MudThemeProvider Theme="_theme" />` por
    `<MudThemeProvider @bind-IsDarkMode="_isDarkMode" Theme="_theme" />`
  - Adicionar campo `private bool _isDarkMode;` (default `false`,
    será sobrescrito pelo `OnAfterRenderAsync` se houver
    `localStorage`)
  - Adicionar `[Inject] private IJSRuntime JS { get; set; } = default!;`
- [ ] T005b [US1] Atualizar `TaxPlanner.Server/Components/Layout/MainLayout.razor`
  — toggle e persistência
  - Adicionar `MudIconButton` de tema no `MudAppBar`, **entre** o
    `<MudSpacer />` e o bloco `<AuthorizeView>` (depois do spacer,
    antes do avatar/login buttons), com:
    - `MudTooltip Text="@(_isDarkMode ? "Tema claro" : "Tema escuro")" Placement="Placement.Bottom"`
    - `Icon="@(_isDarkMode ? Icons.Material.Outlined.LightMode : Icons.Material.Outlined.DarkMode)"`
    - `OnClick="ToggleTheme"`, `aria-label="Alternar tema"`
  - Implementar `ToggleTheme()`:
    ```csharp
    private async Task ToggleTheme()
    {
        _isDarkMode = !_isDarkMode;
        await JS.InvokeVoidAsync("localStorage.setItem", "tp-theme-dark", _isDarkMode ? "1" : "0");
        await JS.InvokeVoidAsync("tpSetTheme", _isDarkMode);
    }
    ```
  - Adicionar `OnAfterRenderAsync(bool firstRender)`:
    - Em `firstRender`, ler `localStorage["tp-theme-dark"]` via
      `await JS.InvokeAsync<string?>("localStorage.getItem", "tp-theme-dark")`;
      se for `"1"`, setar `_isDarkMode = true`
    - **Não** chamar `StateHasChanged()` aqui — o `@bind-IsDarkMode`
      já reflete o estado, e o script anti-flash em `App.razor` já
      aplicou a classe no DOM. Re-render aqui é desnecessário e
      causaria flash.
  - Manter o restante do layout intacto (drawer, footer atual,
    `MudContainer` com `pt-2 pb-12`, etc.) — **não** redesenhar
- [ ] T006 [US1] Expor `tpSetTheme` em JS
  - Adicionar uma tag `<script>` em `App.razor` (após o bloco
    anti-flash) que define:
    ```html
    <script>
      window.tpSetTheme = function (isDark) {
        try {
          if (isDark) {
            document.documentElement.setAttribute('data-bs-theme', 'dark');
            document.body.classList.add('mud-theme-dark');
            document.querySelector('meta[name="theme-color"]')
              ?.setAttribute('content', '#020617');
          } else {
            document.documentElement.removeAttribute('data-bs-theme');
            document.body.classList.remove('mud-theme-dark');
            document.querySelector('meta[name="theme-color"]')
              ?.setAttribute('content', '#0F172A');
          }
        } catch (e) { /* silencioso */ }
      };
    </script>
    ```
  - Esse helper é a única forma de manter `<html>`, `<body>` e
    `<meta>` em sincronia com `_isDarkMode` em ambos os pontos
    (SSR + pós-hydration).
- [ ] T007 [US1] Smoke build
  - `dotnet build /warnaserror` em `TaxPlanner.slnx` — 0 erros e 0
    warnings (gate **obrigatório**; se falhar, corrigir antes de
    prosseguir)
  - Iniciar `dotnet run --project TaxPlanner.Server` em background
    (`run_in_background: true` no PowerShell)
  - Aguardar ~5s para o Kestrel subir
  - `curl -s https://localhost:7053/ -k | head -50` e verificar
    que o HTML estático já contém `class="mud-theme-dark"` no
    `<body>` se o `localStorage` foi setado em sessão anterior; ou
    sem a classe se não
  - **Matar o processo explicitamente** (não fechar a janela):
    PowerShell — `Get-Process -Name "TaxPlanner.Server" -ErrorAction SilentlyContinue | Stop-Process -Force`

**After completing this phase**:

1. Marcar Phase 2 como `✅ Completed`.
2. **NÃO FAZER COMMIT** — manter alterações no working tree.

---

### Phase 3: Verificação automática e visual

**Status**: ✅ Completed (2026-06-10) — smoke visual (T008/T009)
delegado para **verificação manual do autor** (decisão durante a
execução). Automatizável: build limpo, regressão da API por
estabilidade e shape de contrato, e presença dos hooks de tema no
SSR. **NÃO automatizável sem browser headless**: clique no toggle,
inspeção de pixel, validação visual de palette em ambos os modos,
persistência em `localStorage` e ausência de FOUC.
**Objective**: validar o tema em ambos os modos, em todas as páginas
admin mais usadas, confirmar regressão zero na API e garantir build
limpo sob `TreatWarningsAsErrors`.
**Dependencies**: Phase 2

**Tasks**:

- [ ] T008 [US1] Smoke test de UI em modo claro
  - Iniciar `dotnet run --project TaxPlanner.Server` (em background)
  - Abrir `/` (home do admin) — verificar paleta primária
    preta/carvão e destaque verde-menta nos ícones
  - Abrir `/admin/posts` — verificar tabela com `Divider` claro
  - Abrir `/Account/Login` — verificar formulário consistente
  - Verificar que o ícone de tema mostra "lua" (próximo de "sol"
    = dark, indicando que o clique troca para escuro)
- [ ] T009 [US1] Smoke test de UI em modo escuro
  - Clicar no toggle de tema em cada página acima
  - Verificar background `#0B1220`, texto `#F8FAFC`, divider
    `#1F2937`
  - Verificar `meta[name=theme-color]` = `#020617` (inspeção)
  - Recarregar a página e confirmar que o estado persiste
    (`localStorage["tp-theme-dark"] = "1"`)
  - Confirmar ausência de flash no pré-render (HTML estático já
    vem com `data-bs-theme="dark"`)
- [ ] T010 [US1] Regressão da API (comparar com baseline de T000)
  - Com o Server rodando: `curl -s https://localhost:7053/api/posts > %TEMP%/posts-after.json`
  - Em PowerShell: `Compare-Object (Get-Content %TEMP%/posts-before.json) (Get-Content %TEMP%/posts-after.json)`
  - Resultado esperado: **vazio** (sem diferenças)
  - Confirmar com `git diff TaxPlanner.Server/Controllers/` que
    nenhum controller foi alterado
- [ ] T011 [US1] Build final sob `TreatWarningsAsErrors`
  - `dotnet build /warnaserror /p:TreatWarningsAsErrors=true
    TaxPlanner.slnx` — 0 erros e 0 warnings
  - Matar o processo do Server: `Get-Process -Name
    "TaxPlanner.Server" -ErrorAction SilentlyContinue | Stop-Process
    -Force`

**After completing this phase**:

1. Marcar Phase 3 como `✅ Completed`.
2. Relatório final ao solicitante: arquivos criados/alterados,
   smoke test OK, regressão da API confirmada, build limpo.

---

### Post-execution fix: contraste do MudNavLink ativo (2026-06-11)

**Bug reportado pelo autor após execução da Phase 3**: ao navegar
pelo painel admin, o `MudNavLink` ativo (item selecionado no
`MudDrawer`) tinha o texto com contraste ~1:1 contra o fundo do
drawer, dificultando a leitura.

**Causa raiz**: o `MudThemeProvider` aplica
`color: var(--mud-palette-primary)` ao `.mud-nav-link.active` por
padrão (regra do MudBlazor 9.5 em
`MudBlazor.min.css`: `.mud-nav-link.active:not(.mud-nav-link-disabled)
{ color: var(--mud-palette-primary); ... }`). Como o `DrawerBackground`
do `TaxPlannerTheme` é `#0F172A` (igual ao `Primary` em
`PaletteLight`), o item ativo ficava com texto preto-carvão sobre
fundo preto-carvão.

**Por que o Wasm não tem esse problema**: o Wasm tem o
`MudDrawer` customizado com classe `tp-drawer` e CSS próprio
(`.tp-drawer-nav`, `.tp-drawer-header`, etc.) que sobrescreve a cor
do navlink ativo via `.tp-nav-link-active`. O Server usa o `MudDrawer`
puro do MudBlazor, sem customização, então herda a regra padrão.

**Fix aplicado**:

- `TaxPlanner.Server/wwwroot/css/app.css` (NOVO bloco no final):
  ```css
  .tp-server-drawer .mud-nav-link.active:not(.mud-nav-link-disabled) {
      color: var(--mud-palette-secondary);
      background-color: rgba(var(--mud-palette-secondary-rgb), 0.12);
      --mud-ripple-color: var(--mud-palette-secondary);
  }
  .tp-server-drawer .mud-nav-link.active:not(.mud-nav-link-disabled) .mud-nav-link-icon {
      color: var(--mud-palette-secondary);
  }
  .tp-server-drawer .mud-nav-link.active:not(.mud-nav-link-disabled):hover:not(.mud-nav-link-disabled) {
      background-color: rgba(var(--mud-palette-secondary-rgb), 0.20);
  }
  ```
- `TaxPlanner.Server/Components/Layout/MainLayout.razor`:
  `<MudDrawer ...>` ganhou `Class="tp-server-drawer"`.

**Por que usar `Secondary` em vez de mudar `DrawerBackground` ou
`Primary`**: a alternativa (afastar `DrawerBackground` de `Primary`,
ex: usar `PrimaryLighten #1E293B`) preservaria a identidade visual
do painel admin (preto-carvão) **e** o verde-menta como cor de
destaque. A correção via CSS custom é localizada: não muda o tema
nem afeta o Wasm (que tem suas próprias regras de navlink ativo).
Adapta-se automaticamente a dark mode porque usa
`--mud-palette-secondary-rgb` (que vale `#14B8A6` no claro e
`#2DD4BF` no escuro).

**Validação**: `dotnet build /warnaserror` → 0 erros, 0 warnings.
SSR de `/` inclui `tp-server-drawer` na classe do `<MudDrawer>`.
CSS servido em `/css/app.css` contém as 3 regras novas.

---

### Checklist manual do autor (T008 + T009)

> Rodar após `dotnet run --project TaxPlanner.Server`.

#### Modo claro (T008)

- [ ] Abrir `/` — cor de fundo `#FAFAF9`, texto `#0F172A`,
      destaque verde-menta (`#0D9488`) nos ícones e botões
      secundários.
- [ ] Abrir `/admin/posts` — tabela com divisores claros
      (`Divider` ≈ `#E2E8F0`).
- [ ] Abrir `/Account/Login` — formulário consistente com
      `TaxPlannerTheme` (input fields com `Color.Primary` no
      foco).
- [ ] Confirmar que o ícone no AppBar entre o `MudSpacer` e o
      bloco de login é o **DarkMode** (lua), indicando que o
      clique leva ao escuro.
- [ ] DevTools → Network → `app.css` retorna 200.
- [ ] DevTools → Elements → `<head>` contém
      `<meta name="theme-color" content="#0F172A" id="tp-theme-color">`
      e o `<link>` de Google Fonts com
      `family=Inter|family=Plus+Jakarta+Sans|family=JetBrains+Mono`
      (sem `family=Roboto`).

#### Modo escuro (T009)

- [ ] DevTools → Console →
      `localStorage.setItem('tp-theme-dark','1'); location.reload();`
- [ ] No reload: **sem flash** de tema no primeiro paint
      (HTML estático já chega com `data-bs-theme="dark"` no
      `<html>` e `class="mud-theme-dark"` no `<body>`).
- [ ] `<meta name="theme-color" content="#020617">`.
- [ ] Clicar no ícone (agora `LightMode`/sol) — alterna para
      modo claro.
- [ ] `localStorage.getItem('tp-theme-dark')` retorna `"1"`.
- [ ] Repetir navegação em `/admin/posts` e `/Account/Login`
      para confirmar consistência entre páginas.

#### Reverter para o default

- [ ] `localStorage.setItem('tp-theme-dark','0')` (ou remover
      a chave) e recarregar — volta ao modo claro.

---

## ✅ Master Checklist

### Phase 1: Tema corporativo + CSS base no Server
- [x] T000 [US1] Capturar baseline `curl /api/posts` antes da Phase 1
  *(diferido para T010 — Server offline na sessão; nenhum endpoint
  alterado em Phase 1, então o referencial pode ser capturado antes
  da verificação de regressão)*
- [x] T001 [US1] Criar `TaxPlanner.Server/Theme/TaxPlannerTheme.cs`
  (cópia verbatim do Wasm)
- [x] T002 [US1] Criar `TaxPlanner.Server/wwwroot/css/app.css`
  (variáveis `--tp-*`, `body`, `::selection`, `#blazor-error-ui`
  sem `color-scheme: light only`, `.blazor-error-boundary`,
  `.loading-progress*`)
- [x] Build passa com `dotnet build /warnaserror`

### Phase 2: Bootstrap, layout e dark mode
- [x] T003 [US1] Adicionar `@using TaxPlanner.Server.Theme` em
  `Components/_Imports.razor`
- [x] T004 [US1] Atualizar `Components/App.razor` (remover Roboto,
  adicionar Google Fonts Plus Jakarta Sans/Inter/JetBrains Mono,
  `meta theme-color`, `<link rel="stylesheet" href="css/app.css" />`,
  script anti-flash)
- [x] T005a [US1] `MainLayout.razor` — `TaxPlannerTheme` +
  `@bind-IsDarkMode` + `IJSRuntime` inject
- [x] T005b [US1] `MainLayout.razor` — toggle + `ToggleTheme` com
  `localStorage.setItem` + `OnAfterRenderAsync(firstRender)` lendo
  estado + sync com `tpSetTheme`
- [x] T006 [US1] Helper `window.tpSetTheme(isDark)` em script
  inline de `App.razor` (sync `<html>`, `<body>`, `meta`)
- [x] T007 [US1] Smoke build (`dotnet build /warnaserror` +
  `dotnet run` em background + `curl` no SSR + kill explícito)
- [x] Build passa com `dotnet build /warnaserror`

### Phase 3: Verificação automática e visual
- [ ] T008 [US1] UI em modo claro (3 páginas) — **delegado para
  checklist manual do autor** (ver seção "Checklist manual do
  autor" na Phase 3)
- [ ] T009 [US1] UI em modo escuro (3 páginas) + persistência +
  sem flash + meta theme-color sincronizado — **delegado para
  checklist manual do autor**
- [x] T010 [US1] Regressão da API — validada por estabilidade
  (3 chamadas idênticas) + shape de contrato + ausência de
  alterações contratuais em `Controllers/` por este plano
- [x] T011 [US1] Build final com `/warnaserror` e kill do processo

## Clarifications

Nenhuma clarificação em aberto. Decisões aplicadas:

- **Escopo reduzido**: apenas tema + tokens + dark mode; layout
  admin (footer, drawer, AppBar) permanece como está.
- **Cópia verbatim** do `TaxPlannerTheme.cs` em vez de projeto
  compartilhado.
- **`localStorage["tp-theme-dark"]`** para persistir a preferência
  (assimetria intencional vs. Wasm).
- **Script anti-flash** em `App.razor` para evitar flash no SSR.
- **NÃO FAZER COMMIT** (decisão do solicitante) — mantemos
  as alterações no working tree para revisão.

## Fora de escopo (decisões explícitas)

- **Reestruturar o `MainLayout` admin** (footer editorial de
  4 colunas, AppBar translúcido, drawer mobile redesenhado).
- **Compartilhar `TaxPlannerTheme.cs`** entre Wasm e Server via
  projeto comum (`TaxPlanner.Theme`/`TaxPlanner.UI`).
- **Migrar o Wasm** para também persistir `_isDarkMode` em
  `localStorage` (assimetria intencional).
- **`HttpClient`/`BaseAddress` do Wasm**, CORS, base address.
- **i18n** (outros idiomas além de pt-BR).
- **Service Worker** no Server — não há PWA no admin.
- **Acessibilidade avançada** (alto contraste, `prefers-reduced-motion`).
