using MudBlazor;

namespace TaxPlanner.Wasm.Theme;

public static class ClaudeTheme
{
    public static MudTheme CreateTheme() => new()
    {
        PaletteLight = new PaletteLight
        {
            Black = "#141413",
            White = "#faf9f5",
            Background = "#faf9f5",
            Surface = "#efe9de",
            DrawerBackground = "#efe9de",
            DrawerText = "#141413",
            DrawerIcon = "#8e8b82",
            AppbarBackground = "#efe9de",
            AppbarText = "#141413",
            TextPrimary = "#141413",
            TextSecondary = "#8e8b82",
            TextDisabled = "#b0aea5",
            Primary = "#d97757",
            PrimaryContrastText = "#faf9f5",
            PrimaryDarken = "#c46a4c",
            PrimaryLighten = "#e08f73",
            Secondary = "#6a9bcc",
            SecondaryContrastText = "#faf9f5",
            SecondaryDarken = "#5a87b5",
            SecondaryLighten = "#82afd6",
            Tertiary = "#788c5d",
            TertiaryContrastText = "#faf9f5",
            Success = "#788c5d",
            SuccessContrastText = "#faf9f5",
            Error = "#b54a3f",
            ErrorContrastText = "#faf9f5",
            Warning = "#c49a3c",
            WarningContrastText = "#141413",
            Info = "#6a9bcc",
            InfoContrastText = "#faf9f5",
            ActionDefault = "#8e8b82",
            ActionDisabled = "#b0aea5",
            ActionDisabledBackground = "#e0dbd0",
            Divider = "#e0dbd0",
            LinesDefault = "#e0dbd0",
            TableLines = "#e0dbd0",
            Dark = "#141413",
        },

        PaletteDark = new PaletteDark
        {
            Black = "#0d0d0c",
            White = "#1F1F1E",
            Background = "#1F1F1E",
            Surface = "#262626",
            DrawerBackground = "#262626",
            DrawerText = "#F6F6F4",
            DrawerIcon = "#8F8D83",
            AppbarBackground = "#2C2C2A",
            AppbarText = "#F6F6F4",
            TextPrimary = "#F6F6F4",
            TextSecondary = "#8F8D83",
            TextDisabled = "#5a5850",
            Primary = "#A65F47",
            PrimaryContrastText = "#F6F6F4",
            PrimaryDarken = "#8f4f3a",
            PrimaryLighten = "#b8735c",
            Secondary = "#6a9bcc",
            SecondaryContrastText = "#F6F6F4",
            SecondaryDarken = "#5a87b5",
            SecondaryLighten = "#82afd6",
            Tertiary = "#788c5d",
            TertiaryContrastText = "#F6F6F4",
            Success = "#788c5d",
            SuccessContrastText = "#F6F6F4",
            Error = "#c45e52",
            ErrorContrastText = "#F6F6F4",
            Warning = "#d4b056",
            WarningContrastText = "#141413",
            Info = "#6a9bcc",
            InfoContrastText = "#F6F6F4",
            ActionDefault = "#8F8D83",
            ActionDisabled = "#5a5850",
            ActionDisabledBackground = "#3a3a38",
            Divider = "#3a3a38",
            LinesDefault = "#3a3a38",
            TableLines = "#3a3a38",
            Dark = "#F6F6F4",
        },

        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "8px",
            AppbarHeight = "64px",
        },

        Typography = new Typography
        {
            Default = new DefaultTypography
            {
                FontFamily = ["Inter", "system-ui", "-apple-system", "Segoe UI", "Roboto", "Helvetica Neue", "Arial", "sans-serif"],
                FontSize = "0.9375rem",
                FontWeight = "400",
                LineHeight = "1.5",
                LetterSpacing = "-0.01em",
            },
            H1 = new H1Typography
            {
                FontSize = "2.5rem",
                FontWeight = "600",
                LineHeight = "1.2",
                LetterSpacing = "-0.02em",
            },
            H2 = new H2Typography
            {
                FontSize = "2rem",
                FontWeight = "600",
                LineHeight = "1.25",
                LetterSpacing = "-0.02em",
            },
            H3 = new H3Typography
            {
                FontSize = "1.625rem",
                FontWeight = "600",
                LineHeight = "1.3",
                LetterSpacing = "-0.015em",
            },
            H4 = new H4Typography
            {
                FontSize = "1.375rem",
                FontWeight = "600",
                LineHeight = "1.35",
                LetterSpacing = "-0.01em",
            },
            H5 = new H5Typography
            {
                FontSize = "1.125rem",
                FontWeight = "600",
                LineHeight = "1.4",
                LetterSpacing = "-0.005em",
            },
            H6 = new H6Typography
            {
                FontSize = "1rem",
                FontWeight = "600",
                LineHeight = "1.4",
                LetterSpacing = "0em",
            },
            Button = new ButtonTypography
            {
                FontSize = "0.875rem",
                FontWeight = "600",
                LineHeight = "1.75",
                LetterSpacing = "0.01em",
                TextTransform = "none",
            },
        },

        Shadows = new Shadow
        {
            Elevation =
            [
                "none",
                "0px 1px 2px rgba(20,20,19,0.06), 0px 1px 3px rgba(20,20,19,0.10)",
                "0px 2px 4px rgba(20,20,19,0.06), 0px 4px 6px rgba(20,20,19,0.10)",
                "0px 4px 6px rgba(20,20,19,0.06), 0px 10px 15px rgba(20,20,19,0.10)",
                "0px 10px 15px rgba(20,20,19,0.06), 0px 20px 25px rgba(20,20,19,0.10)",
                "0px 20px 25px rgba(20,20,19,0.06), 0px 25px 50px rgba(20,20,19,0.10)",
                "0px 25px 50px rgba(20,20,19,0.12)",
                "0px 25px 50px rgba(20,20,19,0.14)",
                "0px 25px 50px rgba(20,20,19,0.16)",
                "0px 25px 50px rgba(20,20,19,0.18)",
                "0px 25px 50px rgba(20,20,19,0.20)",
                "0px 25px 50px rgba(20,20,19,0.22)",
                "0px 25px 50px rgba(20,20,19,0.24)",
                "0px 25px 50px rgba(20,20,19,0.26)",
                "0px 25px 50px rgba(20,20,19,0.28)",
                "0px 25px 50px rgba(20,20,19,0.30)",
                "0px 25px 50px rgba(20,20,19,0.32)",
                "0px 25px 50px rgba(20,20,19,0.34)",
                "0px 25px 50px rgba(20,20,19,0.36)",
                "0px 25px 50px rgba(20,20,19,0.38)",
                "0px 25px 50px rgba(20,20,19,0.40)",
                "0px 25px 50px rgba(20,20,19,0.42)",
                "0px 25px 50px rgba(20,20,19,0.44)",
                "0px 25px 50px rgba(20,20,19,0.46)",
                "0px 25px 50px rgba(20,20,19,0.48)",
                "0px 25px 50px rgba(20,20,19,0.50)",
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
