using MudBlazor;

namespace TaxPlanner.Wasm.Theme;

/// <summary>
/// Identidade visual corporativa do TaxPlanner.
/// Paleta inspirada em publicações financeiras editoriais: base carvão/grafite sóbria
/// com verde-menta como cor de destaque (tributos, receita, resultado positivo).
/// </summary>
public static class TaxPlannerTheme
{
    // ── Tokens compartilhados (claro/escuro) ────────────────────────────────
    private const string BrandPrimary = "#0F172A";      // Preto-carvão (autoridade)
    private const string BrandSecondary = "#0D9488";   // Verde-menta (tributos)
    private const string BrandTertiary = "#475569";    // Cinza-grafite
    private const string BrandAccent = "#14B8A6";      // Verde-menta claro (destaque)

    public static MudTheme CreateTheme() => new()
    {
        PaletteLight = new PaletteLight
        {
            // Superfícies
            Black = "#0B1220",
            White = "#FFFFFF",
            Background = "#FAFAF9",
            Surface = "#FFFFFF",

            // Drawer
            DrawerBackground = "#0F172A",
            DrawerText = "#E2E8F0",
            DrawerIcon = "#94A3B8",

            // AppBar
            AppbarBackground = "#0F172A",
            AppbarText = "#F8FAFC",

            // Tipografia
            TextPrimary = "#0F172A",
            TextSecondary = "#475569",
            TextDisabled = "#94A3B8",

            // Identidade
            Primary = BrandPrimary,
            PrimaryContrastText = "#FFFFFF",
            PrimaryDarken = "#020617",
            PrimaryLighten = "#1E293B",

            Secondary = BrandSecondary,
            SecondaryContrastText = "#FFFFFF",
            SecondaryDarken = "#0F766E",
            SecondaryLighten = BrandAccent,

            Tertiary = BrandTertiary,
            TertiaryContrastText = "#FFFFFF",
            TertiaryDarken = "#334155",
            TertiaryLighten = "#64748B",

            // Estados
            Success = "#059669",
            SuccessContrastText = "#FFFFFF",
            Error = "#DC2626",
            ErrorContrastText = "#FFFFFF",
            Warning = "#D97706",
            WarningContrastText = "#FFFFFF",
            Info = "#0284C7",
            InfoContrastText = "#FFFFFF",

            // Ações / linhas
            ActionDefault = "#475569",
            ActionDisabled = "#CBD5E1",
            ActionDisabledBackground = "#F1F5F9",
            Divider = "#E2E8F0",
            LinesDefault = "#E2E8F0",
            TableLines = "#E2E8F0",
            TableStriped = "#F8FAFC",
            TableHover = "#F1F5F9",
            Dark = "#0F172A",
        },

        PaletteDark = new PaletteDark
        {
            Black = "#020617",
            White = "#F8FAFC",
            Background = "#0B1220",
            Surface = "#111827",

            DrawerBackground = "#020617",
            DrawerText = "#E2E8F0",
            DrawerIcon = "#64748B",

            AppbarBackground = "#020617",
            AppbarText = "#F8FAFC",

            TextPrimary = "#F8FAFC",
            TextSecondary = "#94A3B8",
            TextDisabled = "#475569",

            Primary = "#F8FAFC",
            PrimaryContrastText = "#0F172A",
            PrimaryDarken = "#CBD5E1",
            PrimaryLighten = "#FFFFFF",

            Secondary = BrandAccent,
            SecondaryContrastText = "#0F172A",
            SecondaryDarken = BrandSecondary,
            SecondaryLighten = "#2DD4BF",

            Tertiary = "#64748B",
            TertiaryContrastText = "#F8FAFC",
            TertiaryDarken = "#475569",
            TertiaryLighten = "#94A3B8",

            Success = "#10B981",
            SuccessContrastText = "#0F172A",
            Error = "#F87171",
            ErrorContrastText = "#0F172A",
            Warning = "#FBBF24",
            WarningContrastText = "#0F172A",
            Info = "#38BDF8",
            InfoContrastText = "#0F172A",

            ActionDefault = "#94A3B8",
            ActionDisabled = "#334155",
            ActionDisabledBackground = "#1F2937",
            Divider = "#1F2937",
            LinesDefault = "#1F2937",
            TableLines = "#1F2937",
            TableStriped = "#0F172A",
            TableHover = "#1F2937",
            Dark = "#F8FAFC",
        },

        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "10px",
            AppbarHeight = "68px",
            DrawerWidthLeft = "280px",
            DrawerWidthRight = "280px",
        },

