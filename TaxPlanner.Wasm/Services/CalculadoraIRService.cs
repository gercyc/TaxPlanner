using TaxPlanner.Wasm.Models;

namespace TaxPlanner.Wasm.Services;

public class CalculadoraIRService
{
    // ── INSS (empregado CLT) ──────────────────────────────────────────────────

    private static readonly (decimal Limite, decimal Aliquota)[] TabelaInss2024 =
    [
        (1_412.00m, 0.075m),
        (2_666.68m, 0.090m),
        (4_000.03m, 0.120m),
        (7_786.02m, 0.140m),
    ];
    private const decimal TetoInss2024 = 908.85m;

    // 2025 — reajuste conforme Portaria MPS 1.432/2024
    private static readonly (decimal Limite, decimal Aliquota)[] TabelaInss2025 =
    [
        (1_518.00m, 0.075m),
        (2_793.88m, 0.090m),
        (4_190.83m, 0.120m),
        (8_157.41m, 0.140m),
    ];
    private const decimal TetoInss2025 = 951.62m;

    // 2026 — Portaria Interministerial MPS/MF nº 13/2026
    private static readonly (decimal Limite, decimal Aliquota)[] TabelaInss2026 =
    [
        (1_621.00m, 0.075m),
        (2_902.84m, 0.090m),
        (4_354.27m, 0.120m),
        (8_475.55m, 0.140m),
    ];
    private const decimal TetoInss2026 = 988.09m;

    // 2027 — tabela ainda não publicada; utilizamos 2026 como estimativa
    private static readonly (decimal Limite, decimal Aliquota)[] TabelaInss2027 = TabelaInss2026;
    private const decimal TetoInss2027 = TetoInss2026;

    // ── IRRF Mensal ───────────────────────────────────────────────────────────

    private static readonly (decimal Limite, decimal Aliquota, decimal Deducao)[] TabelaIrrfMensal2024 =
    [
        (2_824.00m,        0.000m,     0.00m),
        (3_751.05m,        0.075m,   211.80m),
        (4_664.68m,        0.150m,   494.04m),
        (5_903.69m,        0.225m,   844.42m),
        (decimal.MaxValue, 0.275m, 1_474.68m),
    ];

    // 2025 — reajuste conforme MP 1.294/2025
    private static readonly (decimal Limite, decimal Aliquota, decimal Deducao)[] TabelaIrrfMensal2025 =
    [
        (3_036.00m,        0.000m,     0.00m),
        (4_000.00m,        0.075m,   227.70m),
        (5_000.00m,        0.150m,   527.70m),
        (6_101.06m,        0.225m,   902.55m),
        (decimal.MaxValue, 0.275m, 1_708.08m),
    ];

    // 2026 — Lei nº 15.191/2025 (tabela progressiva); redução aplicada separadamente (Lei nº 15.270/2025)
    private static readonly (decimal Limite, decimal Aliquota, decimal Deducao)[] TabelaIrrfMensal2026 =
    [
        (2_428.80m,        0.000m,     0.00m),
        (2_826.65m,        0.075m,   182.16m),
        (3_751.05m,        0.150m,   394.16m),
        (4_664.68m,        0.225m,   675.49m),
        (decimal.MaxValue, 0.275m,   908.73m),
    ];

    // 2027 — tabela ainda não publicada; utilizamos 2026 como estimativa
    private static readonly (decimal Limite, decimal Aliquota, decimal Deducao)[] TabelaIrrfMensal2027 = TabelaIrrfMensal2026;

    // ── IR Anual (declaração IRPF) ────────────────────────────────────────────

    private static readonly (decimal Limite, decimal Aliquota, decimal Deducao)[] TabelaIrAnual2024 =
    [
        (33_888.00m,       0.000m,       0.00m),
        (45_012.60m,       0.075m,   2_541.60m),
        (55_976.16m,       0.150m,   5_922.54m),
        (70_844.28m,       0.225m,   9_671.28m),
        (decimal.MaxValue, 0.275m,  17_613.66m),
    ];

    // 2025 — equivalente anual da tabela mensal 2025
    private static readonly (decimal Limite, decimal Aliquota, decimal Deducao)[] TabelaIrAnual2025 =
    [
        (36_432.00m,       0.000m,       0.00m),
        (48_000.00m,       0.075m,   2_732.40m),
        (60_000.00m,       0.150m,   6_332.40m),
        (73_212.72m,       0.225m,  10_832.28m),
        (decimal.MaxValue, 0.275m,  18_497.28m),
    ];

    // 2026 — Lei nº 15.191/2025 (tabela progressiva anual exercício 2027); redução anual aplicada separadamente
    private static readonly (decimal Limite, decimal Aliquota, decimal Deducao)[] TabelaIrAnual2026 =
    [
        (29_145.60m,       0.000m,       0.00m),
        (33_919.80m,       0.075m,   2_185.92m),
        (45_012.60m,       0.150m,   4_729.91m),
        (55_976.16m,       0.225m,   8_105.85m),
        (decimal.MaxValue, 0.275m,  10_904.66m),
    ];

