namespace ConsultaFinance.API.Models;

public class Despesa
{
    public int Id { get; set; }

    public string Descricao { get; set; } = string.Empty;

    public string Categoria { get; set; } = string.Empty;

    public decimal ValorOrcado { get; set; }

    public decimal ValorRealizado { get; set; }

    public DateTime Data { get; set; }

    public int ProjetoId { get; set; }

    public Projeto? Projeto { get; set; }
}