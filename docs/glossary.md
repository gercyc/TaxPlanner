# Glossário — TaxPlanner

Termos canônicos do domínio e tecnologia usados no projeto **TaxPlanner**,
com siglas e desambiguações.

## Domínio (planejamento tributário)

| Termo                       | Definição                                                                                          |
| --------------------------- | -------------------------------------------------------------------------------------------------- |
| **Planejamento tributário** | Conjunto de estratégias legais para otimizar a carga de impostos de pessoa física ou jurídica.    |
| **Tributo**                 | Pagamento obrigatório previsto em lei (impostos, taxas, contribuições de melhoria, contribuições especiais). |
| **Imposto**                 | Tributo cuja obrigação tem por fato gerador situação independente de qualquer atividade estatal.  |
| **Receita bruta**           | Total de receitas antes de deduções.                                                               |
| **Alíquota**                | Percentual aplicado sobre a base de cálculo para apurar o tributo.                                  |
| **Base de cálculo**         | Valor sobre o qual se aplica a alíquota.                                                            |
| **Lucro real**              | Regime tributário no qual o IRPJ e a CSLL incidem sobre o lucro contábil ajustado.                 |
| **Lucro presumido**         | Regime no qual a base de cálculo do IRPJ/CSLL é presumida a partir da receita bruta.                |
| **Simples Nacional**        | Regime unificado de tributação para micro e pequenas empresas.                                      |

## Tecnologia e arquitetura

| Termo / Sigla         | Significado                                                                                  |
| --------------------- | -------------------------------------------------------------------------------------------- |
| **Blazor**            | Framework .NET para construir UI web interativa em C# (Server e WebAssembly).                |
| **Blazor WebAssembly**| Hospedagem do Blazor no cliente (navegador) — a UI roda em WebAssembly.                       |
| **Blazor Server**     | Hospedagem do Blazor no servidor — UI processada no servidor, atualizações via SignalR.     |
| **Wasm**              | WebAssembly — formato de binário executado em navegadores modernos.                          |
| **PWA**               | Progressive Web App — aplicação web instalável, com service worker e manifest.              |
| **EF Core**           | Entity Framework Core — ORM oficial do .NET.                                                |
| **DbContext**         | Unidade de trabalho do EF Core (`ApplicationDbContext` neste projeto).                       |
| **Migration**         | Versão do schema EF Core (gerada por `dotnet ef migrations add`).                            |
| **Npgsql**            | Provider PostgreSQL para .NET / EF Core.                                                     |
| **MudBlazor**         | Biblioteca de componentes Material Design para Blazor.                                      |
| **MudTheme**          | Objeto de configuração de tema do MudBlazor.                                                 |
| **Razor**             | Sintaxe que mistura HTML + C# usada em `.razor`.                                             |
| **Identity**          | `Microsoft.AspNetCore.Identity` — sistema de autenticação/autorização do ASP.NET Core.      |
| **Cookie auth**       | Esquema de autenticação baseado em cookies HTTP (`IdentityConstants.ApplicationScheme`).    |
| **Passkey**           | Credencial sem senha baseada em chaves criptográficas (FIDO2/WebAuthn).                      |
| **RSS**               | Formato de feed para distribuição de conteúdo (usado em `/api/posts/rss`).                    |
| **Slug**              | Versão URL-safe de um título (ex.: `planejamento-tributario-2026`).                          |
| **Markdown**          | Linguagem de marcação leve — convertida para HTML via `Markdig`.                             |
| **Markdig**           | Biblioteca .NET para renderizar Markdown em HTML.                                            |
| **CORS**              | Cross-Origin Resource Sharing — política de compartilhamento de recursos entre origens.     |
| **Path traversal**    | Ataque em que `..` é usado para acessar arquivos fora do diretório permitido.                |

## Identificadores internos

| Identificador              | Onde aparece                                                   |
| -------------------------- | -------------------------------------------------------------- |
| `TaxPlanner.Wasm`          | Projeto frontend Blazor WebAssembly.                            |
| `TaxPlanner.Server`        | Projeto backend ASP.NET Core + Blazor Server.                  |
| `BlogService`              | Serviço de domínio do blog (`TaxPlanner.Server/Services/`).    |
| `ApplicationDbContext`     | DbContext agregador (Identity + `Posts`).                       |
| `ApplicationUser`          | Entidade de usuário do Identity.                                |
| `Post`                     | Entidade de post (`TaxPlanner.Server/Models/Post.cs`).         |
| `PostsController`          | Endpoints públicos de leitura.                                  |
| `AdminPostsController`     | Endpoints administrativos de CRUD.                              |
| `TaxPlannerTheme`          | `MudTheme` corporativo (`TaxPlanner.Wasm/Theme/`).              |
| `MainLayout.razor`         | Layout raiz com AppBar translúcido e drawer mobile.             |
| `service-worker.js`        | Service worker de desenvolvimento (cache desativado).           |
| `service-worker.published.js` | Service worker de produção (cache offline).                  |

## Desambiguação

- **"Tema"** pode significar (a) o `MudTheme` em código
  (`TaxPlannerTheme.cs`) ou (b) o tema visual (claro/escuro) escolhido pelo
  usuário. Documentos referem-se a (a) como **tema/MudTheme** e a (b) como
  **modo claro/escuro** ou **color scheme**.
- **"Post"** é a unidade de conteúdo do blog. Não confundir com **"Post"**
  de HTTP (`POST /api/admin/posts`).
- **"Server"** em `TaxPlanner.Server` é o **projeto backend**. Não confundir
  com **"Blazor Server"**, que é um modo de hospedagem específico dentro
  desse projeto.
- **"Admin"** refere-se a usuários autenticados com acesso ao painel
  administrativo. No estado atual do código, os endpoints
  `/api/admin/*` **não** exigem autenticação — este é um trabalho pendente
  registrado em [`integrations.md`](./integrations.md).