    // 2027 — tabela ainda não publicada; utilizamos 2026 como estimativa
    private static readonly (decimal Limite, decimal Aliquota, decimal Deducao)[] TabelaIrAnual2027 = TabelaIrAnual2026;

    // ── Tabelas de Redução (Lei nº 15.270/2025, vigente a partir de 2026) ────

    private const decimal LimiteIsencaoMensal   = 5_000.00m;
    private const decimal LimiteFimReducaoMensal = 7_350.00m;
    private const decimal ReducaoMaximaMensal   = 312.89m;
    private const decimal ReducaoBaseMensal     = 978.62m;
    private const decimal ReducaoCoefMensal     = 0.133145m;

    private const decimal LimiteIsencaoAnual    = 60_000.00m;
    private const decimal LimiteFimReducaoAnual = 88_200.00m;
    private const decimal ReducaoMaximaAnual    = 2_694.15m;
    private const decimal ReducaoBaseAnual      = 8_429.73m;
    private const decimal ReducaoCoefAnual      = 0.095575m;

    private static decimal AplicarReducaoMensal(decimal imposto, decimal baseCalculo, int ano)
    {
        if (ano < 2026 || baseCalculo <= 0) return imposto;

        decimal reducao;
        if (baseCalculo <= LimiteIsencaoMensal)
            reducao = Math.Min(imposto, ReducaoMaximaMensal);
        else if (baseCalculo <= LimiteFimReducaoMensal)
            reducao = Math.Min(imposto, ReducaoBaseMensal - ReducaoCoefMensal * baseCalculo);
        else
            return imposto;

        return Math.Max(0, imposto - reducao);
    }

    private static decimal AplicarReducaoAnual(decimal imposto, decimal baseAnual, int ano)
    {
        if (ano < 2026 || baseAnual <= 0) return imposto;

        decimal reducao;
        if (baseAnual <= LimiteIsencaoAnual)
            reducao = Math.Min(imposto, ReducaoMaximaAnual);
        else if (baseAnual <= LimiteFimReducaoAnual)
            reducao = Math.Min(imposto, ReducaoBaseAnual - ReducaoCoefAnual * baseAnual);
        else
            return imposto;

        return Math.Max(0, imposto - reducao);
    }

    // ── Seleção por ano ───────────────────────────────────────────────────────

    private static (decimal Limite, decimal Aliquota)[] ObterTabelaInss(int ano) => ano switch
    {
        2024 => TabelaInss2024,
        2025 => TabelaInss2025,
        2026 => TabelaInss2026,
        _    => TabelaInss2027,
    };

    private static decimal ObterTetoInss(int ano) => ano switch
    {
        2024 => TetoInss2024,
        2025 => TetoInss2025,
        2026 => TetoInss2026,
        _    => TetoInss2027,
    };

    private static (decimal Limite, decimal Aliquota, decimal Deducao)[] ObterTabelaIrrfMensal(int ano) => ano switch
    {
        2024 => TabelaIrrfMensal2024,
        2025 => TabelaIrrfMensal2025,
        2026 => TabelaIrrfMensal2026,
        _    => TabelaIrrfMensal2027,
    };

    private static (decimal Limite, decimal Aliquota, decimal Deducao)[] ObterTabelaIrAnual(int ano) => ano switch
    {
        2024 => TabelaIrAnual2024,
        2025 => TabelaIrAnual2025,
        2026 => TabelaIrAnual2026,
        _    => TabelaIrAnual2027,
    };

    // ── Métodos de cálculo ────────────────────────────────────────────────────

    public decimal CalcularInss(decimal salarioBruto, int ano)
    {
        if (salarioBruto <= 0) return 0;

        var tabela = ObterTabelaInss(ano);
        var teto   = ObterTetoInss(ano);

        decimal inss = 0;
        decimal limiteAnterior = 0;

        foreach (var (limite, aliquota) in tabela)
        {
            if (salarioBruto <= limiteAnterior) break;

            var baseNaFaixa = Math.Min(salarioBruto, limite) - limiteAnterior;
            inss += baseNaFaixa * aliquota;
            limiteAnterior = limite;

            if (salarioBruto <= limite) break;
        }

        if (salarioBruto > tabela[^1].Limite)
            inss = teto;

        return Math.Round(inss, 2);
    }

    public decimal CalcularIrrfMensal(decimal baseCalculo, int ano)
    {
        if (baseCalculo <= 0) return 0;

        decimal impostoBruto = 0;
        foreach (var (limite, aliquota, deducao) in ObterTabelaIrrfMensal(ano))
        {
            if (baseCalculo <= limite)
            {
                impostoBruto = Math.Max(0, baseCalculo * aliquota - deducao);
                break;
            }
        }

        var impostoFinal = AplicarReducaoMensal(impostoBruto, baseCalculo, ano);
        return Math.Round(impostoFinal, 2);
    }

