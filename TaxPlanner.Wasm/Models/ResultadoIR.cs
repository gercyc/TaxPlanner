namespace TaxPlanner.Wasm.Models;

public class ResultadoIR
{
    public decimal RendimentoBrutoAnual { get; set; }
    public decimal InssAnual { get; set; }
    public decimal BaseDeCaculoAnual { get; set; }
    public decimal IrDevidoAnual { get; set; }
    public decimal IrrfRetidoAnual { get; set; }

    public bool IncluiDecimo { get; set; }
    public decimal SalarioDecimoTerceiro { get; set; }
    public decimal InssDecimoTerceiro { get; set; }
    public decimal BaseCaculoDecimoTerceiro { get; set; }
    public decimal IrDevidoDecimoTerceiro { get; set; }
    public decimal IrrfRetidoDecimoTerceiro { get; set; }

    // Positivo = a pagar | Negativo = a restituir
    public decimal ResultadoFinal { get; set; }

    public List<DetalheMensal> DetalhesMensais { get; set; } = [];
}

public class DetalheMensal
{
    public int Mes { get; set; }
    public int Ano { get; set; }
    public decimal SalarioBruto { get; set; }
    public decimal Inss { get; set; }
    public decimal BaseCalculo { get; set; }
    public decimal IrrfRetido { get; set; }
}