        Typography = new Typography
        {
            Default = new DefaultTypography
            {
                FontFamily = ["\"Plus Jakarta Sans\"", "\"Inter\"", "system-ui", "-apple-system", "Segoe UI", "Roboto", "Helvetica Neue", "Arial", "sans-serif"],
                FontSize = "0.9375rem",
                FontWeight = "400",
                LineHeight = "1.6",
                LetterSpacing = "-0.005em",
            },
            H1 = new H1Typography
            {
                FontFamily = ["\"Plus Jakarta Sans\"", "system-ui", "sans-serif"],
                FontSize = "2.75rem",
                FontWeight = "700",
                LineHeight = "1.15",
                LetterSpacing = "-0.025em",
            },
            H2 = new H2Typography
            {
                FontFamily = ["\"Plus Jakarta Sans\"", "system-ui", "sans-serif"],
                FontSize = "2.125rem",
                FontWeight = "700",
                LineHeight = "1.2",
                LetterSpacing = "-0.02em",
            },
            H3 = new H3Typography
            {
                FontFamily = ["\"Plus Jakarta Sans\"", "system-ui", "sans-serif"],
                FontSize = "1.625rem",
                FontWeight = "600",
                LineHeight = "1.3",
                LetterSpacing = "-0.015em",
            },
            H4 = new H4Typography
            {
                FontFamily = ["\"Plus Jakarta Sans\"", "system-ui", "sans-serif"],
                FontSize = "1.375rem",
                FontWeight = "600",
                LineHeight = "1.35",
                LetterSpacing = "-0.01em",
            },
            H5 = new H5Typography
            {
                FontFamily = ["\"Plus Jakarta Sans\"", "system-ui", "sans-serif"],
                FontSize = "1.125rem",
                FontWeight = "600",
                LineHeight = "1.4",
                LetterSpacing = "-0.005em",
            },
            H6 = new H6Typography
            {
                FontFamily = ["\"Plus Jakarta Sans\"", "system-ui", "sans-serif"],
                FontSize = "1rem",
                FontWeight = "600",
                LineHeight = "1.45",
                LetterSpacing = "0em",
            },
            Button = new ButtonTypography
            {
                FontFamily = ["\"Plus Jakarta Sans\"", "system-ui", "sans-serif"],
                FontSize = "0.875rem",
                FontWeight = "600",
                LineHeight = "1.5",
                LetterSpacing = "0.005em",
                TextTransform = "none",
            },
            Subtitle1 = new Subtitle1Typography
            {
                FontFamily = ["\"Plus Jakarta Sans\"", "system-ui", "sans-serif"],
                FontSize = "1rem",
                FontWeight = "500",
                LineHeight = "1.5",
            },
            Subtitle2 = new Subtitle2Typography
            {
                FontFamily = ["\"Plus Jakarta Sans\"", "system-ui", "sans-serif"],
                FontSize = "0.875rem",
                FontWeight = "600",
                LineHeight = "1.45",
                LetterSpacing = "0.01em",
            },
            Body1 = new Body1Typography
            {
                FontSize = "1rem",
                FontWeight = "400",
                LineHeight = "1.65",
            },
            Body2 = new Body2Typography
            {
                FontSize = "0.875rem",
                FontWeight = "400",
                LineHeight = "1.6",
            },
        },

        Shadows = new Shadow
        {
            Elevation =
            [
                "none",
                "0 1px 2px rgba(15,23,42,0.04), 0 1px 3px rgba(15,23,42,0.06)",
                "0 2px 4px rgba(15,23,42,0.04), 0 4px 8px rgba(15,23,42,0.06)",
                "0 4px 6px rgba(15,23,42,0.04), 0 10px 20px rgba(15,23,42,0.08)",
                "0 8px 16px rgba(15,23,42,0.06), 0 20px 30px rgba(15,23,42,0.08)",
                "0 12px 24px rgba(15,23,42,0.08), 0 24px 40px rgba(15,23,42,0.10)",
                "0 16px 32px rgba(15,23,42,0.10), 0 28px 50px rgba(15,23,42,0.12)",
                "0 20px 40px rgba(15,23,42,0.12), 0 32px 60px rgba(15,23,42,0.14)",
                "0 24px 48px rgba(15,23,42,0.14), 0 36px 70px rgba(15,23,42,0.16)",
                "0 28px 56px rgba(15,23,42,0.16), 0 40px 80px rgba(15,23,42,0.18)",
                "0 32px 64px rgba(15,23,42,0.18), 0 44px 90px rgba(15,23,42,0.20)",
                "0 36px 72px rgba(15,23,42,0.20), 0 48px 100px rgba(15,23,42,0.22)",
                "0 40px 80px rgba(15,23,42,0.22), 0 52px 110px rgba(15,23,42,0.24)",
                "0 44px 88px rgba(15,23,42,0.24), 0 56px 120px rgba(15,23,42,0.26)",
                "0 48px 96px rgba(15,23,42,0.26), 0 60px 130px rgba(15,23,42,0.28)",
                "0 52px 104px rgba(15,23,42,0.28), 0 64px 140px rgba(15,23,42,0.30)",
                "0 56px 112px rgba(15,23,42,0.30), 0 68px 150px rgba(15,23,42,0.32)",
                "0 60px 120px rgba(15,23,42,0.32), 0 72px 160px rgba(15,23,42,0.34)",
                "0 64px 128px rgba(15,23,42,0.34), 0 76px 170px rgba(15,23,42,0.36)",
                "0 68px 136px rgba(15,23,42,0.36), 0 80px 180px rgba(15,23,42,0.38)",
                "0 72px 144px rgba(15,23,42,0.38), 0 84px 190px rgba(15,23,42,0.40)",
                "0 76px 152px rgba(15,23,42,0.40), 0 88px 200px rgba(15,23,42,0.42)",
                "0 80px 160px rgba(15,23,42,0.42), 0 92px 210px rgba(15,23,42,0.44)",
                "0 84px 168px rgba(15,23,42,0.44), 0 96px 220px rgba(15,23,42,0.46)",
                "0 88px 176px rgba(15,23,42,0.46), 0 100px 230px rgba(15,23,42,0.48)",
                "0 92px 184px rgba(15,23,42,0.48), 0 104px 240px rgba(15,23,42,0.50)",
            ]
        },

        ZIndex = new ZIndex
        {
            Drawer = 1100,
            AppBar = 1200,
            Dialog = 1300,
            Popover = 1400,
            Snackbar = 1500,
            Tooltip = 1600,
        }
    };
}