    public decimal CalcularIrAnual(decimal baseAnual, int ano)
    {
        if (baseAnual <= 0) return 0;

        decimal impostoBruto = 0;
        foreach (var (limite, aliquota, deducao) in ObterTabelaIrAnual(ano))
        {
            if (baseAnual <= limite)
            {
                impostoBruto = Math.Max(0, baseAnual * aliquota - deducao);
                break;
            }
        }

        var impostoFinal = AplicarReducaoAnual(impostoBruto, baseAnual, ano);
        return Math.Round(impostoFinal, 2);
    }

    public bool TabelaEstimada(int ano) => ano >= 2027;

    public ResultadoIR Calcular(List<PeriodoSalarial> periodos, bool incluirDecimo, int anoCalendario)
    {
        var resultado = new ResultadoIR { IncluiDecimo = incluirDecimo };
        var mesesCobertos = new Dictionary<(int ano, int mes), decimal>();

        foreach (var periodo in periodos)
        {
            var cursor = new DateOnly(periodo.DataInicio.Year, periodo.DataInicio.Month, 1);
            var fim    = new DateOnly(periodo.DataFim.Year,   periodo.DataFim.Month,   1);

            while (cursor <= fim)
            {
                if (cursor.Year == anoCalendario)
                    mesesCobertos[(cursor.Year, cursor.Month)] = periodo.SalarioBruto;

                cursor = cursor.AddMonths(1);
            }
        }

        for (int mes = 1; mes <= 12; mes++)
        {
            if (!mesesCobertos.TryGetValue((anoCalendario, mes), out var salario))
                continue;

            var inss        = CalcularInss(salario, anoCalendario);
            var baseCalculo = Math.Max(0, salario - inss);
            var irrf        = CalcularIrrfMensal(baseCalculo, anoCalendario);

            resultado.RendimentoBrutoAnual += salario;
            resultado.InssAnual            += inss;
            resultado.IrrfRetidoAnual      += irrf;

            resultado.DetalhesMensais.Add(new DetalheMensal
            {
                Mes          = mes,
                Ano          = anoCalendario,
                SalarioBruto = salario,
                Inss         = inss,
                BaseCalculo  = baseCalculo,
                IrrfRetido   = irrf,
            });
        }

        resultado.BaseDeCaculoAnual = Math.Max(0, resultado.RendimentoBrutoAnual - resultado.InssAnual);
        resultado.IrDevidoAnual     = CalcularIrAnual(resultado.BaseDeCaculoAnual, anoCalendario);

        if (incluirDecimo && resultado.DetalhesMensais.Count > 0)
        {
            var salarioDecimo = resultado.DetalhesMensais.Last().SalarioBruto;
            resultado.SalarioDecimoTerceiro = salarioDecimo;

            var inssDecimo = CalcularInss(salarioDecimo, anoCalendario);
            resultado.InssDecimoTerceiro = inssDecimo;

            var baseDecimo = Math.Max(0, salarioDecimo - inssDecimo);
            resultado.BaseCaculoDecimoTerceiro = baseDecimo;

            resultado.IrDevidoDecimoTerceiro    = CalcularIrrfMensal(baseDecimo, anoCalendario);
            resultado.IrrfRetidoDecimoTerceiro  = resultado.IrDevidoDecimoTerceiro;
        }

        resultado.ResultadoFinal = resultado.IrDevidoAnual - resultado.IrrfRetidoAnual;

        return resultado;
    }

    public TabelasImposto ObterTabelas(int ano)
    {
        var tabelas = new TabelasImposto
        {
            Ano = ano,
            TabelaEstimada = TabelaEstimada(ano),
            TetoInss = ObterTetoInss(ano),
            PossuiReducao = ano >= 2026,
            LimiteIsencaoMensal = LimiteIsencaoMensal,
            LimiteFimReducaoMensal = LimiteFimReducaoMensal,
            ReducaoMaximaMensal = ReducaoMaximaMensal,
            LimiteIsencaoAnual = LimiteIsencaoAnual,
            LimiteFimReducaoAnual = LimiteFimReducaoAnual,
            ReducaoMaximaAnual = ReducaoMaximaAnual,
        };

        foreach (var (limite, aliquota) in ObterTabelaInss(ano))
        {
            tabelas.Inss.Add(new FaixaInss { Limite = limite, Aliquota = aliquota });
        }

        foreach (var (limite, aliquota, deducao) in ObterTabelaIrrfMensal(ano))
        {
            tabelas.IrrfMensal.Add(new FaixaIr { Limite = limite, Aliquota = aliquota, Deducao = deducao });
        }

        foreach (var (limite, aliquota, deducao) in ObterTabelaIrAnual(ano))
        {
            tabelas.IrAnual.Add(new FaixaIr { Limite = limite, Aliquota = aliquota, Deducao = deducao });
        }

        return tabelas;
    }
}
