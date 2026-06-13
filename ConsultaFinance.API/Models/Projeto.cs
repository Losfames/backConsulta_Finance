namespace ConsultaFinance.API.Models;

public class Projeto
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string Descricao { get; set; } = string.Empty;

    public decimal OrcamentoTotal { get; set; }

    public DateTime DataInicio { get; set; }

    public DateTime DataFim { get; set; }

    public int UsuarioId { get; set; }

    public Usuario? Usuario { get; set; }

    public ICollection<Despesa>? Despesas { get; set; }
}