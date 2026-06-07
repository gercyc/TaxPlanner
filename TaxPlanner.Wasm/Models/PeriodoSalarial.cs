namespace TaxPlanner.Wasm.Models;

public class PeriodoSalarial
{
    public DateOnly DataInicio { get; set; }
    public DateOnly DataFim { get; set; }
    public decimal SalarioBruto { get; set; }

    public int MesesNoPeriodo
    {
        get
        {
            if (DataFim < DataInicio) return 0;
            return (DataFim.Year - DataInicio.Year) * 12 + DataFim.Month - DataInicio.Month + 1;
        }
    }
}
