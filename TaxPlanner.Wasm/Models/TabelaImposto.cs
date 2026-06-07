namespace TaxPlanner.Wasm.Models;

public class FaixaInss
{
    public decimal Limite { get; set; }
    public decimal Aliquota { get; set; }
}

public class FaixaIr
{
    public decimal Limite { get; set; }
    public decimal Aliquota { get; set; }
    public decimal Deducao { get; set; }
}

public class TabelasImposto
{
    public int Ano { get; set; }
    public List<FaixaInss> Inss { get; set; } = [];
    public decimal TetoInss { get; set; }
    public List<FaixaIr> IrrfMensal { get; set; } = [];
    public List<FaixaIr> IrAnual { get; set; } = [];
    public bool PossuiReducao { get; set; }
    public bool TabelaEstimada { get; set; }

    // Redução mensal
    public decimal LimiteIsencaoMensal { get; set; }
    public decimal LimiteFimReducaoMensal { get; set; }
    public decimal ReducaoMaximaMensal { get; set; }

    // Redução anual
    public decimal LimiteIsencaoAnual { get; set; }
    public decimal LimiteFimReducaoAnual { get; set; }
    public decimal ReducaoMaximaAnual { get; set; }
}
