# TaxPlanner — Design System

> Identidade visual corporativa do projeto, aplicada em Blazor WebAssembly com **MudBlazor** 9.5.
> Visual editorial minimalista: base preta/carvão sóbria com verde-menta como cor de destaque (tributos, receita, resultado positivo).

---

## 1. Visão geral

| Atributo       | Valor                                                                |
|----------------|----------------------------------------------------------------------|
| Estilo         | Editorial, corporativo, minimalista, blog-first                       |
| Inspiração     | Publicações financeiras (Bloomberg, The Economist, fintechs sérias)  |
| Biblioteca UI  | [MudBlazor](https://mudblazor.com/) 9.5                              |
| Tema           | `TaxPlanner.Wasm/Theme/TaxPlannerTheme.cs`                           |
| Layout         | `TaxPlanner.Wasm/Layout/MainLayout.razor`                            |
| Estilos globais| `TaxPlanner.Wasm/wwwroot/css/app.css`                                |
| Tipografia     | Plus Jakarta Sans (UI/títulos) · Inter (corpo) · JetBrains Mono (código) |

**Princípios**

1. **Sóbrio sobre ruidoso** — fundos neutros, hierarquia por tipografia, sem gradientes decorativos.
2. **Verde-menta com parcimônia** — reservado para ações primárias, links e resultados positivos.
3. **Mobile-first** — drawer temporário em telas pequenas, navegação horizontal em `≥md`.
4. **Privacidade visível** — selos e avisos de "cálculo local" sempre ao alcance do usuário.

---

## 2. Paleta de cores

Todas as cores são expostas como propriedades do `MudTheme` (`Primary`, `Secondary`, `Surface`, `Background`, `TextPrimary`, etc.) e consumidas pelos componentes MudBlazor e pelo CSS global via `var(--mud-palette-*)`.

### 2.1 Modo claro (default)

| Token                   | Hex       | Uso                                              |
|-------------------------|-----------|--------------------------------------------------|
| `Primary`               | `#0F172A` | Cor da marca, títulos principais, AppBar         |
| `PrimaryDarken`         | `#020617` | Estados hover/pressed de elementos primários     |
| `PrimaryLighten`        | `#1E293B` | Variação clara do primário                       |
| `Secondary`             | `#0D9488` | Destaque (verde-menta), links, CTAs, accent      |
| `SecondaryDarken`       | `#0F766E` | Hover do secundário                              |
| `SecondaryLighten`      | `#14B8A6` | Variação clara (badges, success-soft)            |
| `Tertiary`              | `#475569` | Texto secundário, ícones neutros                 |
| `Background`            | `#FAFAF9` | Fundo da página (off-white quente)               |
| `Surface`               | `#FFFFFF` | Cards, AppBar translúcida, drawers               |
| `Divider` / `LinesDefault` | `#E2E8F0` | Bordas, separadores, linhas de tabela         |
| `TextPrimary`           | `#0F172A` | Texto principal                                  |
| `TextSecondary`         | `#475569` | Texto de apoio, metadados                        |
| `TextDisabled`          | `#94A3B8` | Estados desabilitados                            |
| `Success`               | `#059669` | Alertas positivos, validação                     |
| `Error`                 | `#DC2626` | Erros, alertas críticos                          |
| `Warning`               | `#D97706` | Avisos                                           |
| `Info`                  | `#0284C7` | Avisos informativos                              |

### 2.2 Modo escuro

| Token             | Hex       |
|-------------------|-----------|
| `Background`      | `#0B1220` |
| `Surface`         | `#111827` |
| `AppbarBackground`| `#020617` |
| `DrawerBackground`| `#020617` |
| `Primary`         | `#F8FAFC` (invertido — texto claro)  |
| `Secondary`       | `#14B8A6` (mantém destaque)          |
| `TextPrimary`     | `#F8FAFC`                            |
| `TextSecondary`   | `#94A3B8`                            |
| `Divider`         | `#1F2937`                            |

A inversão do `Primary` (preto no claro, branco no escuro) garante contraste consistente em títulos e textos invertidos (ex.: AppBar) em ambos os modos.

### 2.3 Gradientes permitidos

Apenas **um** gradiente é usado no design system: o **brand mark** (logo do app).

```css
background: linear-gradient(135deg, var(--mud-palette-secondary) 0%, var(--mud-palette-secondary-darken) 100%);
```

**Regra**: gradientes não devem ser usados em botões, cards ou seções — apenas em elementos de marca e avatares com tamanho ≤ 64px.

---

## 3. Tipografia

Fontes carregadas via Google Fonts em `wwwroot/index.html` com `preconnect`.

| Função         | Família                                            | Pesos            |
|----------------|----------------------------------------------------|------------------|
| UI / Títulos   | `Plus Jakarta Sans`, `system-ui`, `sans-serif`     | 500, 600, 700    |
| Corpo          | `Inter`, `system-ui`, `-apple-system`, `Segoe UI`  | 400, 500, 600, 700 |
| Código         | `JetBrains Mono`, `Cascadia Code`, `Fira Code`     | 400, 500         |

### Escala tipográfica (definida em `TaxPlannerTheme.Typography`)

| Nível         | Tamanho | Peso | `line-height` | `letter-spacing` | Uso                          |
|---------------|---------|------|---------------|------------------|------------------------------|
| `H1`          | 2.75rem | 700  | 1.15          | -0.025em         | Hero, títulos de página      |
| `H2`          | 2.125rem| 700  | 1.20          | -0.020em         | Seções principais            |
| `H3`          | 1.625rem| 600  | 1.30          | -0.015em         | Cabeçalhos de cards          |
| `H4`          | 1.375rem| 600  | 1.35          | -0.010em         | Subseções                    |
| `H5`          | 1.125rem| 600  | 1.40          | -0.005em         | Títulos menores              |
| `H6`          | 1rem    | 600  | 1.45          | 0                | Labels, eyebrows             |
| `Subtitle1`   | 1rem    | 500  | 1.50          | —                | Subtítulos                   |
| `Subtitle2`   | 0.875rem| 600  | 1.45          | 0.01em           | Eyebrow, footer titles       |
| `Body1`       | 1rem    | 400  | 1.65          | —                | Texto corrido                |
| `Body2`       | 0.875rem| 400  | 1.60          | —                | Texto auxiliar               |
| `Button`      | 0.875rem| 600  | 1.50          | 0.005em          | Texto de botão               |

**Observação**: todos os botões e labels usam `text-transform: none` — sem CAIXA ALTA forçada.

### `font-feature-settings`

Habilitado globalmente em `body`:

```css
font-feature-settings: "cv11", "ss01", "ss03";
```

Ativa variantes estilísticas (cifrão com barra vertical, formas alternativas de 'a' e 'g' no Inter) adequadas a conteúdo financeiro.

---

## 4. Espaçamento, raios e sombras

### 4.1 Variáveis CSS (`wwwroot/css/app.css`)

| Variável                | Valor                                                                       |
|-------------------------|-----------------------------------------------------------------------------|
| `--tp-radius-sm`        | `6px`                                                                       |
| `--tp-radius-md`        | `10px` (padrão do tema)                                                     |
| `--tp-radius-lg`        | `16px`                                                                      |
| `--tp-shadow-card`      | `0 1px 2px rgba(15,23,42,0.04), 0 4px 12px rgba(15,23,42,0.06)`             |
| `--tp-shadow-card-hover`| `0 4px 8px rgba(15,23,42,0.06), 0 16px 32px rgba(15,23,42,0.10)`            |
| `--tp-transition`       | `200ms cubic-bezier(0.4, 0, 0.2, 1)`                                        |

### 4.2 Tokens do MudTheme

| Token                 | Valor   |
|-----------------------|---------|
| `DefaultBorderRadius` | `10px`  |
| `AppbarHeight`        | `68px`  |
| `DrawerWidthLeft`     | `280px` |

### 4.3 Diretrizes de espaçamento

- **Seções entre blocos**: `mt-8` / `mb-8` (32px) ou maior.
- **Cards**: padding interno de `pa-4` (16px) a `pa-6` (24px).
- **Footer**: `py-10` (40px vertical).
- **Container principal**: `MaxWidth.Large` (~1100px) com `pt-8 pb-12`.

---

## 5. Layout principal

O `MainLayout.razor` implementa o shell global da aplicação.

```
┌─────────────────────────────────────────────────────────┐
│  AppBar (translúcido, 68px)                             │
│  [☰] ▣ TaxPlanner        Início Análise Tabelas Blog 🌗│
├─────────────────────────────────────────────────────────┤
│                                                         │
│  <MudMainContent>  (pt-8 pb-12)                        │
│  @Body                                                 │
│                                                         │
├─────────────────────────────────────────────────────────┤
│  Footer (Surface, 4 colunas)                            │
│  [brand]  [ferramentas]  [recursos]  [privacidade]      │
│  ─────────────────────────────────────────────         │
│  © 2026 TaxPlanner                       feito no 🇧🇷  │
└─────────────────────────────────────────────────────────┘
```

### 5.1 AppBar (`.tp-appbar`)

- **Cor**: `Color.Surface` com `backdrop-filter: saturate(180%) blur(12px)` — glassmorphism sutil.
- **Borda inferior**: `1px solid var(--mud-palette-divider)`.
- **Em mobile (`<md`)**: oculta navegação inline e exibe `MudIconButton` de menu (☰) que abre o drawer.
- **Em desktop (`≥md`)**: links inline `.tp-nav-link`, divisor vertical, e toggle de tema com `MudTooltip`.
- **Estado ativo**: classe `.tp-nav-link-active` aplicada dinamicamente por `GetActiveClass(path)`.

### 5.2 Drawer mobile (`.tp-drawer`)

- `MudDrawer` com `Variant.DrawerVariant.Temporary` e `Anchor.Start`.
- Fundo `var(--mud-palette-drawer-background)` (preto-azulado no claro, mais escuro no dark).
- Header com brand mark, `MudNavMenu` com ícones, rodapé com copyright.
- Fecha automaticamente após `OnClick="CloseDrawer"` em cada `MudNavLink`.

### 5.3 Brand mark (`.tp-brand`)

Combinação fixa usada no AppBar, drawer e footer:

```
┌──┐  TaxPlanner
│📄│  Planejamento tributário
└──┘
```

- Quadrado de **38px** com cantos `10px` arredondados, gradiente verde-menta, ícone `ReceiptLong` (Material Rounded).
- Tagline em caixa-alta com `letter-spacing: 0.08em`.

### 5.4 Footer (`.tp-footer`)

Quatro colunas em `≥md`, duas em `sm`, uma em `xs`:

1. **Marca + descrição** (5/12)
2. **Ferramentas** (2/12) — Análise Tributária · Tabelas · Blog
3. **Recursos** (2/12) — Como funciona · Privacidade · FAQ
4. **Transparência** (3/12) — `MudAlert` verde com ícone de cadeado

Rodapé inferior com copyright e "feito no Brasil" + ícone de coração.

---

## 6. Componentes e padrões de uso

### 6.1 Botões

| Variante     | Uso                                                |
|--------------|----------------------------------------------------|
| `Filled`     | CTA principal (ex.: "Começar Simulação")           |
| `Outlined`   | Ação secundária                                    |
| `Text`       | Ação terciária (ex.: "Ler" em cards)               |

- Todos com `Color="Color.Primary"` (preto) por padrão; use `Color="Color.Secondary"` (verde) para destaques financeiros.
- `StartIcon` / `EndIcon` Material Rounded quando relacionado a finanças/recibos, Outlined quando neutro.

### 6.2 Cards

- `MudCard` com `Elevation="2"` em conteúdo principal, `Elevation="0"` em cards de "como funciona" (com borda).
- Padding `pa-4` a `pa-6`.
- `MudCardHeader` com `CardHeaderAvatar` quando há ícone associado.

### 6.3 Alertas

| Severidade   | Cor              | Uso                                          |
|--------------|------------------|----------------------------------------------|
| `Success`    | Verde            | Confirmação, alerta de privacidade           |
| `Info`       | Azul             | Avisos informativos, isenção de responsabilidade |
| `Warning`    | Âmbar            | Atenção                                      |
| `Error`      | Vermelho         | Validação, erro                              |

Variante preferida: `Outlined` com `Dense="true"` quando usado em footer/lateral; sem `Dense` quando dentro do conteúdo.

### 6.4 Ícones

Biblioteca Material Symbols. Padrão de escolha:

- **Rounded** (`@Icons.Material.Rounded.*`) — marca, elementos financeiros, recibo.
- **Outlined** (`@Icons.Material.Outlined.*`) — navegação, ações, listagens.
- **Filled** (`@Icons.Material.Filled.*`) — indicadores de status, números de passo.

### 6.5 Navegação

- Links principais ficam no AppBar (desktop) e no drawer (mobile).
- Itens: **Início**, **Análise Tributária**, **Tabelas**, **Blog**.
- Adicionar novo item: edite `MainLayout.razor` em ambos os blocos (`.tp-nav-desktop` e `MudNavMenu`).

---

## 7. Conteúdo de blog

A classe `.blog-content` é aplicada em `<article>` que renderiza HTML de posts. Estilos definidos em `wwwroot/css/app.css`.

| Elemento     | Estilo                                                                 |
|--------------|------------------------------------------------------------------------|
| `h1`, `h2`   | Cor primária no `h1`; `h2` com `border-bottom` 1px divider             |
| `h3`, `h4`   | Hierarquia tipográfica do tema                                         |
| `p`          | `line-height: 1.7`, `margin: 0.85rem 0`                               |
| `blockquote` | Borda esquerda 4px `secondary`, fundo `secondary` a 7%                 |
| `pre`        | Fundo `#0F172A`, raio `--tp-radius-md`, fonte JetBrains Mono           |
| `code` (inline) | Fundo `secondary` a 12%, padding `0.15rem 0.45rem`                  |
| `table`      | Bordas 1px divider, cabeçalho com fundo `secondary` a 10%             |
| `img`        | `max-width: 100%`, raio `--tp-radius-md`, sombra card                 |
| `a`          | `color: secondary`, sublinhado 1px com offset 2px                     |
| `hr`         | `border-top: 1px divider`, margem vertical 2rem                        |

A highlight de sintaxe em `<pre>` é feita por **Prism.js** (CDN) com tema `prism-tomorrow`.

---

## 8. Acessibilidade

- `lang="pt-BR"` no `<html>`.
- `meta name="theme-color"` sincronizado com `AppbarBackground` em ambos os modos.
- `MudTooltip` em botões só com ícone (toggle de tema, abrir menu).
- `aria-label` em `MudIconButton` sem texto visível.
- Contraste mínimo AA em todos os pares cor/fundo das paletas light e dark.
- Foco visível herdado do MudBlazor (anéis de foco de alta visibilidade em ambos os modos).

---

## 9. Como aplicar o tema em novos componentes

```razor
@* 1. Use as cores semânticas do MudTheme — nunca cor literal  *@
<MudCard>
    <MudText Color="Color.TextPrimary">Título</MudText>
    <MudText Color="Color.TextSecondary">Descrição</MudText>
    <MudButton Color="Color.Secondary" Variant="Variant.Filled">Ação</MudButton>
</MudCard>

@* 2. Para ajustes fora do MudBlazor, use variáveis CSS:  *@
<div class="tp-card-minha">...</div>

<style>
    .tp-card-minha {
        background: var(--mud-palette-surface);
        color: var(--mud-palette-text-primary);
        border: 1px solid var(--mud-palette-divider);
        border-radius: var(--tp-radius-md);
        padding: 1.25rem;
    }
</style>
```

**Nunca** use cores literais (`#0F172A`, `#0D9488`) em componentes — sempre via `var(--mud-palette-*)` para que o modo escuro funcione automaticamente.

---

## 10. Checklist de revisão de UI

Antes de abrir um PR com mudanças visuais, confirme:

- [ ] Todos os textos estão em **português (pt-BR)**.
- [ ] Cores usadas são **tokens semânticos** (não hex literais).
- [ ] Funciona em **modo claro e escuro**.
- [ ] Layout responsivo em **mobile** (`<md`) — drawer abre/fecha, sem overflow horizontal.
- [ ] Botões só com ícone têm `aria-label` e/ou `MudTooltip`.
- [ ] Cards têm hierarquia: `Header` (com `CardHeaderAvatar` se houver ícone) → `Content` → `Actions`.
- [ ] Espaçamentos respeitam a escala (4, 8, 12, 16, 24, 32, 40, 48 px).
- [ ] `dotnet build` passa com **0 erros e 0 warnings**.
- [ ] Nenhuma regressão em `MainLayout` (AppBar, drawer, footer continuam funcionais).
