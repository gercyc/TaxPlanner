# Módulo: Identidade (Identity)

- **Projeto**: `TaxPlanner.Server`
- **Componentes**: `Components/Account/*`
- **Configuração**: `Program.cs`
- **Status**: Configurado parcialmente — UI de autenticação não exposta
  publicamente

## Propósito

Autenticação de usuários (administradores do blog) baseada em **ASP.NET
Core Identity** com **cookies** (`IdentityConstants.ApplicationScheme`).
Suporte a **passkeys** (FIDO2/WebAuthn) para login sem senha.

## Componentes principais

- `Components/Account/IdentityComponentsEndpointRouteBuilderExtensions.cs`
  — endpoints de Razor Components para registro, login, logout, etc.
- `Components/Account/IdentityNoOpEmailSender.cs` — implementação **no-op**
  de envio de e-mail (sem provedor SMTP real configurado).
- `Components/Account/IdentityRedirectManager.cs` — gerencia
  redirecionamentos após login/logout.
- `Components/Account/IdentityRevalidatingAuthenticationStateProvider.cs`
  — revalida o estado de autenticação periodicamente.
- `Components/Account/PasskeyInputModel.cs` + `PasskeyOperation.cs` —
  inputs e operações para credenciais FIDO2.

## Configuração (`Program.cs`)

```csharp
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider,
    IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
.AddIdentityCookies();

builder.Services.AddIdentityCore<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Stores.SchemaVersion = IdentitySchemaVersions.Version3;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddSignInManager()
.AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>,
    IdentityNoOpEmailSender>();
```

## Modelo de dados

- `ApplicationUser` — herda de `IdentityUser` (tabela `AspNetUsers`).
- Tabelas Identity v3 criadas pelo EF Core:
  - `AspNetUsers`, `AspNetRoles`, `AspNetUserRoles`,
  - `AspNetUserClaims`, `AspNetRoleClaims`, `AspNetUserLogins`,
  - `AspNetUserTokens`.

## Fluxo de autenticação

1. Usuário acessa rota admin.
2. Razor Component detecta ausência de auth state e redireciona para
   `/Account/Login` (componentes do namespace
   `TaxPlanner.Server.Components.Account`).
3. Após login, cookie é emitido e a sessão é revalidada periodicamente
   pelo `IdentityRevalidatingAuthenticationStateProvider`.
4. Para passkeys, o componente `PasskeyInputModel` lida com a
   cerimônia FIDO2.

## Endpoints (Razor Components)

Mapeados via `app.MapAdditionalIdentityEndpoints()` em
`Program.cs`. Rotas comuns:

- `/Account/Login`
- `/Account/Register`
- `/Account/Logout`
- `/Account/Manage` (gestão de conta)
- `/Account/AccessDenied`

## Pontos abertos (dívida)

1. **Sem UI pública de login exposta no Wasm** — o frontend Wasm é
   público; a área admin (no Server) ainda precisa ter o
   `MapRazorComponents<App>().AddInteractiveServerRenderMode()`
   ajustado para entregar os componentes de auth.
2. **Sem provedor de e-mail real** — `IdentityNoOpEmailSender` ignora
   confirmações (que já estão desativadas via
   `SignIn.RequireConfirmedAccount = false`).
3. **Endpoints admin da API REST (`/api/admin/*`) não usam
   `[Authorize]`** — ver [`blog-admin.md`](./blog-admin.md).
4. **CORS permissivo** no `Program.cs` (`AllowAnyOrigin`).

## Referências

- `TaxPlanner.Server/Program.cs`
- `TaxPlanner.Server/Components/Account/*`
- `TaxPlanner.Server/Data/ApplicationUser.cs`
- `TaxPlanner.Server/Data/ApplicationDbContext.cs`
- [`../integrations.md`](../integrations.md)
- [`../architecture.md`](../architecture.md)
